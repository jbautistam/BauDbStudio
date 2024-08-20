using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibCsvFiles.Controllers;
using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;

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
	protected override async Task SaveFileAsync(ILogger logger, string target, CancellationToken cancellationToken)
	{
		if (!target.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) && !target.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			SolutionViewModel.MainController.SystemController.ShowMessage($"Can't convert CSV to {target}");
		else
		{
			Exporters.CsvFileExporter exporter = new(logger, FileName, FileParameters, target, RecordsPerBlock, GetFilters());

				// Encola el proceso
				await SolutionViewModel.MainController.MainWindowController.EnqueueProcessAsync(exporter, cancellationToken);
		}
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