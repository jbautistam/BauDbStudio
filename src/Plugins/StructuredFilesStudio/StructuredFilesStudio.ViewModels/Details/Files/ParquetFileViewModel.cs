using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibParquetFiles.Readers;
using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;
using Bau.Libraries.LibCsvFiles.Controllers;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel para visualización de archivos parquet
/// </summary>
public class ParquetFileViewModel : BaseFileViewModel
{
	public ParquetFileViewModel(StructuredFilesStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName) {}

	/// <summary>
	///		Carga la página del archivo
	/// </summary>
	protected override async Task<(DataTable table, long totalRecords)> LoadFileAsync(bool countRecords, CancellationToken cancellationToken)
	{
		return await new ParquetDataTableReader().LoadAsync(FileName, ActualPage, RecordsPerPage, countRecords, GetFilters(), cancellationToken);
	}

	/// <summary>
	///		Obtiene los filtros
	/// </summary>
	private ParquetFiltersCollection GetFilters()
	{
		ParquetFiltersCollection filters = new();

			// Convierte los filtros
			foreach (ListItemFileFilterViewModel filter in Filters.GetFilters())
				filters.Add(filter.Column.Name, ConvertCondition(filter.GetSelectedCondition()), filter.Value1, filter.Value2);
			// Devuelve los filtros
			return filters;

			// Convierte la condición
			ParquetFilter.ConditionType ConvertCondition(ListItemFileFilterViewModel.ConditionType conditionType)
			{
				return conditionType switch
					{
						ListItemFileFilterViewModel.ConditionType.Equals => ParquetFilter.ConditionType.Equals,
						ListItemFileFilterViewModel.ConditionType.Distinct => ParquetFilter.ConditionType.Distinct,
						ListItemFileFilterViewModel.ConditionType.Greater => ParquetFilter.ConditionType.Greater,
						ListItemFileFilterViewModel.ConditionType.GreaterOrEqual => ParquetFilter.ConditionType.GreaterOrEqual,
						ListItemFileFilterViewModel.ConditionType.Less => ParquetFilter.ConditionType.Less,
						ListItemFileFilterViewModel.ConditionType.LessOrEqual => ParquetFilter.ConditionType.LessOrEqual,
						ListItemFileFilterViewModel.ConditionType.In => ParquetFilter.ConditionType.In,
						ListItemFileFilterViewModel.ConditionType.Between => ParquetFilter.ConditionType.Between,
						ListItemFileFilterViewModel.ConditionType.Contains => ParquetFilter.ConditionType.Contains,
						_ => ParquetFilter.ConditionType.NoCondition
					};
			}
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	protected override async Task SaveFileAsync(ILogger logger, string fileName, CancellationToken cancellationToken)
	{
		if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
			await SaveFileToCsvAsync(logger, fileName, cancellationToken);
		else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			await SaveFileToParquetAsync(logger, fileName, cancellationToken);
		else
			SolutionViewModel.MainController.SystemController.ShowMessage($"Can't convert CSV to {fileName}");
	}

	/// <summary>
	///		Graba el archivo a CSV
	/// </summary>
	private async Task SaveFileToCsvAsync(ILogger logger, string target, CancellationToken cancellationToken)
	{
		// Escribe el archivo
		using (CsvDataTableWriter writer = new CsvDataTableWriter())
		{
			ParquetDataTableReader reader = new ParquetDataTableReader();
			int actualPage = 1;
			(DataTable table, long totalRecords) = await reader.LoadAsync(FileName, actualPage, RecordsPerBlock, true, GetFilters(), cancellationToken);
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
						(table, _) = await reader.LoadAsync(FileName, ++actualPage, RecordsPerBlock, false, GetFilters(), cancellationToken);
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
			ParquetDataTableReader reader = new ParquetDataTableReader();
			int actualPage = 1;
			(DataTable table, long totalRecords) = await reader.LoadAsync(FileName, actualPage, RecordsPerBlock, true, GetFilters(), cancellationToken);
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
						(table, _) = await reader.LoadAsync(FileName, ++actualPage, RecordsPerBlock, false, GetFilters(), cancellationToken);
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
		await Task.Delay(1, cancellationToken);
		SolutionViewModel.MainController.OpenDialog(new ParquetFilePropertiesViewModel(SolutionViewModel, this));
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public override void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}
}