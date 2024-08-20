using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

/// <summary>
///		Datos de campo para importar desde un archivo
/// </summary>
public class ImportDatabaseFieldViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _column = default!, _type = default!;
	private bool _isChecked;
	private ComboViewModel _comboFields = default!;
	private SynchronizationContext _contextUI = SynchronizationContext.Current ?? new SynchronizationContext();

	public ImportDatabaseFieldViewModel(ImportDatabaseViewModel importDatabaseViewModel, ConnectionTableFieldModel field)
	{
		ImportDatabaseViewModel = importDatabaseViewModel;
		Field = field;
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		Column = Field.Name;
		Type = Field.TypeText;
		Checked = true;
	}

	/// <summary>
	///		Carga el combo de campos
	/// </summary>
	internal void LoadComboFields(List<string> fileFields)
	{
		object state = new object();

			// Carga los campos en el combo
			_contextUI.Send(_ => {
									// Limpia el combo
									ComboFields = new ComboViewModel(this);
									// Añade un elemento vacío
									ComboFields.AddItem(null, "<Seleccione un campo>", null);
									// Añade los elementos al combo
									foreach (string field in fileFields)
									{
										// Añade el elemento
										ComboFields.AddItem(null, field, field);
										// Selecciona el campo si el nombre es igual al de la columna
										if (Column.Equals(field, StringComparison.CurrentCultureIgnoreCase))
											ComboFields.SelectedIndex = ComboFields.Items.Count - 1;
									}
									// Selecciona el primer elemento
									if (ComboFields.SelectedTag is null)
										ComboFields.SelectedIndex = 0;
								}, 
						  state);
	}

	/// <summary>
	///		Obtiene el nombre del campo
	/// </summary>
	internal string? GetFileField()
	{
		if (ComboFields.SelectedTag is not null)
			return ComboFields.SelectedTag as string;
		else
			return null;
	}

	/// <summary>
	///		ViewModel padre
	/// </summary>
	public ImportDatabaseViewModel ImportDatabaseViewModel { get; }

	/// <summary>
	///		Datos del campo
	/// </summary>
	public ConnectionTableFieldModel Field { get; }

	/// <summary>
	///		Indica si se ha seleccionado el elemento
	/// </summary>
	public bool Checked
	{
		get { return _isChecked; }
		set { CheckProperty(ref _isChecked, value); }
	}

	/// <summary>
	///		Nombre del campo en la tabla
	/// </summary>
	public string Column
	{
		get { return _column; }
		set { CheckProperty(ref _column, value); }
	}

	/// <summary>
	///		Tipo del campo
	/// </summary>
	public string Type
	{
		get { return _type; }
		set { CheckProperty(ref _type, value); }
	}

	/// <summary>
	///		Combo de campos del archivo
	/// </summary>
	public ComboViewModel ComboFields
	{
		get { return _comboFields; }
		set { CheckObject(ref _comboFields, value); }
	}
}