using Bau.Libraries.LibReporting.Solution;
using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting;

/// <summary>
///		ViewModel para una solución de informes
/// </summary>
public class ReportingSolutionViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _fileName = string.Empty;
	private Explorers.TreeReportingViewModel _treeReportingViewModel = default!;

	public ReportingSolutionViewModel(DbStudioViewModel solutionViewModel)
	{
		SolutionViewModel = solutionViewModel;
		ReportingSolutionManager = new ReportingSolutionManager();
		TreeReportingViewModel = new Explorers.TreeReportingViewModel(this);
	}

	/// <summary>
	///		Carga la solución
	/// </summary>
	public void Load(string fileName)
	{
		// Guarda el nombre de archivo
		_fileName = fileName;
		// Carga la solución
		ReportingSolutionManager.LoadSolution(_fileName);
		// Carga el árbol del explorador
		TreeReportingViewModel.Load();
	}

	/// <summary>
	///		Graba la solución
	/// </summary>
	internal void SaveSolution()
	{
		if (!string.IsNullOrWhiteSpace(_fileName))
			ReportingSolutionManager.SaveSolution(_fileName);
	}

	/// <summary>
	///		Graba un origen de datos
	/// </summary>
	internal void SaveDataWarehouse(DataWarehouseModel dataWarehouse)
	{
		// Graba el archivo
		ReportingSolutionManager.SaveDataWarehouse(dataWarehouse);
		// Actualiza el árbol
		TreeReportingViewModel.Load();
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Manager de la solución de reporting
	/// </summary>
	public ReportingSolutionManager ReportingSolutionManager { get; }

	/// <summary>
	///		ViewModel del árbol de informes
	/// </summary>
	public Explorers.TreeReportingViewModel TreeReportingViewModel
	{
		get { return _treeReportingViewModel; }
		set { CheckObject(ref _treeReportingViewModel, value); }
	}
}
