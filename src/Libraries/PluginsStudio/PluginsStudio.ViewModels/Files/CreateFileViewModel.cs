using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

namespace Bau.Libraries.PluginsStudio.ViewModels.Files;

/// <summary>
///		ViewModel para la creación de archivos
/// </summary>
public class CreateFileViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	/// <summary>
	///		Codificación del archivo
	/// </summary>
	public enum Encoding
	{
		Utf8,
		Utf8NoBom,
		Unicode,
		Utf32,
		Ascii
	}
	// Constantes privadas
	private const string LastFileCreatedConfigurationKey = "LastFileCreated";
	// Variables privadas
	private string _fileName = string.Empty;
	private ComboViewModel _comboTypes = default!, _comboEncoding = default!;

	public CreateFileViewModel(PluginsStudioViewModel mainViewModel, string path, List<FileAssignedModel> filesAssigned)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		Folder = path;
		FilesAssigned = filesAssigned;
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		string? lastFileCreated = MainViewModel.MainController.PluginsController.ConfigurationController.GetConfiguration(PluginsStudioViewModel.PluginName, 
																														  LastFileCreatedConfigurationKey);

			// Carga los combos
			LoadComboTypes(lastFileCreated);
			LoadComboEncoding();
	}

	/// <summary>
	///		Carga el combo de tipos de archivos
	/// </summary>
	private void LoadComboTypes(string? lastFile)
	{
		string? extension = Path.GetExtension(lastFile);

			// Crea el combo
			ComboTypes = new ComboViewModel(this);
			// Añade los elementos
			foreach (FileAssignedModel fileAssigned in FilesAssigned)
			{
				// Añade el tipo al combo
				ComboTypes.AddItem(ComboTypes.Items.Count + 1, $"{fileAssigned.Name} ({fileAssigned.FileExtension})", fileAssigned);
				// Selecciona el elemento si coindice con la extensión del último archivo guardado
				if (!string.IsNullOrWhiteSpace(extension) && 
						(extension.Equals(fileAssigned.FileExtension, StringComparison.CurrentCultureIgnoreCase) ||
						 extension.Equals($".{fileAssigned.FileExtension}", StringComparison.CurrentCultureIgnoreCase)))
					ComboTypes.SelectedIndex = ComboTypes.Items.Count - 1;
			}
			// Asigna el manejador de eventos
			ComboTypes.PropertyChanged += (sender, args) =>
												{
													if ((args.PropertyName ?? string.Empty).Equals(nameof(ComboTypes.SelectedItem)))
														UpdateFileExtension();
												};
			// Selecciona el primer elemento
			if (ComboTypes.SelectedId is null)
				ComboTypes.SelectedItem = ComboTypes.Items[0];
			else
				UpdateFileExtension();
	}

	/// <summary>
	///		Carga el combo de tipo codificación
	/// </summary>
	private void LoadComboEncoding()
	{
		// Crea el combo
		ComboEncoding = new ComboViewModel(this);
		// Añade los elementos
		ComboEncoding.AddItem((int) Encoding.Utf8, "Utf 8");
		ComboEncoding.AddItem((int) Encoding.Utf8NoBom, "Utf 8 (sin BOM)");
		ComboEncoding.AddItem((int) Encoding.Utf32, "Utf 32");
		ComboEncoding.AddItem((int) Encoding.Unicode, "Unicode");
		ComboEncoding.AddItem((int) Encoding.Ascii, "Ascii");
		// Selecciona el primer elemento
		ComboEncoding.SelectedItem = ComboEncoding.Items[0];
	}

	/// <summary>
	///		Selecciona una codificación en el combo
	/// </summary>
	public void SelectEncoding(int encodingIndex)
	{
		if (encodingIndex >= 0 && encodingIndex < ComboEncoding.Items.Count)
			ComboEncoding.SelectedItem = ComboEncoding.Items[encodingIndex];
	}

	/// <summary>
	///		Modifica la extensión del archivo
	/// </summary>
	private void UpdateFileExtension()
	{
		string file = FileName;

			// Cambia la extensión
			if (ComboTypes.SelectedTag is not null && ComboTypes.SelectedTag is FileAssignedModel fileAssigned)
			{
				// Inicializa el nombre de archivo
				if (string.IsNullOrWhiteSpace(file))
					file = $"New file";
				// Cambia la extensión
				file = GetFileNameWithoutExtension(file);
				file += GetExtension(fileAssigned.FileExtension);
			}
			// Cambia el nombre de archivo
			FileName = file;
	}

	/// <summary>
	///		Obtiene el nombre de archivo sin la extensión (no se puede utilizar System.IO.Path.GetFileNameWithoutExtension porque
	///	en ocasiones tiene más de una extensión)
	/// </summary>
	private string GetFileNameWithoutExtension(string file) => file.Cut(".", out string _);

	/// <summary>
	///		Obtiene la extensión asociada a un tipo de archivo
	/// </summary>
	private string GetExtension(string extension)
	{
		// Añade el punto a la extensión
		if (!string.IsNullOrWhiteSpace(extension) && !extension.StartsWith("."))
			extension = "." + extension;
		// Devuelve la extensión
		return extension;
	}

	/// <summary>
	///		Obtiene la codificación seleccionada en el combo
	/// </summary>
	public Encoding GetSelectedEncoding() => (Encoding) (ComboEncoding.SelectedId ?? 0);

	/// <summary>
	///		Comprueba los datos introducidos en el formulario
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(FileName))
				MainViewModel.MainController.MainWindowController.SystemController.ShowMessage("Introduzca el nombre de archivo");
			else if (File.Exists(FullFileName))
				MainViewModel.MainController.MainWindowController.SystemController.ShowMessage("Ya existe este nombre de archivo en el directorio");
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
		if (ValidateData())
		{
			// Graba el archivo creado en la configuración
			MainViewModel.MainController.PluginsController.ConfigurationController.SetConfiguration(PluginsStudioViewModel.PluginName, LastFileCreatedConfigurationKey,
																									FileName);
			MainViewModel.MainController.PluginsController.ConfigurationController.Save();
			// Lanza el evento de cierre de la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Directorio
	/// </summary>
	public string Folder { get; }

	/// <summary>
	///		Archivos asignados por los plugins
	/// </summary>
	public List<FileAssignedModel> FilesAssigned { get; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Nombre completo del archivo
	/// </summary>
	public string FullFileName
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(FileName))
				return Path.Combine(Folder, FileName);
			else
				return Path.Combine(Folder, "New file.sql");
		}
	}

	/// <summary>
	///		Tipos de archivo
	/// </summary>
	public ComboViewModel ComboTypes
	{
		get { return _comboTypes; }
		set { CheckObject(ref _comboTypes!, value); }
	}

	/// <summary>
	///		Combo de codificación
	/// </summary>
	public ComboViewModel ComboEncoding
	{
		get { return _comboEncoding; }
		set { CheckObject(ref _comboEncoding!, value); }
	}
}
