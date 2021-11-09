using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Storage;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibParquetFiles.Readers;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Implementation
{
	/// <summary>
	///		Implementación de la importación / exportación de archivos parquet
	/// </summary>
	internal class ParquetFileImplementation : BaseFileImplementation
	{
		/// <summary>
		///		Importa un archivo
		/// </summary>
		internal override void Import(BlockLogModel block, ProviderModel provider, Sentences.Files.SentenceFileImport sentence, System.IO.Stream stream)
		{
			long records = 0;

				// Copia del origen al destino
				using (ParquetDataReader reader = new ParquetDataReader(sentence.BatchSize))
				{
					// Asigna el manejador de eventos
					reader.Progress += (sender, args) => block.Progress("Importing", args.Records, 0);
					// Abre el archivo de entrada
					reader.Open(stream);
					// Copia los datos del archivo sobre la tabla
					records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);
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
				long records = 0;

					// Escribe en el archivo
					records = GetDataWriter(sentence.BatchSize, block).Write(stream, reader);
					// Log
					block.Info($"Exported {records:#,##0} records");
			}
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

		/// <summary>
		///		Obtiene el generador de archivos parquet
		/// </summary>
		private ParquetWriter GetDataWriter(int recordsPerBlock, BlockLogModel block)
		{
			ParquetWriter writer = new ParquetWriter(recordsPerBlock);

				// Asigna el manejador de eventos
				writer.Progress += (sender, args) => block.Progress("Writing to file", args.Records, 0);
				// Devuelve el generador
				return writer;
		}
	}
}
