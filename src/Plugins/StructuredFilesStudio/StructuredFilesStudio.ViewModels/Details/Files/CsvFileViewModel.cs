using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibCsvFiles.Controllers;
using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel para visualización de archivos CSV
/// </summary>
public class CsvFileViewModel : BaseFileViewModel
{
	public CsvFileViewModel(StructuredFilesStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName) {}

	/// <summary>
	///		Carga el archivo
	/// </summary>
	protected override async Task<(DataTable table, long totalRecords)> LoadFileAsync(bool countRecords, CancellationToken cancellationToken)
	{
		DataTable table = new CsvDataTableReader(FileParameters)
									.Load(FileName, ActualPage, RecordsPerPage, countRecords, GetFilters(), out long totalRecords);

			// Evita las advertencias
			await Task.Delay(1);
			// Devuelve la tabla
			return (table, totalRecords);
	}

	/// <summary>
	///		Obtiene los filtros
	/// </summary>
	private CsvFiltersCollection GetFilters()
	{
		CsvFiltersCollection filters = new();

			// Convierte los filtros
			foreach (ListItemFileFilterViewModel filter in Filters.GetFilters())
				filters.Add(filter.Column.Name, ConvertCondition(filter.GetSelectedCondition()), filter.Value1, filter.Value2);
			// Devuelve los filtros
			return filters;

			// Convierte la condición
			CsvFilter.ConditionType ConvertCondition(ListItemFileFilterViewModel.ConditionType conditionType)
			{
				return conditionType switch
					{
						ListItemFileFilterViewModel.ConditionType.Equals => CsvFilter.ConditionType.Equals,
						ListItemFileFilterViewModel.ConditionType.Distinct => CsvFilter.ConditionType.Distinct,
						ListItemFileFilterViewModel.ConditionType.Greater => CsvFilter.ConditionType.Greater,
						ListItemFileFilterViewModel.ConditionType.GreaterOrEqual => CsvFilter.ConditionType.GreaterOrEqual,
						ListItemFileFilterViewModel.ConditionType.Less => CsvFilter.ConditionType.Less,
						ListItemFileFilterViewModel.ConditionType.LessOrEqual => CsvFilter.ConditionType.LessOrEqual,
						ListItemFileFilterViewModel.ConditionType.In => CsvFilter.ConditionType.In,
						ListItemFileFilterViewModel.ConditionType.Between => CsvFilter.ConditionType.Between,
						ListItemFileFilterViewModel.ConditionType.Contains => CsvFilter.ConditionType.Contains,
						_ => CsvFilter.ConditionType.NoCondition
					};
			}
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	protected override async Task SaveFileAsync(ILogger logger, string fileName, CancellationToken cancellationToken)
	{
		if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
			SaveFileToCsv(logger, fileName, cancellationToken);
		else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			await SaveFileToParquetAsync(logger, fileName, cancellationToken);
		else
			SolutionViewModel.MainController.SystemController.ShowMessage($"Can't convert CSV to {fileName}");
	}

	/// <summary>
	///		Graba el archivo a CSV
	/// </summary>
	private void SaveFileToCsv(ILogger logger, string target, CancellationToken cancellationToken)
	{
		// Escribe el archivo
		using (CsvDataTableWriter writer = new CsvDataTableWriter(FileParameters))
		{
			CsvDataTableReader reader = new CsvDataTableReader(FileParameters);
			int actualPage = 1;
			DataTable table = reader.Load(FileName, actualPage, RecordsPerBlock, true, GetFilters(), out long totalRecords);
			long records = 0;

				// Abre el archivo
				writer.Open(target);
				// Escribe los registros
				do
				{
					// Graba el archivo
					writer.Save(table);
					// Añade el número de registros
					records += table.Rows.Count;
					// Lee la siguiente página
					if (records < totalRecords)
						table = reader.Load(FileName, ++actualPage, RecordsPerBlock, false, GetFilters(), out _);
					// Log
					logger.LogInformation($"Save '{Path.GetFileName(target)}' {records:0,##0} / {totalRecords + 1:#,##0}");
				}
				while (records < totalRecords && !cancellationToken.IsCancellationRequested);
		}
		// Log
		logger.LogInformation($"Fin de la grabación del archivo '{target}'");
	}

	/// <summary>
	///		Graba el archivo a Parquet
	/// </summary>
	private async Task SaveFileToParquetAsync(ILogger logger, string target, CancellationToken cancellationToken)
	{
		// Escribe el archivo
		await using (ParquetDataTableWriter writer = new ParquetDataTableWriter(RecordsPerBlock))
		{
			CsvDataTableReader reader = new CsvDataTableReader();
			int actualPage = 1;
			DataTable table = reader.Load(FileName, actualPage, RecordsPerBlock, true, GetFilters(), out long totalRecords);
			long records = 0;

				// Abre el archivo
				writer.Open(target);
				// Escribe los registros
				do
				{
					// Graba el archivo
					await writer.WriteAsync(table, cancellationToken);
					// Añade el número de registros
					records += table.Rows.Count;
					// Lee la siguiente página
					if (records < totalRecords)
						table = reader.Load(FileName, ++actualPage, RecordsPerBlock, false, GetFilters(), out _);
					// Log
					logger.LogInformation($"Save '{Path.GetFileName(target)}' {records:0,##0} / {totalRecords + 1:#,##0}");
				}
				while (records < totalRecords && !cancellationToken.IsCancellationRequested);
				// Escribe los registros finales
				await writer.FlushAsync(cancellationToken);
		}
		// Log
		logger.LogInformation($"Fin de la grabación del archivo '{target}'");
	}

	/// <summary>
	///		Abre las propiedades del archivo
	/// </summary>
	protected override async Task OpenFilePropertiesAsync(CancellationToken cancellationToken)
	{
		if (SolutionViewModel.MainController.OpenDialog(new CsvFilePropertiesViewModel(SolutionViewModel, this)) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			await LoadFileAsync(cancellationToken);
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

	/// <summary>
	///		Columnas del archivo
	/// </summary>
	public List<LibCsvFiles.Models.ColumnModel> FileColumns { get; } = new List<LibCsvFiles.Models.ColumnModel>();
}