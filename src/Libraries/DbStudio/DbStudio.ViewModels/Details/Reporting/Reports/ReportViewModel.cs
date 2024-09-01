using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Reports;

/// <summary>
///		ViewModel de mantenimiento de un <see cref="ReportModel"/>
/// </summary>
public class ReportViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _header = string.Empty, _key = string.Empty, _description = string.Empty;
	private ObservableCollection<ReportDataSourceViewModel> _dataSources = default!;
	private ReportDataSourceViewModel? _selectedDataSource;
	private bool _isNew;

	public ReportViewModel(ReportingSolutionViewModel reportingSolutionViewModel, ReportModel report, bool isNew)
	{
		// Inicializa los objetos
		ReportingSolutionViewModel = reportingSolutionViewModel;
		Report = report;
		// Inicializa las variables
		_isNew = isNew;
		// Inicializa las propiedades
		InitViewModel();
		// Asigna los comandos
		NewDataSourceCommand = new BaseCommand(_ => CreateDataSource());
		DeleteDataSourceCommand = new BaseCommand(_ => DeleteDataSource(), _ => SelectedDataSource is not null)
											.AddListener(this, nameof(SelectedDataSource));
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades básicas
		if (_isNew)
			Key = "NewReport";
		else
			Key = Report.Id;
		Description = Report.Description;
		Header = Key;
		// Carga la lista de orígenes de datos
		LoadDataSources();
		// Indica que por ahora no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Carga los orígenes de datos
	/// </summary>
	private void LoadDataSources()
	{
		// Inicializa la lista
		DataSources = new ObservableCollection<ReportDataSourceViewModel>();
		// Carga los orígenes de datos
		foreach (ReportDataSourceModel dataSource in Report.DataSources)
			DataSources.Add(new ReportDataSourceViewModel(ReportingSolutionViewModel, Report, dataSource));
	}

	/// <summary>
	///		Crea un origen de datos
	/// </summary>
	private void CreateDataSource()
	{
		throw new NotImplementedException("¿De dónde debería recogee el datasource?");
		DataSources.Add(new ReportDataSourceViewModel(ReportingSolutionViewModel, Report, null));
	}

	/// <summary>
	///		Borra un origen de datos
	/// </summary>
	private void DeleteDataSource()
	{
		if (SelectedDataSource != null && 
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion("¿Desea quitar este origen de datos del informe?"))
			DataSources.Remove(SelectedDataSource);
	}

	/// <summary>
	///		Obtiene el mensaje de grabación
	/// </summary>
	public string GetSaveAndCloseMessage() => $"¿Desea grabar las modificaciones del informe '{Key}'?";

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(Key))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca la clave del informe");
			else if (ValidateDataSources())
				validated = true;
			// Devuelve el valor que indica si se ha podido grabar
			return validated;
	}

	/// <summary>
	///		Comprueba los orígenes de datos
	/// </summary>
	private bool ValidateDataSources()
	{
		bool validated = false;

			// Comprueba los datos
			if (DataSources.Count == 0)
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un origen de datos");
			else
			{
				// Supone que los datos son correctos
				validated = true;
				// Comprueba que todos los orígenes de datos tengan algo en el combo
				foreach (ReportDataSourceViewModel dataSource in DataSources)
					if (!dataSource.ValidateData())
						validated = false;
			}
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba el informe
	/// </summary>
	public void SaveDetails(bool newName)
	{
		if (ValidateData())
		{
			// Asigna las propiedades a la dimensión
			Report.Id = Key;
			Report.Description = Description;
			// Si es nuevo se añade a la colección
			if (_isNew)
			{
				// Añade el informe
				Report.DataWarehouse.Reports.Add(Report);
				// Indica que ya no es nuevo
				_isNew = false;
			}
			// Añade los orígenes de datos
			Report.DataSources.Clear();
			Report.DataSources.AddRange(GetDataSources());
			// Graba la solución
			ReportingSolutionViewModel.SaveDataWarehouse(Report.DataWarehouse);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Obtiene los orígenes de datos seleccionados
	/// </summary>
	private List<ReportDataSourceModel> GetDataSources()
	{
		List<ReportDataSourceModel> dataSources = new List<ReportDataSourceModel>();

			// Añade los orígenes de datos de la lista
			foreach (ReportDataSourceViewModel dataSource in DataSources)
				dataSources.Add(dataSource.GetReportDataSource());
			// Devuelve los orígenes de datos
			return dataSources;
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Informe
	/// </summary>
	public ReportModel Report { get; }

	/// <summary>
	///		Cabecera de la ventana
	/// </summary>
	public string Header
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Id de la ficha en la aplicación
	/// </summary>
	public string TabId => $"{GetType().ToString()}_{Report.Id}";

	/// <summary>
	///		Clave de la dimensión
	/// </summary>
	public string Key
	{
		get { return _key; }
		set 
		{ 
			if (CheckProperty(ref _key, value))
				Header = value;
		}
	}

	/// <summary>
	///		Descripción
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Orígenes de datos asociados al informe
	/// </summary>
	public ObservableCollection<ReportDataSourceViewModel> DataSources
	{
		get { return _dataSources; }
		set { CheckObject(ref _dataSources, value); }
	}

	/// <summary>
	///		Orgien de datos seleccionado en la lista
	/// </summary>
	public ReportDataSourceViewModel? SelectedDataSource
	{
		get { return _selectedDataSource; }
		set { CheckObject(ref _selectedDataSource, value); }
	}

	/// <summary>
	///		Nuevo origen de datos
	/// </summary>
	public BaseCommand NewDataSourceCommand { get; }

	/// <summary>
	///		Borrar origen de datos
	/// </summary>
	public BaseCommand DeleteDataSourceCommand { get; }
}