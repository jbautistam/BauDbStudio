using System;

using Bau.Libraries.LibParquetFiles;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parquet;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.ParquetFiles
{
	/// <summary>
	///		Controlador para exportar datos de una consulta SQL a un archivo parquet
	/// </summary>
	internal class ExportParquetController : BaseManager
	{
		internal ExportParquetController(DbScriptProcessor processor) : base(processor) {}

		/// <summary>
		///		Procesa una exportación de una consulta a un archivo parquet
		/// </summary>
		internal bool Execute(SentenceExportParquet sentence)
		{
			string fileName = Processor.Manager.Step.Project.GetFullFileName(sentence.FileName);
			bool exported = false;

				// Procesa la sentencia
				using (BlockLogModel block = Processor.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, 
																						  $"Start exporting to '{fileName}'"))
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
											// Exporta al archivo
											Export(block, fileName, provider, command, sentence);
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
		///		Exporta los datos a un archivo Parquet
		/// </summary>
		private void Export(BlockLogModel block, string fileName, ProviderModel provider, CommandModel command, SentenceExportParquet sentence)
		{
			using (System.Data.IDataReader reader = provider.OpenReader(command, sentence.Timeout))
			{
				long records = 0;

					// Crea el directorio
					LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
					// Escribe en el archivo
					records = GetDataWriter(fileName, sentence.RecordsPerBlock, block).Write(reader, sentence.RowGroupSize);
					// Log
					block.Info($"Exported {records:#,##0} records");
			}
		}

		/// <summary>
		///		Obtiene el generador de archivos parquet
		/// </summary>
		private ParquetDataWriter GetDataWriter(string fileName, int recordsPerBlock, BlockLogModel block)
		{
			ParquetDataWriter writer = new ParquetDataWriter(fileName, recordsPerBlock);

				// Asigna el manejador de eventos
				writer.Progress += (sender, args) => block.Progress("Writing to file", args.Records, 0);
				// Devuelve el generador
				return writer;
		}
	}
}