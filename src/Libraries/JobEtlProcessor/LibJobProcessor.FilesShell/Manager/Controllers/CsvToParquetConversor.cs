using System;
using System.Threading;
using System.Threading.Tasks;

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
		internal async Task ConvertAsync(string source, string target, CancellationToken cancellationToken)
		{
			LibParquetFiles.ParquetDataWriter writer = new LibParquetFiles.ParquetDataWriter(target);

				// Evita el error de await
				await Task.Delay(1);
				// Crea el directorio de salida
				LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(target));
				// Escribe el archivo
				using (LibCsvFiles.CsvReader reader = new LibCsvFiles.CsvReader(source, null, null))
				{
					writer.Write(reader);
				}
		}
	}
}
