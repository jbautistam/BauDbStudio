using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibParquetFiles.Readers;
using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;

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
	protected override async Task SaveFileAsync(ILogger logger, string target, CancellationToken cancellationToken)
	{
		if (!target.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) && !target.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			SolutionViewModel.MainController.SystemController.ShowMessage($"Can't convert parquet to {target}");
		else
		{
			Exporters.ParquetFileExporter exporter = new(logger, FileName, target, RecordsPerBlock, GetFilters());

				// Encola el proceso
				await SolutionViewModel.MainController.MainWindowController.EnqueueProcessAsync(exporter, cancellationToken);
		}
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