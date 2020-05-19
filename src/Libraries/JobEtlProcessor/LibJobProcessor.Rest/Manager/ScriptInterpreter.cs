using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Rest.Manager
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
								await ConvertFileAsync(block, sentence, cancellationToken);
							break;
						case ConvertPathSentence sentence:
								await ConvertPathAsync(block, sentence, cancellationToken);
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
		///		Procesa una copia de archivos
		/// </summary>
		private async Task ProcessCopyAsync(BlockLogModel parent, CopySentence sentence)
		{
			// Evita el warning
			await Task.Delay(1);
			// Copia los archivos / directorios
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start copy from '{sentence.Source}' to '{sentence.Target}'"))
			{
				string source = Step.Project.GetFullFileName(sentence.Source);
				string target = Step.Project.GetFullFileName(sentence.Target);

					try
					{
						if (System.IO.Directory.Exists(source))
							LibHelper.Files.HelperFiles.CopyPath(source, target, sentence.Mask);
						else if (System.IO.File.Exists(source))
							LibHelper.Files.HelperFiles.CopyFile(source, target);
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
						if (System.IO.Directory.Exists(path))
						{
							if (!string.IsNullOrWhiteSpace(sentence.Mask))
								LibHelper.Files.HelperFiles.KillFiles(path, sentence.Mask);
							else
								LibHelper.Files.HelperFiles.KillPath(path);
						}
						else if (System.IO.File.Exists(path))
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
		private async Task ConvertPathAsync(BlockLogModel parent, ConvertPathSentence sentence, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start convert path '{sentence.Path}' from {sentence.Source} to {sentence.Target}"))
			{
				string path = Step.Project.GetFullFileName(sentence.Path);

					if (!System.IO.Directory.Exists(path))
						AddError(block, $"Cant find the path '{path}'");
					else
						try
						{
							switch (sentence.Source)
							{
								case ConvertPathSentence.FileType.Csv:
										if (sentence.Target == ConvertPathSentence.FileType.Parquet)
											foreach (string source in System.IO.Directory.GetFiles(path, "*.csv"))
												if (!cancellationToken.IsCancellationRequested)
												{
													string target = System.IO.Path.Combine(path, System.IO.Path.GetFileNameWithoutExtension(source) + ".parquet");

														await new Controllers.CsvToParquetConversor().ConvertAsync(source, target, cancellationToken);
												}
										else
											AddError(block, $"Cant convert '{sentence.Source}' to '{sentence.Target}'");
									break;
								case ConvertPathSentence.FileType.Parquet:
										if (sentence.Target == ConvertPathSentence.FileType.Csv)
											foreach (string source in System.IO.Directory.GetFiles(path, "*.parquet"))
												if (!cancellationToken.IsCancellationRequested)
												{
													string target = System.IO.Path.Combine(path, System.IO.Path.GetFileNameWithoutExtension(source) + ".csv");

														await new Controllers.ParquetToCsvConversor().ConvertAsync(source, target, cancellationToken);
												}
										else
											AddError(block, $"Cant convert '{sentence.Source}' to '{sentence.Target}'");
									break;
								default:
										AddError(block, $"Cant convert '{sentence.Source}' to '{sentence.Target}'");
									break;
							}
						}
						catch (Exception exception)
						{
							AddError(block, $"Error when convert path '{path}'", exception);
						}
			}
		}

		/// <summary>
		///		Convierte un archivo
		/// </summary>
		private async Task ConvertFileAsync(BlockLogModel parent, ConvertFileSentence sentence, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start convert file '{sentence.FileNameSource}' to {sentence.FileNameTarget}"))
			{
				string source = Step.Project.GetFullFileName(sentence.FileNameSource);
				string target = Step.Project.GetFullFileName(sentence.FileNameTarget);

					if (!System.IO.File.Exists(source))
						AddError(block, $"Cant find the file '{source}'");
					else if (source.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) && target.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
						try
						{
							await new Controllers.CsvToParquetConversor().ConvertAsync(source, target, cancellationToken);
						}
						catch (Exception exception)
						{
							AddError(block, $"Cant convert '{source}' to '{target}'", exception);
						}
					else if (source.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase) && target.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
						try
						{
							await new Controllers.ParquetToCsvConversor().ConvertAsync(source, target, cancellationToken);
						}
						catch (Exception exception)
						{
							AddError(block, $"Cant convert '{source}' to '{target}'", exception);
						}
					else
						AddError(block, $"Cant convert file '{source}' to '{target}'");
			}
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