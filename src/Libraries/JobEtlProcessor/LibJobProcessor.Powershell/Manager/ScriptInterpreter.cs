using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Powershell.Models;

namespace Bau.Libraries.LibJobProcessor.Powershell.Manager
{
	/// <summary>
	///		Intérprete del script
	/// </summary>
	internal class ScriptInterpreter
	{
		internal ScriptInterpreter(JobPowershellManager manager, JobStepModel step)
		{
			Manager = manager;
			Step = step;
		}

		/// <summary>
		///		Procesa las sentencias del script
		/// </summary>
		internal async Task<bool> ProcessAsync(BlockLogModel block, List<BaseSentence> program, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			// Ejecuta el programa
			await ExecuteAsync(block, program, parameters, cancellationToken);
			// Devuelve el valor que indica si todo es correcto
			return !HasError;
		}

		/// <summary>
		///		Procesa los datos
		/// </summary>
		private async Task ExecuteAsync(BlockLogModel block, List<BaseSentence> sentences, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			foreach (BaseSentence sentenceBase in sentences)
				if (!HasError && sentenceBase.Enabled && !cancellationToken.IsCancellationRequested)
					switch (sentenceBase)
					{
						case ExecuteScriptSentence sentence:
								await ProcessScriptAsync(block, sentence, parameters, cancellationToken);
							break;
						default:
								AddError(block, $"Unknown sentence {sentenceBase.GetType().ToString()}");
							break;
					}
		}

		/// <summary>
		///		Procesa un script
		/// </summary>
		private async Task ProcessScriptAsync(BlockLogModel parent, ExecuteScriptSentence sentence, NormalizedDictionary<object> parameters, 
											  CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start execute script"))
			{
				string fileName = Step.Project.GetFullFileName(sentence.FileName);

					if (!System.IO.File.Exists(fileName))
						AddError(block, $"Can't find the file '{fileName}'");
					else
						try
						{
							// Sube el archivo
							//using (ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(connection.StorageConnectionString))
							//{
							//	await manager.UploadAsync(sentence.Target.Container, sentence.Target.Blob, fileName);
							//}
							// Log
							block.Info($"Executed the script");
						}
						catch (Exception exception)
						{
							AddError(block, "Error when execute script sentence", exception);
						}
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
		private JobPowershellManager Manager { get; }

		/// <summary>
		///		Paso de ejecución
		/// </summary>
		private JobStepModel Step { get; }

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