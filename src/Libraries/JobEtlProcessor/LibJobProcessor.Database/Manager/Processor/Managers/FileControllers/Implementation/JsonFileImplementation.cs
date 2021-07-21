using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Storage;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJsonFiles;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Implementation
{
	/// <summary>
	///		Implementación de la importación / exportación de archivos json
	/// </summary>
	internal class JsonFileImplementation : BaseFileImplementation
	{
		/// <summary>
		///		Importa un archivo
		/// </summary>
		internal override void Import(BlockLogModel block, ProviderModel provider, Sentences.Files.SentenceFileImport sentence, System.IO.Stream stream)
		{
			long records = 0;

				// Copia del origen al destino
				using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
				{
					using (JsonReader reader = new JsonReader(sentence.BatchSize))
					{
						// Asigna el manejador de eventos
						reader.Progress += (sender, args) => block.Progress("Importing", args.Records, 0);
						// Abre el archivo de entrada
						reader.Open(streamReader);
						// Copia los datos del archivo sobre la tabla
						records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);
					}
				}
				// Log
				block.Info($"Imported {records:#,##0} records");
		}

		/// <summary>
		///		Exporta un archivo
		/// </summary>
		internal override void Export(BlockLogModel block, System.IO.Stream stream, ProviderModel provider, CommandModel command, Sentences.Files.SentenceFileExport sentence)
		{
			using (System.Data.IDataReader reader = provider.OpenReader(command, sentence.Timeout))
			{
				using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(stream))
				{
					using (JsonWriter writer = new JsonWriter())
					{
						// Abre el archivo
						writer.Open(streamWriter);
						// Graba las líneas
						while (reader.Read())
						{
							// Graba los valores
							writer.WriteRow(GetValues(reader));
							// Lanza el evento de progreso
							if (writer.Rows % sentence.BatchSize == 0)
								block.Progress($"Copying {writer.Rows:#,##0}", writer.Rows, 0);
						}
						// Log
						block.Info($"Exported {writer.Rows:#,##0} records");
					}
				}
			}
		}

		/// <summary>
		///		Obtiene el diccionario de valores a partir de un registro
		/// </summary>
		private Dictionary<string, object> GetValues(IDataReader reader)
		{
			Dictionary<string, object> values = new Dictionary<string, object>();

				// Añade los nombres de columnas por las cabeceras
				for (int index = 0; index < reader.FieldCount; index++)
					values.Add(reader.GetName(index), reader.GetValue(index));
				// Devuelve la lista de valores
				return values;
		}

		/// <summary>
		///		Exporta un archivo particionado
		/// </summary>
		internal override async Task ExportPartitionedAsync(BlockLogModel block, BaseFileStorage fileManager, ProviderModel provider, CommandModel command, 
															SentenceFileExportPartitioned sentence, string baseFileName, CancellationToken cancellationToken)
		{
			// Evita los warnings
			await Task.Delay(1);
			// Lanza una excepción
			throw new NotImplementedException();
		}
	}
}
