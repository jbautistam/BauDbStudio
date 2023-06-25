using System.Data;

using Bau.Libraries.LibExcelFiles.Data;
using Bau.Libraries.LibParquetFiles.Writers;
using Microsoft.Extensions.Logging;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel para visualización de archivos Excel
/// </summary>
public class ExcelFileViewModel : BaseFileViewModel
{
	public ExcelFileViewModel(StructuredFilesStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName) {}

	/// <summary>
	///		Carga el archivo
	/// </summary>
	protected override async Task<(DataTable table, long totalRecords)> LoadFileAsync(bool countRecords, CancellationToken cancellationToken)
	{
		long totalRecords = 0;

			// Evita las advertencias
			await Task.Delay(1);
			// Inicializa los argumentos de salida
			if (countRecords)
				totalRecords = new ExcelDataTableReader().CountRows(FileName, 1, true);
			// Carga el archivo
			return (new ExcelDataTableReader().LoadFile(FileName, 1, (ActualPage - 1) * RecordsPerPage, RecordsPerPage, true), totalRecords);
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	protected override async Task SaveFileAsync(ILogger logger, string fileName, CancellationToken cancellationToken)
	{
		ExcelDataTableReader excelReader = new ExcelDataTableReader();
		long rows = excelReader.CountRows(FileName, 1, true);

			// Graba el archivo
			using (IDataReader reader = excelReader.LoadFile(FileName, 1, 0, rows, true).CreateDataReader())
			{
				await using (ParquetDataWriter writer = new(200_000))
				{
					// Log
					writer.Progress += (sender, args) => logger.LogInformation($"Save '{Path.GetFileName(fileName)}' {args.Records:0,##0} / {args.Records + 1:#,##0}");;
					// Escribe el archivo
					await writer.WriteAsync(fileName, reader, cancellationToken);
				}
			}
			// Log
			logger.LogInformation($"Fin de la grabación del archivo '{fileName}'");
	}

	/// <summary>
	///		Abre las propiedades del archivo
	/// </summary>
	protected override async Task OpenFilePropertiesAsync(CancellationToken cancellationToken)
	{
		// No hace nada, sólo implementa la interface
		await Task.Delay(1, cancellationToken);
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public override void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Parámetros del archivo
	/// </summary>
	public LibCsvFiles.Models.FileModel FileParameters { get; } = new LibCsvFiles.Models.FileModel();
}
