using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.FilesShell.Manager.Controllers;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Manager
{
	/// <summary>
	///		Intérprete del script
	/// </summary>
	internal class ScriptInterpreter
	{
		internal ScriptInterpreter(JobFileShellManager manager, JobStepModel step)
		{
			Manager = manager;
			Step = step;
		}

		/// <summary>
		///		Procesa las sentencias del script
		/// </summary>
		internal async Task<bool> ProcessAsync(BlockLogModel block, List<BaseSentence> program, 
											   NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			// Guarda los parámetros de entrada
			Parameters = parameters;
			// Ejecuta el programa
			await ExecuteAsync(block, program, cancellationToken);
			// Devuelve el valor que indica si todo es correcto
			return !HasError;
		}

		/// <summary>
		///		Procesa los datos
		/// </summary>
		private async Task ExecuteAsync(BlockLogModel block, List<BaseSentence> sentences, CancellationToken cancellationToken)
		{
			foreach (BaseSentence sentenceBase in sentences)
				if (!HasError && sentenceBase.Enabled && !cancellationToken.IsCancellationRequested)
					switch (sentenceBase)
					{
						case BlockSentence sentence:
								await ProcessBlockAsync(block, sentence, cancellationToken);
							break;
						case CopySentence sentence:
								await ProcessCopyAsync(block, sentence);
							break;
						case DeleteSentence sentence:
								await ProcessDeleteAsync(block, sentence);
							break;
						case ExecuteSentence sentence:
								await ProcessExecuteAsync(block, sentence);
							break;
						case ConvertFileSentence sentence:
								await ProcessConvertFileAsync(block, sentence, cancellationToken);
							break;
						case ConvertPathSentence sentence:
								await ProcessConvertPathAsync(block, sentence, cancellationToken);
							break;
						case IfExistsSentence sentence:
								await ProcessExistsAsync(block, sentence, cancellationToken);
							break;
					}
		}

		/// <summary>
		///		Procesa un bloque de sentencias
		/// </summary>
		private async Task ProcessBlockAsync(BlockLogModel parent, BlockSentence sentence, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, sentence.GetMessage("Start block")))
			{
				await ExecuteAsync(block, sentence.Sentences, cancellationToken);
			}
		}

		/// <summary>
		///		Procesa la sentencia que compureba si existe un archivo o directorio
		/// </summary>
		private async Task ProcessExistsAsync(BlockLogModel block, IfExistsSentence sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(sentence.Path))
				AddError(block, "Path is empty");
			else 
			{
				string path = Step.Project.GetFullFileName(sentence.Path);

					if (Directory.Exists(path) || File.Exists(path))
						await ExecuteAsync(block, sentence.IfSentences, cancellationToken);
					else
						await ExecuteAsync(block, sentence.ElseSentences, cancellationToken);
			}
		}

		/// <summary>
		///		Procesa una copia de archivos
		/// </summary>
		private async Task ProcessCopyAsync(BlockLogModel parent, CopySentence sentence)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start copy from '{sentence.Source}' to '{sentence.Target}'"))
			{
				string source = Step.Project.GetFullFileName(sentence.Source);
				string target = Step.Project.GetFullFileName(sentence.Target);

					try
					{
						if (Directory.Exists(source))
							await LibHelper.Files.HelperFiles.CopyPathAsync(source, target, sentence.Mask, sentence.Recursive, sentence.FlattenPaths);
						else if (File.Exists(source))
						{
							if (!string.IsNullOrWhiteSpace(Path.GetExtension(target)))
								await LibHelper.Files.HelperFiles.CopyFileAsync(source, target);
							else
								await LibHelper.Files.HelperFiles.CopyFileAsync(source, Path.Combine(target, Path.GetFileName(source)));
						}
						else
							AddError(block, $"Cant find source file or path '{source}'");
					}
					catch (Exception exception)
					{
						AddError(block, $"Error when copy '{source}' to '{target}'. {exception.Message}");
					}
			}
		}

		/// <summary>
		///		Procesa un borrado de archivos
		/// </summary>
		private async Task ProcessDeleteAsync(BlockLogModel parent, DeleteSentence sentence)
		{
			// Evita el warning
			await Task.Delay(1);
			// Borra los archivos / directorios
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start delete '{sentence.Path}'"))
			{
				string path = Step.Project.GetFullFileName(sentence.Path);

					try
					{
						if (Directory.Exists(path))
						{
							if (!string.IsNullOrWhiteSpace(sentence.Mask))
								LibHelper.Files.HelperFiles.KillFiles(path, sentence.Mask);
							else
								LibHelper.Files.HelperFiles.KillPath(path);
						}
						else if (File.Exists(path))
							LibHelper.Files.HelperFiles.KillFile(path);
						else
							block.Info($"Cant find file or path '{path}' for delete");
					}
					catch (Exception exception)
					{
						AddError(block, $"Error when delete '{path}'. {exception.Message}");
					}
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de proceso
		/// </summary>
		private async Task ProcessExecuteAsync(BlockLogModel parent, ExecuteSentence sentence)
		{
			// Evita el warning
			await Task.Delay(1);
			// Ejecuta la sentencia
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start execution '{sentence.Process}'"))
			{
				try
				{
					LibHelper.Processes.SystemProcessHelper processor = new LibHelper.Processes.SystemProcessHelper();

						// Ejecuta el proceso
						processor.ExecuteApplication(sentence.Process, ConvertArguments(sentence.Arguments), true, sentence.Timeout);
						// Log
						block.Info($"End execution '{sentence.Process}'");
				}
				catch (Exception exception)
				{
					AddError(block, $"Error when execute '{sentence.Process}'. {exception.Message}");
				}
			}
		}

		/// <summary>
		///		Convierte la lista de argumentos
		/// </summary>
		private string ConvertArguments(List<ExecuteSentenceArgument> arguments)
		{
			string parameters = string.Empty;

				// Crea la lista de argumentos
				foreach (ExecuteSentenceArgument argument in arguments)
				{
					string result = string.Empty;

						// Añade la clave
						if (!string.IsNullOrWhiteSpace(argument.Key))
							result += argument.Key.TrimIgnoreNull();
						// Añade el valor
						if (!string.IsNullOrWhiteSpace(argument.Value))
						{
							if (argument.TransformFileName)
								result = result.AddWithSeparator(Step.Project.GetFullFileName(argument.Value), " ");
							else
								result = result.AddWithSeparator(argument.Value, " ");
						}
						// Añade la cadena a la lista de parámetros
						if (!string.IsNullOrWhiteSpace(result))
							parameters = parameters.AddWithSeparator(result, " ");
				}
				// Devuelve la lista de argumentos
				return parameters;
		}

		/// <summary>
		///		Convierte los archivos de un directorio
		/// </summary>
		private async Task ProcessConvertPathAsync(BlockLogModel parent, ConvertPathSentence sentence, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start convert path '{sentence.Path}' from {sentence.Source} to {sentence.Target}"))
			{
				string path = Step.Project.GetFullFileName(sentence.Path);

					if (!Directory.Exists(path))
						AddError(block, $"Cant find the path '{path}'");
					else
						foreach (string source in Directory.GetFiles(path, "*" + sentence.SourceExtension))
							if (!cancellationToken.IsCancellationRequested && !HasError)
								await ConvertFileAsync(block, source, Path.Combine(path, Path.GetFileNameWithoutExtension(source) + sentence.TargetExtension),
													   GetExcelOptions(sentence.ExcelWithHeader, sentence.ExcelSheetIndex), cancellationToken);
			}
		}

		/// <summary>
		///		Convierte un archivo
		/// </summary>
		private async Task ProcessConvertFileAsync(BlockLogModel parent, ConvertFileSentence sentence, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start convert file '{sentence.FileNameSource}' to {sentence.FileNameTarget}"))
			{
				string source = Step.Project.GetFullFileName(sentence.FileNameSource);
				string target = Step.Project.GetFullFileName(sentence.FileNameTarget);

					if (!File.Exists(source))
						AddError(block, $"Cant find the file '{source}'");
					else 
						await ConvertFileAsync(block, source, target, GetExcelOptions(sentence.ExcelWithHeader, sentence.ExcelSheetIndex), cancellationToken);
			}
		}

		/// <summary>
		///		Obtiene las opciones de los archivos Excel
		/// </summary>
		private ExcelfileOptions GetExcelOptions(bool withHeader, int sheetIndex)
		{
			return new ExcelfileOptions
							{
								WithHeader = withHeader,
								SheetIndex = sheetIndex
							};
		}

		/// <summary>
		///		Convierte un archivo
		/// </summary>
		private async Task ConvertFileAsync(BlockLogModel block, string source, string target, Controllers.ExcelfileOptions options, CancellationToken cancellationToken)
		{
			bool converted = false;

				// Log
				block.Info($"Converting '{source}' to '{target}'");
				// Convierte el archivo
				if (source.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) && target.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
					converted = await new Controllers.CsvToParquetConversor().ConvertAsync(block, source, target, cancellationToken);
				else if (source.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase) && target.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
					converted = await new Controllers.ParquetToCsvConversor().ConvertAsync(block, source, target, cancellationToken);
				else if (source.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase) && target.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
					converted = await new Controllers.ExcelToParquetConversor().ConvertAsync(block, source, target, options, cancellationToken);
				// Indica el error si es necesario
				if (!converted)
					AddError(block, $"Cant convert file '{source}' to '{target}'");
		}

		/// <summary>
		///		Añade un error a la colección
		/// </summary>
		private void AddError(BlockLogModel block, string message, Exception exception = null)
		{
			block.Error(message, exception);
			Errors.Add(message + Environment.NewLine + exception?.Message);
		}

		/// <summary>
		///		Manager
		/// </summary>
		private JobFileShellManager Manager { get; }

		/// <summary>
		///		Paso de ejecución
		/// </summary>
		private JobStepModel Step { get; }

		/// <summary>
		///		Parámetros de entrada
		/// </summary>
		private NormalizedDictionary<object> Parameters { get; set; }

		/// <summary>
		///		Errores de proceso
		/// </summary>
		internal List<string> Errors { get; } = new List<string>();

		/// <summary>
		///		Indica si ha habido algún error
		/// </summary>
		public bool HasError 
		{ 
			get { return Errors.Count > 0; }
		}
	}
}