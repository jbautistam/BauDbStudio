using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Manager.Controllers
{
	/// <summary>
	///		Conversor de archivos Excel a parquet
	/// </summary>
	internal class ExcelToParquetConversor
	{
		/// <summary>
		///		Convierte un archivo Excel a parquet
		/// </summary>
		internal async Task<bool> ConvertAsync(BlockLogModel block, string source, string target, ExcelfileOptions options, CancellationToken cancellationToken)
		{
			bool converted = false;

				// Convierte el archivo
				try
				{
					LibParquetFiles.Writers.ParquetWriter writer = new LibParquetFiles.Writers.ParquetWriter(200_000);

						// Evita el error de await
						await Task.Delay(1);
						// Crea el directorio de salida
						LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(target));
						// Escribe el archivo
						using (System.Data.IDataReader reader = new LibExcelFiles.Data.ExcelDataTableReader().LoadFile(source, options.SheetIndex,  
																													   1, 10_000_000, options.WithHeader).CreateDataReader())
						{
							writer.Write(target, reader);
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
