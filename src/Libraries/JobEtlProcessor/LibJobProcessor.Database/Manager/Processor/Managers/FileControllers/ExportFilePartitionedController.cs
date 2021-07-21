using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers
{
	/// <summary>
	///		Controlador para exportar datos de una consulta SQL a una serie de archivos CSV particionados
	/// </summary>
	internal class ExportFilePartitionedController : BaseFileController
	{
		internal ExportFilePartitionedController(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una exportación de una consulta a CSV
		/// </summary>
		internal async	Task<bool> ExecuteAsync(SentenceFileExportPartitioned sentence, CancellationToken cancellationToken)
		{
			bool exported = false;

				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, 
																						  $"Start exporting partitioned to {System.IO.Path.GetFileName(sentence.FileName)}"))
				{
					if (string.IsNullOrWhiteSpace(sentence.Command.Sql))
						block.Error("There is not command at export sentence");
					else
					{
						ProviderModel provider = Processor.GetProvider(sentence.Source);
						string baseFileName = Processor.Manager.Step.Project.GetFullFileName(sentence.FileName);

							if (provider == null)
								block.Error($"Can't find the provider. Key: '{sentence.Source}'");
							else
							{
								string container = Processor.Manager.Step.Project.ApplyParameters(sentence.Container);
								CommandModel command = Processor.ConvertProviderCommand(sentence.Command, out string error);

									if (!string.IsNullOrWhiteSpace(error))
										block.Error($"Error when convert export command. {error}");
									else
										try
										{
											// Exporta los datos
											using (Storage.BaseFileStorage fileManager = GetStorageManager(Processor, sentence, container))
											{
												// Abre la conexión
												fileManager.Open();
												// Exporta los datos
												await GetFileImplementation(sentence).ExportPartitionedAsync(block, fileManager, provider, command, sentence, baseFileName, cancellationToken);
											}
											// Indica que se ha exportado correctamente
											exported = true;
										}
										catch (Exception exception)
										{
											block.Error($"Error when export '{sentence.FileName}'", exception);
										}
							}
					}
				}
				// Devuelve el valor que indica si se ha exportado correctamente
				return exported;
		}
	}
}