using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers
{
	/// <summary>
	///		Controlador para exportar datos de una consulta SQL a un archivo
	/// </summary>
	internal class ExportFileController : BaseFileController
	{
		internal ExportFileController(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una exportación de una consulta a un archivo
		/// </summary>
		internal async Task<bool> ExecuteAsync(SentenceFileExport sentence, CancellationToken cancellationToken)
		{
			bool exported = false;
			string fileName = Processor.Manager.Step.Project.GetFullFileName(sentence.FileName);

				// Exporta los datos
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start exporting to '{fileName}'"))
				{
					if (string.IsNullOrWhiteSpace(sentence.Command.Sql))
						block.Error("There is not command at export sentence");
					else if (string.IsNullOrWhiteSpace(fileName))
						block.Error("The file name is undefined");
					else if (!string.IsNullOrWhiteSpace(sentence.Target) && string.IsNullOrWhiteSpace(sentence.Container))
						block.Error("It's defined target but is not defined a container");
					else if (string.IsNullOrWhiteSpace(sentence.Target) && !string.IsNullOrWhiteSpace(sentence.Container))
						block.Error("It's defined a container but is not defined target");
					else
					{
						string container = Processor.Manager.Step.Project.ApplyParameters(sentence.Container);
						ProviderModel provider = Processor.GetProvider(sentence.Source);

							if (provider == null)
								block.Error($"Can't find the provider. Key: '{sentence.Source}'");
							else
							{
								CommandModel command = Processor.ConvertProviderCommand(sentence.Command, out string error);

									if (!string.IsNullOrWhiteSpace(error))
										block.Error($"Error when convert export command. {error}");
									else
										try
										{
											// Exporta al archivo
											using (Storage.BaseFileStorage fileManager = GetStorageManager(Processor, sentence, container))
											{
												// Abre la conexión
												fileManager.Open();
												// Crea el directorio
												await fileManager.CreatePathAsync(System.IO.Path.GetDirectoryName(fileName));
												// Exporta los datos
												using (System.IO.Stream stream = await fileManager.GetStreamAsync(fileName, Storage.BaseFileStorage.OpenFileMode.Write))
												{
													GetFileImplementation(sentence).Export(block, stream, provider, command, sentence);
												}
												// Indica que se ha exportado correctamente
												exported = true;
											}
										}
										catch (Exception exception)
										{
											block.Error($"Error when export to '{fileName}'", exception);
										}
							}
					}
				}
				// Devuelve el valor que indica si se ha exportado correctamente
				return exported;
		}
	}
}
