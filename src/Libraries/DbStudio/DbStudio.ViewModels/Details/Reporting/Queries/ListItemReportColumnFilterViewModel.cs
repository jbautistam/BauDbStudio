using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;

/// <summary>
///		ViewModel de un <see cref="FilterRequestModel"/>
/// </summary>
public class ListItemReportColumnFilterViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas
	private ComboViewModel _comboConditions = default!;
	private object? _value, _value2;
	public bool _isNumber, _isBool, _isDateTime, _isString;

	public ListItemReportColumnFilterViewModel(ListReportColumnFilterViewModel listViewModel, FilterRequestModel filter) : base("Filter", filter)
	{
		// Inicializa las propiedades
		ListViewModel = listViewModel;
		Filter = filter;
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna el tipo de la columna
		if (ListViewModel.NodeColumnViewModel.Column is null)
			IsString = true;
		else
		{
			// Cambia el tipo de columna
			switch (ListViewModel.NodeColumnViewModel.Column.Type)
			{
				case DataSourceColumnModel.FieldType.Boolean:
						IsBool = true;
					break;
				case DataSourceColumnModel.FieldType.Date:
						IsDateTime = true;
					break;
				case DataSourceColumnModel.FieldType.Decimal:
				case DataSourceColumnModel.FieldType.Integer:
						IsNumber = true;
					break;
				default:
						IsString = true;
					break;
			}
		}
		// Carga el combo de condiciones
		LoadComboConditions();
		ComboConditions.SelectedId = (int) Filter.Condition;
		// Carga los valores
		if (Filter.Values.Count > 0)
			Value1 = Filter.Values[0];
		if (Filter.Values.Count > 1)
			Value2 = Filter.Values[1];
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Carga el combo de tipos de condición
	/// </summary>
	private void LoadComboConditions()
	{
		ComboConditions = new ComboViewModel(this);
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.Undefined, "<Seleccione una condición>");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.Equals, "Igual");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.Greater, "Mayor");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.GreaterOrEqual, "Mayor o igual");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.Less, "Menor");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.LessOrEqual, "Menor o igual");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.In, "En estos valores");
		ComboConditions.AddItem((int) FilterRequestModel.ConditionType.Between, "Entre estos valores");
		if (ListViewModel.NodeColumnViewModel.Column is not null && 
				ListViewModel.NodeColumnViewModel.Column.Type == DataSourceColumnModel.FieldType.String)
			ComboConditions.AddItem((int) FilterRequestModel.ConditionType.Contains, "Contiene");
		ComboConditions.SelectedId = (int) FilterRequestModel.ConditionType.Undefined;
	}

	/// <summary>
	///		Obtiene la condición seleccionada
	/// </summary>
	internal FilterRequestModel.ConditionType GetSelectedCondition() => (FilterRequestModel.ConditionType) (ComboConditions.SelectedId ?? 0);

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public ListReportColumnFilterViewModel ListViewModel { get; }

	/// <summary>
	///		Filtro
	/// </summary>
	public FilterRequestModel Filter { get; }

	/// <summary>
	///		Indica si la columna es de tipo numérico
	/// </summary>
	public bool IsNumber
	{ 
		get { return _isNumber; }
		set { CheckProperty(ref _isNumber, value); }
	}

	/// <summary>
	///		Indica si la columna es de tipo lógico
	/// </summary>
	public bool IsBool
	{ 
		get { return _isBool; }
		set { CheckProperty(ref _isBool, value); }
	}

	/// <summary>
	///		Indica si la columna es de tipo fecha
	/// </summary>
	public bool IsDateTime
	{ 
		get { return _isDateTime; }
		set { CheckProperty(ref _isDateTime, value); }
	}

	/// <summary>
	///		Indica si la columna es de tipo cadena
	/// </summary>
	public bool IsString
	{ 
		get { return _isString; }
		set { CheckProperty(ref _isString, value); }
	}

	/// <summary>
	///		Combo con las condiciones
	/// </summary>
	public ComboViewModel ComboConditions
	{
		get { return _comboConditions; }
		set { CheckObject(ref _comboConditions, value); }
	}

	/// <summary>
	///		Primer valor
	/// </summary>
	public object? Value1
	{
		get { return _value; }
		set { CheckObject(ref _value, value); }
	}

	/// <summary>
	///		Segundo valor
	/// </summary>
	public object? Value2
	{
		get { return _value2; }
		set { CheckObject(ref _value2, value); }
	}
}