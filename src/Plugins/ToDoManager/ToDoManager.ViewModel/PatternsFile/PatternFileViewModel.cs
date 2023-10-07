using Microsoft.Extensions.Logging;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Files;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;

namespace Bau.Libraries.ToDoManager.ViewModel.PatternsFile;

/// <summary>
///		ViewModel para ver el contenido de un archivo de tareas
/// </summary>
public class PatternFileViewModel : BaseFileViewModel, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _source = string.Empty, _separator = string.Empty, _quoteChar = string.Empty;
	private string _formula = string.Empty, _extensionHighlight = string.Empty;
	private bool _withHeader;
	private ComboViewModel? _comboSeparators, _comboExtensions;

	public PatternFileViewModel(ToDoManagerViewModel mainViewModel, string fileName) : base(fileName)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		// Crea el combo
		ComboSeparators = new ComboViewModel(this);
		ComboExtensions = new ComboViewModel(this);
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	public override void Load()
	{
		try
		{
			LibPatternText.Models.PatternModel pattern = new LibPatternText.PatternManager().Load(FileName);

				// Carga los datos
				Source = pattern.Source;
				WithHeader = pattern.WithHeader;
				Separator = pattern.Separator;
				QuoteChar = pattern.QuoteChar;
				Formula = pattern.Formula;
				ExtensionHighlight = NormalizeExtension(pattern.ExtensionHighlight);
				// Carga los combos
				LoadComboSeparators(Separator);
				LoadComboExtensions(ExtensionHighlight);
				// Indica que no ha habido cambios
				IsUpdated = false;
		}
		catch (Exception exception)
		{
			MainViewModel.ViewsController.Logger.LogError(exception, $"Error when load {FileName}. {exception.Message}");
			MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load {FileName}. {exception.Message}");
		}
	}

	/// <summary>
	///		Normaliza la extensión
	/// </summary>
	private string NormalizeExtension(string extension)
	{
		if (string.IsNullOrWhiteSpace(extension))
			return string.Empty;
		else if (!extension.StartsWith("."))
			return "." + extension.Trim();
		else
			return extension.Trim();
	}

	/// <summary>
	///		Carga el combo de separadores
	/// </summary>
	private void LoadComboSeparators(string separator)
	{
		// Limpia el combo
		ComboSeparators.Items.Clear();
		// Añade el separador si no está en la lista de posibles
		if (!string.IsNullOrEmpty(separator) && separator != "," && separator != ";" && separator != "\t" && separator != " ")
			ComboSeparators.AddItem(0, separator);
		// Añade los separadores básicos
		ComboSeparators.AddItem(1, ",");
		ComboSeparators.AddItem(2, ";");
		ComboSeparators.AddItem(3, "Tab");
		ComboSeparators.AddItem(4, "Space");
		// Selecciona el separador
		if (separator == "\t")
			ComboSeparators.SelectedId = 3;
		else if (separator == " ")
			ComboSeparators.SelectedId = 4;
		else
			ComboSeparators.SelectedText = separator;
	}

	/// <summary>
	///		Carga el combo de extensiones
	/// </summary>
	private void LoadComboExtensions(string extensionHighligth)
	{
		List<string> extensions = new() { ".sql", ".cs", ".py", ".md", ".txt", ".htm" };
		bool added = false;

			// Normaliza la extensión
			if (string.IsNullOrWhiteSpace(extensionHighligth))
				extensionHighligth = extensions[0];
			// Limpia el combo
			ComboExtensions.Items.Clear();
			// Añade las extensiones predeterminadas
			foreach (string extension in extensions)
			{
				// Añade la extensión
				ComboExtensions.AddItem(ComboExtensions.Items.Count, extension);
				// Comprueba si la extensión es una de las seleccionadas
				if (extension.Equals(extensionHighligth, StringComparison.CurrentCultureIgnoreCase))
					added = true;
			}
			// Añade la extensión seleccionada si no está
			if (!added)
				ComboExtensions.AddItem(ComboExtensions.Items.Count, extensionHighligth);
			// Selecciona la extensión
			ComboExtensions.SelectedText = extensionHighligth;
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public override void SaveDetails(bool newName)
	{
		// Graba el archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string? newFileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave
									(string.Empty, 
									 $"Archivo patrones (*{ToDoManagerViewModel.PatternFileExtension})|*{ToDoManagerViewModel.PatternFileExtension}|Todos los archivos (*.*)|*.*",
									 FileName, ToDoManagerViewModel.PatternFileExtension);

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Graba el archivo
			new LibPatternText.PatternManager().Save(FileName, GetPattern());
			// Actualiza el árbol
			MainViewModel.ViewsController.HostPluginsController.RefreshFiles();
			// Añade el archivo a los últimos archivos abiertos
			MainViewModel.ViewsController.HostPluginsController.AddFileUsed(FileName);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Obtiene los datos del patrón
	/// </summary>
	private LibPatternText.Models.PatternModel GetPattern()
	{
		return new LibPatternText.Models.PatternModel()
							{
								Source = Source,
								WithHeader = WithHeader,
								Separator = GetSeparator(Separator),
								QuoteChar = QuoteChar,
								Formula = Formula,
								ExtensionHighlight = ExtensionHighlight
							};

			// Convierte al separador adecuado
			string GetSeparator(string separator)
			{
				if (string.IsNullOrWhiteSpace(separator))
					return ",";
				else if (separator.Equals("Tab", StringComparison.CurrentCultureIgnoreCase))
					return "\t";
				else if (separator.Equals("Space", StringComparison.CurrentCultureIgnoreCase))
					return " ";
				else
					return separator;
			}
	}

	/// <summary>
	///		Ejecuta el patrón
	/// </summary>
	public string Execute()
	{
		// Ejecuta la fórmula
		try
		{
			return new LibPatternText.PatternManager().Convert(GetPattern());
		}
		catch (Exception exception)
		{
			MainViewModel.ViewsController.Logger.LogError(exception, $"Error when execute {FileName}. {exception.Message}");
			MainViewModel.ViewsController.SystemController.ShowMessage($"Error when execute {FileName}. {exception.Message}");
		}
		// Devuelve el resultado
		return string.Empty;
	}

	/// <summary>
	///		Cierra la ventana
	/// </summary>
	public override void Close()
	{
		// ... No hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Origen de los datos
	/// </summary>
	public string Source
	{
		get { return _source; }
		set { CheckProperty(ref _source, value); }
	}

	/// <summary>
	///		Indica si el origen de los datos tiene cabecera
	/// </summary>
	public bool WithHeader
	{
		get { return _withHeader; }
		set { CheckProperty(ref _withHeader, value); }
	}

	/// <summary>
	///		Separador de columnas
	/// </summary>
	public string Separator
	{
		get { return _separator; }
		set { CheckProperty(ref _separator, value); }
	}

	/// <summary>
	///		Carácter utilizado como comillas
	/// </summary>
	public string QuoteChar
	{
		get { return _quoteChar; }
		set { CheckProperty(ref _quoteChar, value); }
	}

	/// <summary>
	///		Fórmula
	/// </summary>
	public string Formula
	{
		get { return _formula; }
		set { CheckProperty(ref _formula, value); }
	}

	/// <summary>
	///		Separadores predeterminados
	/// </summary>
	public ComboViewModel ComboSeparators
	{
		get { return _comboSeparators ?? default!; }
		set { CheckObject(ref _comboSeparators, value); }
	}

	/// <summary>
	///		Extensiones de archivos
	/// </summary>
	public ComboViewModel ComboExtensions
	{
		get { return _comboExtensions ?? default!; }
		set { CheckObject(ref _comboExtensions, value); }
	}

	/// <summary>
	///		Extensión seleccionada
	/// </summary>
	public string ExtensionHighlight
	{
		get { return _extensionHighlight; }
		set { CheckProperty(ref _extensionHighlight, NormalizeExtension(value)); }
	}
}
