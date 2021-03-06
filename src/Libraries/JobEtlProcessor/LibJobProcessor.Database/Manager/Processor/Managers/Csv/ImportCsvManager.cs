﻿using System;

using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.Csv
{
	/// <summary>
	///		Procesador de las sentencias de importación de archivos CSV
	/// </summary>
	internal class ImportCsvManager : BaseManager
	{
		internal ImportCsvManager(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una importación de un archivo
		/// </summary>
		internal bool Execute(SentenceImportCsv sentence)
		{
			bool imported = false;

				// Recoge los datos necesarios e importa el archivo
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start import CSV {sentence.FileName}"))
				{
					ProviderModel provider = Processor.GetProvider(sentence.Target);

						// Ejecuta el comando
						if (provider == null)
							block.Error($"Can't find the provider. Key: '{sentence.Target}'");
						else
						{
							string fileName = Processor.Manager.Step.Project.GetFullFileName(sentence.FileName);

								if (!System.IO.File.Exists(fileName))
									block.Error($"Cant find the file '{fileName}'");
								else
									try
									{
										// Importa el archivo
										Import(provider, sentence, fileName, block);
										// Indica que se ha ejecutado correctamente
										imported = true;
									}
									catch (Exception exception)
									{
										block.Error($"Error when import CSV '{fileName}'", exception);
									}
						}
				}
				// Devuelve el valor que indica si se ha importado
				return imported;
		}

		/// <summary>
		///		Importa los datos del archivo sobre el proveedor
		/// </summary>
		private void Import(ProviderModel provider, SentenceImportCsv sentence, string fileName, BlockLogModel block)
		{
			long records = 0;

				// Copia del origen al destino
				using (CsvReader reader = new CsvReader(fileName, sentence.Definition, sentence.Columns, sentence.BatchSize))
				{
					// Asigna el manejador de eventos
					reader.ReadBlock += (sender, args) => block.Progress("Importing", args.Records, 0);
					// Copia los datos del archivo sobre la tabla
					records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);
				}
				// Log
				block.Info($"Imported {records:#,##0} records");
		}
	}
}
