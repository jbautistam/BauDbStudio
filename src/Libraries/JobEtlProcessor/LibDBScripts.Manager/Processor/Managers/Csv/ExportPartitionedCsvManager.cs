using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Csv;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Managers.Csv
{
	/// <summary>
	///		Controlador para exportar datos de una consulta SQL a una serie de archivos CSV particionados
	/// </summary>
	internal class ExportPartitionedCsvManager : BaseExportCsvManager
	{
		internal ExportPartitionedCsvManager(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una exportación de una consulta a CSV
		/// </summary>
		internal bool Execute(SentenceExportPartitionedCsv sentence)
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
						string baseFileName = Processor.Manager.GetFullFileName(sentence.FileName);

							if (provider == null)
								block.Error($"Can't find the provider. Key: '{sentence.Source}'");
							else if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(baseFileName)))
								block.Error($"Can't find the directory '{System.IO.Path.GetDirectoryName(baseFileName)}'");
							else
							{
								CommandModel command = Processor.ConvertProviderCommand(sentence.Command, out string error);

									if (!string.IsNullOrWhiteSpace(error))
										block.Error($"Error when convert export command. {error}");
									else
										try
										{
											// Exporta los datos
											Export(provider, command, sentence, baseFileName, block);
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

		/// <summary>
		///		Exporta el resultado del comando
		/// </summary>
		private void Export(ProviderModel provider, CommandModel command, SentenceExportPartitionedCsv sentence, string baseFileName, BlockLogModel block)
		{
			using (IDataReader reader = provider.OpenReader(command, sentence.Timeout))
			{
				long records = 0;
				List<ColumnModel> headers = GetColumns(reader);
				string partitionKey = string.Empty;
				CsvWriter writer = null;

					// Lee los registros y los va grabando en particiones
					while (reader.Read())
					{
						string actualPartition = GetPartitionKey(sentence.Columns, sentence.PartitionSeparator, reader);

							// Cambia la partición
							if (!actualPartition.EqualsIgnoreCase(partitionKey))
							{
								string fileName = GetFileName(baseFileName, sentence.PartitionSeparator, actualPartition);

									// Log
									block.Info($"Opening the file: {fileName}");
									// Cierra el archivo si ya existía
									CloseFile(writer);
									// Abre un nuevo archivo
									writer = OpenFile(sentence, headers, fileName);
									// Cambia la clave de partición
									partitionKey = actualPartition;
							}
							// Añade la línea
							writer.WriteRow(GetValues(headers, reader));
							// Lanza el evento de progreso
							if (++records % sentence.BatchSize == 0)
								block.Progress("Copying", records, 0);
					}
					// Cierra el archivo si estaba abierto
					CloseFile(writer);
					// Log
					block.Info($"Exported {records:#,##0} records");
			}
		}

		/// <summary>
		///		Abre un archivo
		/// </summary>
		private CsvWriter OpenFile(SentenceExportPartitionedCsv sentence, List<ColumnModel> headers, string fileName)
		{
			CsvWriter writer = new CsvWriter(sentence.Definition);

				// Crea el directorio
				LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
				// Abre el archivo
				writer.Open(fileName);
				// Escribe las cabeceras
				writer.WriteHeaders(GetHeaders(headers, sentence.Definition.TypedHeader));
				// Devuelve el archivo
				return writer;
		}

		/// <summary>
		///		Obtiene el nombre de archivo
		/// </summary>
		private string GetFileName(string fileName, string separator, string partitionKey)
		{
			string path = System.IO.Path.GetDirectoryName(fileName);
			string result = $"{System.IO.Path.GetFileNameWithoutExtension(fileName)}{separator}{partitionKey}{System.IO.Path.GetExtension(fileName)}";

				// Agrega el directorio
				if (!string.IsNullOrWhiteSpace(path))
					result = System.IO.Path.Combine(path, result);
				// Devuelve el nombre de archivo
				return result;
		}

		/// <summary>
		///		Cierra un archivo si estaba abierto
		/// </summary>
		private void CloseFile(CsvWriter writer)
		{
			if (writer != null)
			{
				writer.Flush();
				writer.Close();
			}
		}

		/// <summary>
		///		Obtiene la clave de partición
		/// </summary>
		private string GetPartitionKey(List<string> columns, string separator, IDataReader reader)
		{
			string partitionKey = string.Empty;

				// Obtiene la clave de partición
				foreach (string column in columns)
				{
					object result = reader[column];

						if (result is DBNull)
							partitionKey = partitionKey.AddWithSeparator(string.Empty, separator, false);
						else
							partitionKey = partitionKey.AddWithSeparator(result.ToString(), separator, false);
				}
				// Devuelve la clave de partición
				return partitionKey;
		}
	}
}