using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter
{
	/// <summary>
	///		Controlador para la exportación de datos
	/// </summary>
	internal class ExportDataController
	{
		// Constantes
		internal const string DefaultFileName = "New file.csv";
		internal const string MaskExportFiles = "Archivos CSV (*.csv)|*.csv|Archivos parquet (*.parquet)|*.parquet|Todos los archivos (*.*)|*.*";
		internal const string DefaultExtension = ".csv";

		/// <summary>
		///		Exporta una tabla de datos a un archivo
		/// </summary>
		internal async Task<(bool exported, string error)> ExportAsync(LibLogger.Core.LogManager logManager, string fileName, 
																	   DbDataReader reader, CancellationToken cancellationToken)
		{
			string error = string.Empty;

				// Exporta el archivo
				using (BlockLogModel block = logManager.Default.CreateBlock(LogModel.LogType.Info, $"Comienzo de grabación del archivo {fileName}"))
				{
					// Exporta el archivo
					try
					{
						if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
							await ExportToCsvAsync(block, fileName, reader, cancellationToken);
						else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
							await ExportToParquetAsync(block, fileName, reader, cancellationToken);
						else
							error = "No se reconoce la extensión del archivo";
					}
					catch (Exception exception)
					{
						error = $"Error al grabar el archivo {fileName}. {exception.Message}";
						block.Error(error, exception);
					}
					// Log
					block.Info($"Fin de grabación del archivo {fileName}");
					logManager.Flush();
				}
				// Devuelve el valor que indica si se ha exportado el archivo
				return (string.IsNullOrEmpty(error), error);
		}

		/// <summary>
		///		Exporta la tabla a CSV
		/// </summary>
		private async Task ExportToCsvAsync(BlockLogModel block, string fileName, DbDataReader reader, CancellationToken cancellationToken)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

				// Asigna el evento de progreso
				writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
				// Graba el archivo
				await writer.SaveAsync(reader, fileName, cancellationToken);
		}

		/// <summary>
		///		Exporta la tabla a parquet
		/// </summary>
		private async Task ExportToParquetAsync(BlockLogModel block, string fileName, DbDataReader reader, CancellationToken cancellationToken)
		{
			await using (LibParquetFiles.Writers.ParquetDataWriterAsync writer = new LibParquetFiles.Writers.ParquetDataWriterAsync(200_000))
			{
				// Asigna el evento de progreso
				writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
				// Graba el archivo
				await writer.WriteAsync(fileName, reader, cancellationToken);
			}
		}
	}
}
