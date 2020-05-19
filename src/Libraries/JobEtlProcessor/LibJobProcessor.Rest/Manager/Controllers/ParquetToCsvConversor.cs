using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bau.Libraries.LibJobProcessor.Rest.Manager.Controllers
{
	/// <summary>
	///		Conversor de archivos parquet a csv
	/// </summary>
	internal class ParquetToCsvConversor
	{
		/// <summary>
		///		Convierte un archivo parquet a csv
		/// </summary>
		internal async Task ConvertAsync(string source, string target, CancellationToken cancellationToken)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

				// Evita el error de await
				await Task.Delay(1);
				// Crea el directorio de salida
				LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(target));
				// Escribe el archivo
				using (LibParquetFiles.ParquetDataReader reader = new LibParquetFiles.ParquetDataReader(source))
				{
					writer.Save(reader, target);
				}
		}
	}
}
