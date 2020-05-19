using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.Csv
{
	/// <summary>
	///		Controlador para exportar datos de una consulta SQL a un archivo CSV
	/// </summary>
	internal class ExportCsvManager : BaseExportCsvManager
	{
		internal ExportCsvManager(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una exportación de una consulta a CSV
		/// </summary>
		internal bool Execute(SentenceExportCsv sentence)
		{
			bool exported = false;
			string fileName = Processor.Manager.GetFullFileName(sentence.FileName);

				// Exporta los datos
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start exporting to '{fileName}'"))
				{
					if (string.IsNullOrWhiteSpace(sentence.Command.Sql))
						block.Error("There is not command at export sentence");
					else
					{
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
											// Exporta los datos
											Export(fileName, provider, command, sentence, block);
											// Indica que se ha exportado correctamente
											exported = true;
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

		/// <summary>
		///		Exporta los datos de un comando
		/// </summary>
		private void Export(string fileName, ProviderModel provider, CommandModel command, SentenceExportCsv sentence, BlockLogModel block)
		{
			using (IDataReader reader = provider.OpenReader(command, sentence.Timeout))
			{
				long records = 0;

					// Escribe en el archivo
					using (CsvWriter writer = new CsvWriter(sentence.Definition))
					{
						List<ColumnModel> columns = GetColumns(reader);

							// Crea el directorio del archivo
							LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
							// Abre el archivo
							writer.Open(fileName);
							// Añade las cabeceras
							writer.WriteHeaders(GetHeaders(columns, sentence.Definition.TypedHeader));
							// Graba las líneas
							while (reader.Read())
							{
								// Graba los valores
								writer.WriteRow(GetValues(columns, reader));
								// Lanza el evento de progreso
								if (++records % sentence.BatchSize == 0)
									block.Progress($"Copying {records:#,##0}", records, 0);
							}
							// Log
							block.Info($"Exported {records:#,##0} records");
					}
			}
		}
	}
}
