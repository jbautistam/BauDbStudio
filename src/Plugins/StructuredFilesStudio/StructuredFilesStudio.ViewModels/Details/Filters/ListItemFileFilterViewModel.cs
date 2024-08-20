using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;

/// <summary>
///		ViewModel de un elemento de la lista de filtros
/// </summary>
public class ListItemFileFilterViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Enumerados públicos
	public enum ConditionType
	{
		/// <summary>Sin condición</summary>
		NoCondition,
		/// <summary>Igual a un valor</summary>
		Equals,
		/// <summary>Distinto a un valor</summary>
		Distinct,
		/// <summary>Mayor que un valor</summary>
		Greater,
		/// <summary>Mayor o igual que un valor</summary>
		GreaterOrEqual,
		/// <summary>Menor que un valor</summary>
		Less,
		/// <summary>Menor o igual que un valor</summary>
		LessOrEqual,
		/// <summary>En una lista de valores</summary>
		In,
		/// <summary>Entre dos valores</summary>
		Between,
		/// <summary>Contiene un valor (cadenas)</summary>
		Contains
	}

	// Variables privadas
	private ComboViewModel _comboConditions = default!;
	private object? _value, _value2;
	public bool _isDecimal, _isInteger, _isBool, _isDateTime, _isString;

	public ListItemFileFilterViewModel(ListFileFilterViewModel listViewModel, ColumnModel column) : base(column.Name, column)
	{
		// Inicializa las propiedades
		ListViewModel = listViewModel;
		Column = column;
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Cambia el tipo de columna
		switch (Column.Type)
		{
			case ColumnModel.ColumnType.Boolean:
					IsBool = true;
				break;
			case ColumnModel.ColumnType.DateTime:
					IsDateTime = true;
				break;
			case ColumnModel.ColumnType.Decimal:
					IsDecimal = true;
				break;
			case ColumnModel.ColumnType.Integer:
					IsInteger = true;
				break;
			default:
					IsString = true;
				break;
		}
		// Carga el combo de condiciones
		LoadComboConditions();
		ComboConditions.SelectedId = (int) ConditionType.NoCondition;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Carga el combo de tipos de condición
	/// </summary>
	private void LoadComboConditions()
	{
		// Rellena el combo con las condiciones
		ComboConditions = new ComboViewModel(this);
		ComboConditions.AddItem((int) ConditionType.NoCondition, "<Seleccione una condición>");
		ComboConditions.AddItem((int) ConditionType.Equals, "Igual");
		ComboConditions.AddItem((int) ConditionType.Distinct, "Distinto");
		ComboConditions.AddItem((int) ConditionType.Greater, "Mayor");
		ComboConditions.AddItem((int) ConditionType.GreaterOrEqual, "Mayor o igual");
		ComboConditions.AddItem((int) ConditionType.Less, "Menor");
		ComboConditions.AddItem((int) ConditionType.LessOrEqual, "Menor o igual");
		ComboConditions.AddItem((int) ConditionType.In, "En estos valores");
		ComboConditions.AddItem((int) ConditionType.Between, "Entre estos valores");
		if (Column.Type == ColumnModel.ColumnType.String)
			ComboConditions.AddItem((int) ConditionType.Contains, "Contiene");
		// SAelecciona el elemento "sin condiciones"
		ComboConditions.SelectedId = (int) ConditionType.NoCondition;
	}

	/// <summary>
	///		Limpia el filtro
	/// </summary>
	internal void Clear()
	{
		ComboConditions.SelectedId = (int) ConditionType.NoCondition;
		Value1 = null;
		Value2 = null;
	}

	/// <summary>
	///		Obtiene la condición seleccionada
	/// </summary>
	internal ConditionType GetSelectedCondition() => (ConditionType) (ComboConditions.SelectedId ?? 0);

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	public bool ValidateData()
	{
		if (GetSelectedCondition() == ConditionType.Contains && string.IsNullOrWhiteSpace(Value1?.ToString()))
			return false;
		else 
			return true;
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public ListFileFilterViewModel ListViewModel { get; }

	/// <summary>
	///		Columna
	/// </summary>
	public ColumnModel Column { get; }

	/// <summary>
	///		Indica si la columna es de tipo decimal
	/// </summary>
	public bool IsDecimal
	{ 
		get { return _isDecimal; }
		set { CheckProperty(ref _isDecimal, value); }
	}

	/// <summary>
	///		Indica si la columna es de tipo entero
	/// </summary>
	public bool IsInteger
	{ 
		get { return _isInteger; }
		set { CheckProperty(ref _isInteger, value); }
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