using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel para un <see cref="DataSourceColumnModel"/>
/// </summary>
public class CsvFileListItemColumnViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _columnId = default!;
	private ComboViewModel _comboTypes = default!;

	public CsvFileListItemColumnViewModel(CsvFileListColumnsViewModel listViewModel, string column)
	{
		// Asigna las propiedades
		ListViewModel = listViewModel;
		ColumnId = column;
		// Inicializa el ViewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Inicializa el combo
		ComboTypes = new ComboViewModel(this);
		// Añade los elementos
		ComboTypes.AddItem((int) BaseFileViewModel.FieldType.Unknown, "<Seleccione un tipo>");
		ComboTypes.AddItem((int) BaseFileViewModel.FieldType.String, "Cadena");
		ComboTypes.AddItem((int) BaseFileViewModel.FieldType.Date, "Fecha / hora");
		ComboTypes.AddItem((int) BaseFileViewModel.FieldType.Integer, "Entero");
		ComboTypes.AddItem((int) BaseFileViewModel.FieldType.Decimal, "Decimal");
		ComboTypes.AddItem((int) BaseFileViewModel.FieldType.Boolean, "Lógico");
		// Selecciona el primer elemento
		ComboTypes.SelectedId = (int) BaseFileViewModel.FieldType.Unknown;
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	internal bool ValidataData()
	{
		bool validated = false;

			// Comprueba los datos
			if (GetSelectedType() == BaseFileViewModel.FieldType.Unknown)
				ListViewModel.FileViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el tipo de la columna");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Obtiene el tipo seleccionado en el combo (o el de la columna si no es modificable)
	/// </summary>
	public BaseFileViewModel.FieldType GetSelectedType()
	{
		return (BaseFileViewModel.FieldType) (ComboTypes.SelectedId ?? (int) BaseFileViewModel.FieldType.Unknown);
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public CsvFileListColumnsViewModel ListViewModel { get; }

	/// <summary>
	///		Código de columna
	/// </summary>
	public string ColumnId
	{
		get { return _columnId; }
		set { CheckProperty(ref _columnId, value); }
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
