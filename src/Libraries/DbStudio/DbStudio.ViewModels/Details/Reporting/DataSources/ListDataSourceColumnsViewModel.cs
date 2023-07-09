using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataSources;

/// <summary>
///		Lista de <see cref="ListItemDataSourceColumnViewModel"/>
/// </summary>
public class ListDataSourceColumnsViewModel : BaseObservableObject 
{
	// Variables privadas
	private ObservableCollection<ListItemDataSourceColumnViewModel> _items = default!;
	private ListItemDataSourceColumnViewModel? _selectedItem;

	public ListDataSourceColumnsViewModel(ReportingSolutionViewModel reportingSolutionViewModel, BaseDataSourceModel dataSource, bool updatable)
	{
		// Asigna las propiedades
		ReportingSolutionViewModel = reportingSolutionViewModel;
		DataSource = dataSource;
		Updatable = updatable;
		// Inicializa el viewModel
		InitViewModel();
		// Asigna los comandos
		NewColumnCommand = new BaseCommand(_ => CreateColumn());
		DeleteColumnCommand = new BaseCommand(_ => DeleteColumn(), _ => SelectedItem != null)
									.AddListener(this, nameof(SelectedItem));
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Crea los elementos de la lista
		Items = new ObservableCollection<ListItemDataSourceColumnViewModel>();
		// Añade las columnas
		foreach (DataSourceColumnModel column in DataSource.Columns.EnumerateValuesSorted())
		{
			ListItemDataSourceColumnViewModel item = new ListItemDataSourceColumnViewModel(ReportingSolutionViewModel, column, Updatable);

				// Añade el manejador de eventos
				item.PropertyChanged += (sender, args) => IsUpdated = true;
				// Añade el elemento
				Items.Add(item);
		}
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	internal bool ValidateData()
	{
		// Comprueba las columnas
		foreach (ListItemDataSourceColumnViewModel column in Items)
			if (!column.ValidataData())
				return false;
		// Si ha llegado hasta aquí es porque todo ha ido bien
		return true;
	}

	/// <summary>
	///		Obtiene las columnas
	/// </summary>
	internal LibReporting.Models.Base.BaseReportingDictionaryModel<DataSourceColumnModel> GetColumns()
	{
		LibReporting.Models.Base.BaseReportingDictionaryModel<DataSourceColumnModel> columns = new();

			// Añade las columnas
			foreach (ListItemDataSourceColumnViewModel column in Items)
				columns.Add(column.GetColumn());
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		Crea una nueva columna
	/// </summary>
	private void CreateColumn()
	{
		if (Updatable)
			Items.Add(new ListItemDataSourceColumnViewModel(ReportingSolutionViewModel, new DataSourceColumnModel(DataSource), Updatable));
	}

	/// <summary>
	///		Borra una columna
	/// </summary>
	private void DeleteColumn()
	{
		if (Updatable && SelectedItem != null && 
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion("¿Realmente desea borrar esta columna?"))
			Items.Remove(SelectedItem);
	}

	/// <summary>
	///		Solución
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Origen de datos
	/// </summary>
	public BaseDataSourceModel DataSource { get; }

	/// <summary>
	///		Indica si se pueden modificar los elementos
	/// </summary>
	public bool Updatable { get; }

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<ListItemDataSourceColumnViewModel> Items
	{
		get { return _items; }
		set { CheckObject(ref _items, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public ListItemDataSourceColumnViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Comando para crear una nueva columna
	/// </summary>
	public BaseCommand NewColumnCommand { get; }

	/// <summary>
	///		Comando para borrar una columna
	/// </summary>
	public BaseCommand DeleteColumnCommand { get; }
}
