namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;

/// <summary>
///		ViewModel de una lista de <see cref="ListItemFileFilterViewModel"/>
/// </summary>
public class ListFileFilterViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private List<ListItemFileFilterViewModel> _items = new();

	public ListFileFilterViewModel(Files.BaseFileViewModel viewModel)
	{
		ViewModel = viewModel;
		ClearCommand = new BauMvvm.ViewModels.BaseCommand(_ => ClearFilters());
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	public void InitViewModel()
	{
		// Limpia la lista
		Items.Clear();
		// Añade las columnas para filtrar
		foreach (ColumnModel column in GetColumns())
			Items.Add(new ListItemFileFilterViewModel(this, column));
		// Indica que se ha inicializado
		IsInitialized = true;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Obtiene las columnas
	/// </summary>
	private List<ColumnModel> GetColumns()
	{
		List<ColumnModel> columns = new();

			// Obtiene las columnas de la tabla de datos
			if (ViewModel.DataResults is not null)
				foreach (System.Data.DataColumn column in ViewModel.DataResults.Columns)
					columns.Add(new ColumnModel(column.ColumnName, GetType(column)));
			// Devuelve la colección de columnas
			return columns;

			// Obtiene el tipo de una columna
			ColumnModel.ColumnType GetType(System.Data.DataColumn column)
			{
				if (column.DataType == typeof(double) || column.DataType == typeof(float) || column.DataType == typeof(decimal))
					return ColumnModel.ColumnType.Decimal;
				else if (column.DataType == typeof(int) || column.DataType == typeof(short) || column.DataType == typeof(byte))
					return ColumnModel.ColumnType.Integer;
				else if (column.DataType == typeof(DateTime) || column.DataType == typeof(DateOnly) || column.DataType == typeof(TimeOnly))
					return ColumnModel.ColumnType.DateTime;
				else if (column.DataType == typeof(bool))
					return ColumnModel.ColumnType.Boolean;
				else
					return ColumnModel.ColumnType.String;
			}
	}

	/// <summary>
	///		Limpia el filtro
	/// </summary>
	private void ClearFilters()
	{
		foreach (ListItemFileFilterViewModel filter in Items)
			filter.Clear();
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	public bool ValidateData()
	{
		bool validate = true;

			// Valida los filtros
			foreach (ListItemFileFilterViewModel filter in Items)
				if (validate && !filter.ValidateData())
				{
					ViewModel.SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Compruebe los filtros");
					validate = false;
				}
			// Si ha llegado hasta aquí es porque todo es correcto
			return validate;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
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
	public List<ListItemFileFilterViewModel> GetFilters()
	{
		List<ListItemFileFilterViewModel> filters = new();

			// Obtiene los filtros
			foreach (ListItemFileFilterViewModel filter in Items)
				if (filter.GetSelectedCondition() != ListItemFileFilterViewModel.ConditionType.NoCondition)
					filters.Add(filter);
			// Devuelve los filtros
			return filters;
	}

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public Files.BaseFileViewModel ViewModel { get; }

	/// <summary>
	///		Elementos del filtro
	/// </summary>
	public List<ListItemFileFilterViewModel> Items
	{
		get { return _items; }
		set { CheckObject(ref _items, value); }
	}

	/// <summary>
	///		Indica si se ha inicializado
	/// </summary>
	public bool IsInitialized { get; private set; }

	/// <summary>
	///		Comando para limpiar el filtro
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand ClearCommand { get; set; }
}