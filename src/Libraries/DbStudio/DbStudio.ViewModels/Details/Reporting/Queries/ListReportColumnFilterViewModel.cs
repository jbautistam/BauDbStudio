using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;

/// <summary>
///		ViewModel de una lista de <see cref="FilterRequestModel"/>
/// </summary>
public class ListReportColumnFilterViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = string.Empty, _type = string.Empty;
	private ControlItemCollectionViewModel<ListItemReportColumnFilterViewModel> _listFiltersViewModel = default!;
	private ListItemReportColumnFilterViewModel? _selectedItem;

	public ListReportColumnFilterViewModel(ReportQueryViewModel viewModel, NodeColumnViewModel nodeColumnViewModel)
	{
		// Inicializa las propiedades
		ViewModel = viewModel;
		NodeColumnViewModel = nodeColumnViewModel;
		// Inicializa el viewModel
		InitViewModel();
		// Asigna los manejadores de eventos
		NewItemCommand = new BaseCommand(_ => NewFilter());
		DeleteItemCommand = new BaseCommand(_ => DeleteFilter());
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Limpia la lista
		FiltersViewModel = new ControlItemCollectionViewModel<ListItemReportColumnFilterViewModel>();
		// Asigna las propiedades
		if (NodeColumnViewModel.Column is not null)
		{
			Name = NodeColumnViewModel.Column.Id;
			Type = NodeColumnViewModel.Column.Type.ToString();
		}
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Añade un filtro a la lista
	/// </summary>
	private void NewFilter()
	{
		FiltersViewModel.Add(new ListItemReportColumnFilterViewModel(this, new FilterRequestModel()));
	}

	/// <summary>
	///		Limpia los filtros
	/// </summary>
	public void Clear()
	{
		FiltersViewModel.Clear();
	}

	/// <summary>
	///		Añade una serie de filtros
	/// </summary>
	public void AddRange(List<FilterRequestModel> filters)
	{
		foreach (FilterRequestModel filter in filters)
			FiltersViewModel.Add(new ListItemReportColumnFilterViewModel(this, filter));
	}

	/// <summary>
	///		Añade un filtro a la lista
	/// </summary>
	public void Add(FilterRequestModel filter)
	{
		FiltersViewModel.Add(new ListItemReportColumnFilterViewModel(this, filter));
	}

	/// <summary>
	///		Obtiene el valor del primer filtro
	/// </summary>
	public object? GetDefaultValue()
	{
		if (FiltersViewModel.Count > 0)
			return FiltersViewModel[0].Value1;
		else
			return null;
	}

	/// <summary>
	///		Elimina un filtro de la vista
	/// </summary>
	private void DeleteFilter()
	{
		if (SelectedItem != null)
			FiltersViewModel.Remove(SelectedItem);
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData(ListItemReportColumnFilterViewModel filter)
	{
		bool validated = false;
		FilterRequestModel.ConditionType condition = filter.GetSelectedCondition();

			// Comprueba los datos introducidos
			if (condition == FilterRequestModel.ConditionType.Undefined)
				ViewModel.ViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la condición");
			else if (condition == FilterRequestModel.ConditionType.Contains && string.IsNullOrWhiteSpace(filter.Value1?.ToString()))
				ViewModel.ViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el valor");
			else 
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (SaveDetails())
		{
			// Indica que ya no es nuevo y está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		Graba los filtros si no hay errores
	/// </summary>
	private bool SaveDetails()
	{
		bool canSave = true;

			// Comprueba cada uno de los filtros de la lista
			foreach (ListItemReportColumnFilterViewModel filter in FiltersViewModel)
				if (ValidateData(filter))
				{
					// Asigna los datos
					filter.Filter.Condition = filter.GetSelectedCondition();
					filter.Filter.Values.Clear();
					filter.Filter.Values.Add(filter.Value1);
					if (filter.Filter.Condition == FilterRequestModel.ConditionType.Between || filter.Filter.Condition == FilterRequestModel.ConditionType.In)
						filter.Filter.Values.Add(filter.Value2);
				}
				else
					canSave = false;
			// Devuelve el valor que indica si se puede cerrar la ventana
			return canSave;
	}

	/// <summary>
	///		Graba los datos (la vista da una excepción "DialogResult sólo se puede establecer después de haber creado Window y una vez mostrada como cuadro de diálogo.")
	/// </summary>
	public bool SaveFilters() => SaveDetails();

	/// <summary>
	///		ViewModel del informe
	/// </summary>
	public ReportQueryViewModel ViewModel { get; }

	/// <summary>
	///		ViewModel del nodo de la columna
	/// </summary>
	public NodeColumnViewModel NodeColumnViewModel { get; }

	/// <summary>
	///		Nombre de la columna
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Tipo de la columna
	/// </summary>
	public string Type
	{
		get { return _type; }
		set { CheckProperty(ref _type, value); }
	}

	/// <summary>
	///		Lista de filtros
	/// </summary>
	public ControlItemCollectionViewModel<ListItemReportColumnFilterViewModel> FiltersViewModel
	{
		get { return _listFiltersViewModel; }
		set { CheckObject(ref _listFiltersViewModel, value); }
	}

	/// <summary>
	///		Elemento seleccionado en la lista de filtros
	/// </summary>
	public ListItemReportColumnFilterViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Comando para crear un nuevo elemento
	/// </summary>
	public BaseCommand NewItemCommand { get; }

	/// <summary>
	///		Comando para borrar un elemento
	/// </summary>
	public BaseCommand DeleteItemCommand { get; }
}