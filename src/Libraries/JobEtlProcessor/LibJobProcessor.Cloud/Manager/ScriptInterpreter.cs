using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Cloud.Models;
using Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Cloud.Manager
{
	/// <summary>
	///		Intérprete del script
	/// </summary>
	internal class ScriptInterpreter
	{
		internal ScriptInterpreter(JobCloudManager manager, JobStepModel step, NormalizedDictionary<CloudConnection> connections)
		{
			Manager = manager;
			Step = step;
			Connections = connections;
		}

		/// <summary>
		///		Procesa las sentencias del script
		/// </summary>
		internal async Task<bool> ProcessAsync(BlockLogModel block, List<BaseSentence> program, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
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
						case UploadBlobSentence sentence:
								await ProcessUploadAsync(block, sentence);
							break;
						case DownloadBlobSentence sentence:
								await ProcessDownloadAsync(block, sentence);
							break;
						case CopyBlobSentence sentence:
								await ProcessCopyAsync(block, sentence);
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
		///		Sube un archivo al blob
		/// </summary>
		private async Task ProcessUploadAsync(BlockLogModel parent, UploadBlobSentence sentence)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start uploading to '{sentence.Target.ToString()}'"))
			{
				CloudConnection connection = GetConnection(sentence.StorageKey);

					if (connection == null)
						AddError(block, $"Can't find the connection for '{sentence.StorageKey}'");
					else
					{
						string fileName = Step.Project.GetFullFileName(sentence.FileName);

							if (!System.IO.File.Exists(fileName))
								AddError(block, $"Can't find the file '{fileName}'");
							else
								try
								{
									// Sube el archivo
									using (ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(connection.StorageConnectionString))
									{
										await manager.UploadAsync(sentence.Target.Container, sentence.Target.Blob, fileName);
									}
									// Log
									block.Info($"Uploaded file '{sentence.FileName}' to '{sentence.Target.ToString()}'");
								}
								catch (Exception exception)
								{
									AddError(block, $"Error when upload '{sentence.FileName}' to '{sentence.Target.ToString()}'", exception);
								}
					}
			}
		}

		/// <summary>
		///		Descarga un archivo del blob
		/// </summary>
		private async Task ProcessDownloadAsync(BlockLogModel parent, DownloadBlobSentence sentence)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start downloading from '{sentence.Source.ToString()}'"))
			{
				CloudConnection connection = GetConnection(sentence.StorageKey);

					if (connection == null)
						AddError(block, $"Can't find the connection for '{sentence.StorageKey}'");
					else
						try
						{
							// Descarga el archivo
							using (ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(connection.StorageConnectionString))
							{
								string fileName = Step.Project.GetFullFileName(sentence.FileName);

									// Crea el directorio
									LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
									// Descarga el archivo
									await manager.DownloadAsync(sentence.Source.Container, sentence.Source.Blob, fileName);
							}
							// Log
							block.Info($"Downloaded file '{sentence.FileName}' to '{sentence.Source.ToString()}'");
						}
						catch (Exception exception)
						{
							AddError(block, $"Error when download '{sentence.FileName}' to '{sentence.Source.ToString()}'", exception);
						}
			}
		}

		/// <summary>
		///		Procesa una copia de archivos
		/// </summary>
		private async Task ProcessCopyAsync(BlockLogModel parent, CopyBlobSentence sentence)
		{
			string processType = sentence.Move ? "move" : "copy";
			string blobTarget = sentence.Target.Blob;

				// Obtiene el nombre del archivo destino si hay que transformarlo en un nombre único
				if (sentence.TransformFileName)
					blobTarget = System.IO.Path.GetFileNameWithoutExtension(sentence.Target.Blob) +
									$" {DateTime.UtcNow:yyyy-MM-dd HH_mm_ss_ms}" +
									System.IO.Path.GetExtension(sentence.Target.Blob);
				// Procesa la instrucción
				using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, $"Start {processType} from '{sentence.Source.ToString()}' to '{sentence.Target.Container}/{blobTarget}'"))
				{
					CloudConnection connection = GetConnection(sentence.StorageKey);

						if (connection == null)
							AddError(block, $"Can't find the connection for '{sentence.StorageKey}'");
						else
							try
							{
								// Copia / mueve el archivo
								using (ICloudStorageManager manager = new StorageManager().OpenAzureStorageBlob(connection.StorageConnectionString))
								{
									if (sentence.Move)
										await manager.MoveAsync(sentence.Source.Container, sentence.Target.Blob, sentence.Target.Container, blobTarget);
									else
										await manager.CopyAsync(sentence.Source.Container, sentence.Target.Blob, sentence.Target.Container, blobTarget);
								}
								// Log
								block.Info($"End {processType} from '{sentence.Source.ToString()}' to '{sentence.Target.Container}/{blobTarget}'");
							}
							catch (Exception exception)
							{
								AddError(block, $"Error when {processType} from '{sentence.Source.ToString()}' to '{sentence.Target.Container}/{blobTarget}'", exception);
							}
				}
		}

		/// <summary>
		///		Obtiene la conexión
		/// </summary>
		private CloudConnection GetConnection(string storageKey)
		{
			if (!string.IsNullOrWhiteSpace(storageKey) && Connections.ContainsKey(storageKey))
				return Connections[storageKey];
			else
				return null;
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
		private JobCloudManager Manager { get; }

		/// <summary>
		///		Paso de ejecución
		/// </summary>
		private JobStepModel Step { get; }

		/// <summary>
		///		Conexiones
		/// </summary>
		private NormalizedDictionary<CloudConnection> Connections { get; }

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