using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers
{
	/// <summary>
	///		Controlador para importar los datos de archivos a las bases de datos
	/// </summary>
	internal class ImportFileController : BaseFileController
	{
		internal ImportFileController(DbScriptProcessor processor) : base(processor)
		{
		}

		/// <summary>
		///		Procesa una importación de un archivo
		/// </summary>
		internal async Task<bool> ExecuteAsync(SentenceFileImport sentence, CancellationToken cancellationToken)
		{
			bool imported = false;

				// Recoge los datos necesarios e importa el archivo
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start import {sentence.FileName}"))
				{
					ProviderModel provider = Processor.GetProvider(sentence.Target);
					string fileName = Processor.Manager.Step.Project.GetFullFileName(sentence.FileName);
					string container = Processor.Manager.Step.Project.ApplyParameters(sentence.Container);

						// Compruba los datos e importa
						if (string.IsNullOrWhiteSpace(sentence.FileName))
							block.Error("The file name / folder is undefined");
						else if (!string.IsNullOrWhiteSpace(sentence.Source) && string.IsNullOrWhiteSpace(sentence.Container))
							block.Error("It's defined target but is not defined a container");
						else if (string.IsNullOrWhiteSpace(sentence.Source) && !string.IsNullOrWhiteSpace(sentence.Container))
							block.Error("It's defined a container but is not defined target");
						else if (provider == null)
							block.Error($"Can't find the provider. Key: '{sentence.Target}'");
						else
							try
							{
								// Importa el archivo
								await ImportAsync(block, provider, sentence, fileName, container, cancellationToken);
								// Indica que se ha ejecutado correctamente
								imported = true;
							}
							catch (Exception exception)
							{
								block.Error($"Error when import '{fileName}'", exception);
							}
				}
				// Devuelve el valor que indica si se ha importado
				return imported;
		}

		/// <summary>
		///		Importa los datos del archivo o directorio sobre el proveedor
		/// </summary>
		private async Task ImportAsync(BlockLogModel block, ProviderModel provider, SentenceFileImport sentence, string fileName, string container, CancellationToken cancellationToken)
		{
			using (Storage.BaseFileStorage fileManager = GetStorageManager(Processor, sentence, container))
			{
				// Abre la conexión
				fileManager.Open();
				// Importa los archivos
				await foreach (string file in fileManager.GetFilesAsync(fileName, sentence.ImportFolder, sentence.GetExtension()))
					if (!cancellationToken.IsCancellationRequested)
					{
						// Log
						block.Info($"Importing {System.IO.Path.GetFileName(file)}");
						// Importa el archivo
						GetFileImplementation(sentence).Import(block, provider, sentence, await fileManager.GetStreamAsync(file, Storage.BaseFileStorage.OpenFileMode.Read));
					}
			}
		}
	}
}
