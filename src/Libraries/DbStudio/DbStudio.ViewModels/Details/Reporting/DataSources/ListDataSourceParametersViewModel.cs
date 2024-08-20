using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataSources;

/// <summary>
///		Lista de <see cref="ListItemDataSourceParameterViewModel"/>
/// </summary>
public class ListDataSourceParametersViewModel : BaseObservableObject 
{
	// Variables privadas
	private ObservableCollection<ListItemDataSourceParameterViewModel> _items = default!;
	private ListItemDataSourceParameterViewModel? _selectedItem;

	public ListDataSourceParametersViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataSourceSqlModel dataSource, bool updatable)
	{
		// Asigna las propiedades
		ReportingSolutionViewModel = reportingSolutionViewModel;
		DataSource = dataSource;
		Updatable = updatable;
		// Inicializa el viewModel
		InitViewModel();
		// Asigna los comandos
		NewParameterCommand = new BaseCommand(_ => CreateParameter());
		DeleteParameterCommand = new BaseCommand(_ => DeleteParameter(), _ => SelectedItem != null)
									.AddListener(this, nameof(SelectedItem));
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Crea los elementos de la lista
		Items = new ObservableCollection<ListItemDataSourceParameterViewModel>();
		// Añade los parámetros
		foreach (DataSourceSqlParameterModel parameter in DataSource.Parameters)
			Items.Add(new ListItemDataSourceParameterViewModel(ReportingSolutionViewModel, parameter, Updatable));
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	internal bool ValidateData()
	{
		// Comprueba las columnas
		foreach (ListItemDataSourceParameterViewModel parameter in Items)
			if (!parameter.ValidataData())
				return false;
		// Si ha llegado hasta aquí es porque todo ha ido bien
		return true;
	}

	/// <summary>
	///		Obtiene los parámetros
	/// </summary>
	internal List<DataSourceSqlParameterModel> GetParameters()
	{
		List<DataSourceSqlParameterModel> parameters = new();

			// Añade las columnas
			foreach (ListItemDataSourceParameterViewModel parameter in Items)
				parameters.Add(parameter.GetParameter());
			// Devuelve las columnas
			return parameters;
	}

	/// <summary>
	///		Crea un nuevo parámetro
	/// </summary>
	private void CreateParameter()
	{
		if (Updatable)
			Items.Add(new ListItemDataSourceParameterViewModel(ReportingSolutionViewModel, new DataSourceSqlParameterModel(), Updatable));
	}

	/// <summary>
	///		Borra un parámetro
	/// </summary>
	private void DeleteParameter()
	{
		if (Updatable && SelectedItem != null && 
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion("¿Realmente desea borrar este parámetro?"))
			Items.Remove(SelectedItem);
	}

	/// <summary>
	///		Solución
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Origen de datos
	/// </summary>
	public DataSourceSqlModel DataSource { get; }

	/// <summary>
	///		Indica si se pueden modificar los elementos
	/// </summary>
	public bool Updatable { get; }

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<ListItemDataSourceParameterViewModel> Items
	{
		get { return _items; }
		set { CheckObject(ref _items, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public ListItemDataSourceParameterViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Comando para crear un nuevo parámetro
	/// </summary>
	public BaseCommand NewParameterCommand { get; }

	/// <summary>
	///		Comando para borrar un parámetro
	/// </summary>
	public BaseCommand DeleteParameterCommand { get; }
}