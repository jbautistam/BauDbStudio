using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibCsvFiles.Controllers;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files.Exporters;

/// <summary>
///		Clase de proceso para la exportación de archivos CSV
/// </summary>
internal class CsvFileExporter : ProcessModel
{
	internal CsvFileExporter(ILogger logger, string fileNameSource, FileModel fileParameters, string fileNameTarget, int recordsPerBlock, CsvFiltersCollection filters)
					: base("Export", $"Export CSV file {fileNameSource}")
	{
		Logger = logger;
		FileNameSource = fileNameSource;
		FileParameters = fileParameters;
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
			SaveFileToCsv(cancellationToken);
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
	private void SaveFileToCsv(CancellationToken cancellationToken)
	{
		// Escribe el archivo
		using (CsvDataTableWriter writer = new CsvDataTableWriter(FileParameters))
		{
			CsvDataTableReader reader = new CsvDataTableReader(FileParameters);
			int actualPage = 1;
			DataTable table = reader.Load(FileNameSource, actualPage, RecordsPerBlock, true, Filters, out long totalRecords);
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
						table = reader.Load(FileNameSource, ++actualPage, RecordsPerBlock, false, Filters, out _);
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
			CsvDataTableReader reader = new CsvDataTableReader();
			int actualPage = 1;
			DataTable table = reader.Load(FileNameSource, actualPage, RecordsPerBlock, true, Filters, out long totalRecords);
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
						table = reader.Load(FileNameSource, ++actualPage, RecordsPerBlock, false, Filters, out _);
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
	///		Parámetros del archivo
	/// </summary>
	public FileModel FileParameters { get; } = new FileModel();

	/// <summary>
	///		Filtros
	/// </summary>
	internal CsvFiltersCollection Filters { get; }
}
