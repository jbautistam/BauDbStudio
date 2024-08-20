using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataSources;

/// <summary>
///		ViewModel para un <see cref="DataSourceColumnModel"/>
/// </summary>
public class ListItemDataSourceColumnViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _columnId = string.Empty, _alias = string.Empty, _type = string.Empty, _formula = string.Empty;
	private bool _isPrimaryKey, _visible, _required, _isUpdatable;
	private ComboViewModel _comboTypes = default!;

	public ListItemDataSourceColumnViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataSourceColumnModel column, bool isUpdatable)
	{
		// Asigna las propiedades
		ReportingSolutionViewModel = reportingSolutionViewModel;
		Column = column;
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
		ColumnId = Column.Id;
		Alias = Column.Alias;
		Formula = Column.FormulaSql;
		Type = Column.Type.ToString();
		IsPrimaryKey = Column.IsPrimaryKey;
		Required = Column.Required;
		Visible = Column.Visible;
		// Selecciona el tipo de columna en el combo
		ComboTypes.SelectedId = (int) Column.Type;
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
			if (string.IsNullOrWhiteSpace(ColumnId))
				ReportingSolutionViewModel.SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la columna");
			else if (GetSelectedType() == DataSourceColumnModel.FieldType.Unknown)
				ReportingSolutionViewModel.SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione el tipo de la columna");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Obtiene la columna
	/// </summary>
	internal DataSourceColumnModel GetColumn()
	{
		return new DataSourceColumnModel(Column.DataSource)
						{
							Id = ColumnId,
							Alias = Alias,
							FormulaSql = Formula,
							Type = GetSelectedType(),
							IsPrimaryKey = IsPrimaryKey,
							Required = Required,
							Visible = Visible
						};
	}

	/// <summary>
	///		Obtiene el tipo seleccionado en el combo (o el de la columna si no es modificable)
	/// </summary>
	private DataSourceColumnModel.FieldType GetSelectedType()
	{
		if (!Updatable)
			return Column.Type;
		else
			return (DataSourceColumnModel.FieldType) (ComboTypes.SelectedId ?? (int) DataSourceColumnModel.FieldType.Unknown);
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Datos de la columna
	/// </summary>
	public DataSourceColumnModel Column { get; }

	/// <summary>
	///		Código de columna
	/// </summary>
	public string ColumnId
	{
		get { return _columnId; }
		set { CheckProperty(ref _columnId, value); }
	}

	/// <summary>
	///		Alias de la columna
	/// </summary>
	public string Alias
	{
		get { return _alias; }
		set { CheckProperty(ref _alias, value); }
	}

	/// <summary>
	///		Fórmula asociada a la columna
	/// </summary>
	public string Formula
	{
		get { return _formula; }
		set { CheckProperty(ref _formula, value); }
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
	///		Indica si el campo es clave primario
	/// </summary>
	public bool IsPrimaryKey
	{
		get { return _isPrimaryKey; }
		set { CheckProperty(ref _isPrimaryKey, value); }
	}

	/// <summary>
	///		Indica si el campo es visible
	/// </summary>
	public bool Visible
	{
		get { return _visible; }
		set { CheckProperty(ref _visible, value); }
	}

	/// <summary>
	///		Indica si el campo es obligatorio
	/// </summary>
	public bool Required
	{
		get { return _required; }
		set { CheckProperty(ref _required, value); }
	}

	/// <summary>
	///		Indica si se pueden modificar los datos de la columna
	/// </summary>
	public bool Updatable
	{
		get { return _isUpdatable; }
		set { CheckProperty(ref _isUpdatable, value); }
	}

	/// <summary>
	///		Combo de tipos de columnas
	/// </summary>
	public ComboViewModel ComboTypes
	{
		get { return _comboTypes; }
		set { CheckObject(ref _comboTypes, value); }
	}
}
