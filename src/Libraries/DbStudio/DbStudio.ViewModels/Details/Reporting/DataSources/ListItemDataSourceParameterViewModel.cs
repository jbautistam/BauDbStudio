using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataSources;

/// <summary>
///		ViewModel para un <see cref="DataSourceSqlParameterModel"/>
/// </summary>
public class ListItemDataSourceParameterViewModel: BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _name = string.Empty, _type = string.Empty, _defaultValue = string.Empty;
	private bool _isUpdatable;
	private ComboViewModel _comboTypes = default!;

	public ListItemDataSourceParameterViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataSourceSqlParameterModel parameter, bool isUpdatable)
	{
		// Asigna las propiedades
		ReportingSolutionViewModel = reportingSolutionViewModel;
		Parameter = parameter;
		Updatable = isUpdatable;
		// Inicializa el ViewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Carga el combo
		LoadComboTypes();
		// Asigna las propiedades
		Name = Parameter.Name;
		Type = Parameter.Type.ToString();
		// Selecciona el tipo de columna en el combo
		ComboTypes.SelectedId = (int) Parameter.Type;
	}

	/// <summary>
	///		Carga el combo de tipos
	/// </summary>
	private void LoadComboTypes()
	{
		// Inicializa el combo
		ComboTypes = new ComboViewModel(this);
		// Añade los elementos
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.Unknown, "<Seleccione un tipo>");
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.String, "Cadena");
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.Date, "Fecha / hora");
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.Integer, "Entero");
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.Decimal, "Decimal");
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.Boolean, "Lógico");
		ComboTypes.AddItem((int) DataSourceColumnModel.FieldType.Binary, "Binario");
		// Selecciona el primer elemento
		ComboTypes.SelectedId = (int) DataSourceColumnModel.FieldType.Unknown;
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	internal bool ValidataData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(Name))
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre del parámetro");
			else if (GetSelectedType() == DataSourceColumnModel.FieldType.Unknown)
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el tipo del parámetro");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Obtiene el parámetro
	/// </summary>
	internal DataSourceSqlParameterModel GetParameter()
	{
		return new DataSourceSqlParameterModel()
						{
							Name = Name,
							Type = GetSelectedType(),
							DefaultValue = DefaultValue
						};
	}

	/// <summary>
	///		Obtiene el tipo seleccionado en el combo (o el de la columna si no es modificable)
	/// </summary>
	private DataSourceColumnModel.FieldType GetSelectedType()
	{
		if (!Updatable)
			return Parameter.Type;
		else
			return (DataSourceColumnModel.FieldType) (ComboTypes.SelectedId ?? (int) DataSourceColumnModel.FieldType.Unknown);
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Datos del parámetro
	/// </summary>
	public DataSourceSqlParameterModel Parameter { get; }

	/// <summary>
	///		Nombre del parámetro
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Tipo
	/// </summary>
	public string Type
	{
		get { return _type; }
		set { CheckProperty(ref _type, value); }
	}

	/// <summary>
	///		Valor predeterminado
	/// </summary>
	public string DefaultValue
	{
		get { return _defaultValue; }
		set { CheckProperty(ref _defaultValue, value); }
	}

	/// <summary>
	///		Indica si se pueden modificar los datos del parámetro
	/// </summary>
	public bool Updatable
	{
		get { return _isUpdatable; }
		set { CheckProperty(ref _isUpdatable, value); }
	}

	/// <summary>
	///		Combo de tipo de parámetro
	/// </summary>
	public ComboViewModel ComboTypes
	{
		get { return _comboTypes; }
		set { CheckObject(ref _comboTypes, value); }
	}
}
