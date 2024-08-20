using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibParquetFiles.Readers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;
using Bau.Libraries.LibParquetFiles.Writers;
using Bau.Libraries.LibCsvFiles.Controllers;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files.Exporters;

/// <summary>
///		Clase de proceso para la exportación de archivos parquet
/// </summary>
internal class ParquetFileExporter : ProcessModel
{
	internal ParquetFileExporter(ILogger logger, string fileNameSource, string fileNameTarget, int recordsPerBlock, ParquetFiltersCollection filters)
					: base("Export", $"Export parquet file {fileNameSource}")
	{
		Logger = logger;
		FileNameSource = fileNameSource;
		FileNameTarget = fileNameTarget;
		RecordsPerBlock = recordsPerBlock;
		Filters = filters;
	}

	/// <summary>
	///		Ejecuta la importación
	/// </summary>
	public async override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// Log de progreso
		RaiseLog(LogEventArgs.Status.Info, $"Exporting {Path.GetFileName(FileNameSource)} to {Path.GetFileName(FileNameTarget)}");
		// Ejecuta la exportación
		if (FileNameTarget.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
			await SaveFileToCsvAsync(cancellationToken);
		else if (FileNameTarget.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			await SaveFileToParquetAsync(cancellationToken);
		// Log de progreso
		RaiseLog(LogEventArgs.Status.Success, $"End export to {Path.GetFileName(FileNameTarget)}");
		// Log
		Logger.LogInformation($"Fin de la grabación del archivo '{FileNameTarget}'");
	}

	/// <summary>
	///		Graba el archivo a CSV
	/// </summary>
	private async Task SaveFileToCsvAsync(CancellationToken cancellationToken)
	{
		// Escribe el archivo
		using (CsvDataTableWriter writer = new CsvDataTableWriter())
		{
			ParquetDataTableReader reader = new ParquetDataTableReader();
			int actualPage = 1;
			(DataTable table, long totalRecords) = await reader.LoadAsync(FileNameSource, actualPage, RecordsPerBlock, true, Filters, cancellationToken);
			long records = 0;

				// Abre el archivo
				writer.Open(FileNameTarget);
				// Escribe los registros
				do
				{
					// Graba el archivo
					writer.Save(table);
					// Añade el número de registros
					records += table.Rows.Count;
					// Lee la siguiente página
					if (records < totalRecords)
						(table, _) = await reader.LoadAsync(FileNameSource, ++actualPage, RecordsPerBlock, false, Filters, cancellationToken);
					// Log
					RaiseProgress(records, totalRecords, $"Save '{Path.GetFileName(FileNameTarget)}'");
				}
				while (records < totalRecords && !cancellationToken.IsCancellationRequested);
		}
	}

	/// <summary>
	///		Graba el archivo a Parquet
	/// </summary>
	private async Task SaveFileToParquetAsync(CancellationToken cancellationToken)
	{
		// Escribe el archivo
		await using (ParquetDataTableWriter writer = new ParquetDataTableWriter(RecordsPerBlock))
		{
			ParquetDataTableReader reader = new ParquetDataTableReader();
			int actualPage = 1;
			(DataTable table, long totalRecords) = await reader.LoadAsync(FileNameSource, actualPage, RecordsPerBlock, true, Filters, cancellationToken);
			long records = 0;

				// Abre el archivo
				writer.Open(FileNameTarget);
				// Escribe los registros
				do
				{
					// Graba el archivo
					await writer.WriteAsync(table, cancellationToken);
					// Añade el número de registros
					records += table.Rows.Count;
					// Lee la siguiente página
					if (records < totalRecords)
						(table, _) = await reader.LoadAsync(FileNameSource, ++actualPage, RecordsPerBlock, false, Filters, cancellationToken);
					// Log
					RaiseProgress(records, totalRecords, $"Save '{Path.GetFileName(FileNameTarget)}'");
				}
				while (records < totalRecords && !cancellationToken.IsCancellationRequested);
				// Escribe los registros finales
				await writer.FlushAsync(cancellationToken);
		}
	}

	/// <summary>
	///		Sistema de log
	/// </summary>
	internal ILogger Logger { get; }

	/// <summary>
	///		Nombre de archivo origen
	/// </summary>
	internal string FileNameSource { get; }

	/// <summary>
	///		Nombre de archivo destino
	/// </summary>
	internal string FileNameTarget { get; }

	/// <summary>
	///		Registros por bloque
	/// </summary>
	internal int RecordsPerBlock { get; }

	/// <summary>
	///		Filtros
	/// </summary>
	internal ParquetFiltersCollection Filters { get; }
}
