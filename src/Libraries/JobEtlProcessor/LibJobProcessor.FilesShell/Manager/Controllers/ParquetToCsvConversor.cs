using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Manager.Controllers
{
	/// <summary>
	///		Conversor de archivos parquet a csv
	/// </summary>
	internal class ParquetToCsvConversor
	{
		/// <summary>
		///		Convierte un archivo parquet a csv
		/// </summary>
		internal async Task<bool> ConvertAsync(BlockLogModel block, string source, string target, CancellationToken cancellationToken)
		{
			bool converted = false;

				// Convierte el archivo
				try
				{
					LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

						// Evita el error de await
						await Task.Delay(1);
						// Crea el directorio de salida
						LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(target));
						// Escribe el archivo
						using (LibParquetFiles.Readers.ParquetDataReader reader = new LibParquetFiles.Readers.ParquetDataReader())
						{
							// Abre el archivo de entrada
							reader.Open(source);
							// Escribe en el archivo de salida
							writer.Save(reader, target);
						}
						// Indica que se ha convertido
						converted = true;
				}
				catch (Exception exception)
				{
					block.Error($"Error when convert '{source}' to '{target}'", exception);
				}
				// Devuelve el valor que indica si se ha convertido
				return converted;
		}
	}
}
