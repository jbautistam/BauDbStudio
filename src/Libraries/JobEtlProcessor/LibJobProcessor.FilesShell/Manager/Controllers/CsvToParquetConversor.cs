using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Manager.Controllers
{
	/// <summary>
	///		Conversor de archivos CSV a parquet
	/// </summary>
	internal class CsvToParquetConversor
	{
		/// <summary>
		///		Convierte un archivo csv a parquet
		/// </summary>
		internal async Task<bool> ConvertAsync(BlockLogModel block, string source, string target, CancellationToken cancellationToken)
		{
			bool converted = false;

				// Convierte el archivo
				try
				{
					LibParquetFiles.Writers.ParquetWriter writer = new LibParquetFiles.Writers.ParquetWriter(target);

						// Evita el error de await
						await Task.Delay(1);
						// Crea el directorio de salida
						LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(target));
						// Escribe el archivo
						using (LibCsvFiles.CsvReader reader = new LibCsvFiles.CsvReader(source, null, null))
						{
							writer.Write(reader);
						}
						// Indica que se ha convertido el archivo
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
