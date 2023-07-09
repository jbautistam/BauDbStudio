using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataSources;

/// <summary>
///		ViewModel de mantenimiento de un <see cref="DataSourceSqlModel"/>
/// </summary>
public class DataSourceSqlViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _key = string.Empty, _sql = string.Empty, _header = string.Empty;
	private ListDataSourceColumnsViewModel _columns = default!;
	private ListDataSourceParametersViewModel _parameters = default!;

	public DataSourceSqlViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataSourceSqlModel dataSource)
	{
		// Inicializa los objetos
		ReportingSolutionViewModel = reportingSolutionViewModel;
		DataSource = dataSource;
		// Inicializa las propiedades
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		Header = DataSource.Id;
		Key = DataSource.Id;
		Sql = DataSource.Sql;
		// Carga las columnas
		ColumnsViewModel = new ListDataSourceColumnsViewModel(ReportingSolutionViewModel, DataSource, true);
		ParametersViewModel = new ListDataSourceParametersViewModel(ReportingSolutionViewModel, DataSource, true);
		// Indica que por ahora no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Obtiene el mensaje de grabación
	/// </summary>
	public string GetSaveAndCloseMessage() => $"¿Desea grabar las modificaciones del origen de datos '{Key}'?";

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(Key))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca la clave del origen de datos");
			else if (string.IsNullOrWhiteSpace(Sql))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el comando SQL del origen de datos");
			else if (ColumnsViewModel.Items.Count == 0)
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("No se ha definido ninguna columna");
			else if (ColumnsViewModel.ValidateData() && ParametersViewModel.ValidateData())
				validated = true;
			// Devuelve el valor que indica si se ha podido grabar
			return validated;
	}

	/// <summary>
	///		Graba la dimensión
	/// </summary>
	public void SaveDetails(bool newName)
	{
		if (ValidateData())
		{
			// Añade el origen de datos si es nuevo
			if (DataSource.DataWarehouse.DataSources[DataSource.Id] == null)
				DataSource.DataWarehouse.DataSources.Add(DataSource);
			// Asigna las propiedades
			DataSource.Id = Key;
			DataSource.Sql = Sql;
			// Asigna las columnas
			DataSource.Columns.Clear();
			DataSource.Columns.AddRange(ColumnsViewModel.GetColumns());
			// Asigna los parámetros
			DataSource.Parameters.Clear();
			DataSource.Parameters.AddRange(ParametersViewModel.GetParameters());
			// Graba la solución
			ReportingSolutionViewModel.SaveDataWarehouse(DataSource.DataWarehouse);
			// Cambia la cabecera
			Header = DataSource.Id;
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
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
	///		Origen de datos
	/// </summary>
	public DataSourceSqlModel DataSource { get; }

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
	public string TabId => $"{GetType().ToString()}_{DataSource.Id}";

	/// <summary>
	///		Clave
	/// </summary>
	public string Key
	{
		get { return _key; }
		set { CheckProperty(ref _key, value); }
	}

	/// <summary>
	///		Sql
	/// </summary>
	public string Sql
	{
		get { return _sql; }
		set { CheckProperty(ref _sql, value); }
	}

	/// <summary>
	///		Columnas
	/// </summary>
	public ListDataSourceColumnsViewModel ColumnsViewModel
	{
		get { return _columns; }
		set { CheckProperty(ref _columns, value); }
	}

	/// <summary>
	///		Parámetros
	/// </summary>
	public ListDataSourceParametersViewModel ParametersViewModel
	{
		get { return _parameters; }
		set { CheckProperty(ref _parameters, value); }
	}
}