using System;

using Bau.Libraries.LibParquetFiles;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parquet;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.ParquetFiles
{
	/// <summary>
	///		Controlador para importar datos de un archivo parquet a una tabla SQL
	/// </summary>
	internal class ImportParquetController : BaseManager
	{
		internal ImportParquetController(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una importación de un archivo parquet a un proveedores
		/// </summary>
		internal bool Execute(SentenceImportParquet sentence)
		{
			string fileName = Processor.Manager.Step.Project.GetFullFileName(sentence.FileName);
			bool imported = false;

				// Importa el archivo
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, 
																						  $"Start importing '{fileName}'"))
				{
					if (!System.IO.File.Exists(fileName))
						block.Error($"Can't find the file '{fileName}'");
					else
					{
						ProviderModel provider = Processor.GetProvider(sentence.Target);

							if (provider == null)
								block.Error($"Can't find the provider. Key: '{sentence.Target}'");
							else
								try
								{
									// Importa los datos
									Import(block, provider, sentence, fileName);
									// Indica que se ha importado correctamente
									imported = true;
								}
								catch (Exception exception)
								{
									block.Error($"Error when import '{fileName}'", exception);
								}
					}
				}
				// Devuelve el valor que indica si se ha importado el archivo
				return imported;
		}

		/// <summary>
		///		Importa el archivo
		/// </summary>
		private void Import(BlockLogModel block, ProviderModel provider, SentenceImportParquet sentence, string fileName)
		{
			long records = 0;

				// Copia del origen al destino
				using (ParquetDataReader reader = new ParquetDataReader(fileName, sentence.RecordsPerBlock))
				{
					// Asigna el manejador de eventos
					reader.Progress += (sender, args) => block.Progress("Importing", args.Records, 0);
					// Copia los datos del archivo sobre la tabla
					records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.RecordsPerBlock, sentence.Timeout);
				}
				// Log
				block.Info($"Imported {records:#,##0} records");
		}
	}
}