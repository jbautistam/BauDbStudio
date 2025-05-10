namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Files;

/// <summary>
///		ViewModel para un archivo de texto
/// </summary>
public abstract class BaseTextFileViewModel : BaseFileViewModel
{
	// Eventos públicos
	public event EventHandler<Controllers.EventArguments.EditorGoToLineEventArgs>? GoToLineRequired;
	public event EventHandler<Controllers.EventArguments.EditorSelectedTextRequiredEventArgs>? SelectedTextRequired;
	// Variables privadas
	private string _content = string.Empty;
	private System.Text.Encoding? _fileEncoding;
	private bool _fileWithBom;

	public BaseTextFileViewModel(Controllers.IPluginsController pluginsController, string fileName, string mask) : base(pluginsController, fileName, mask) 
	{
	}

	/// <summary>
	///		Carga el texto del archivo
	/// </summary>
	public override void Load()
	{
		// Carga el archivo
		if (!string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName))
		{
			// Obtiene la codificación del archivo (para grabarlo después con la misma codificación)
			FileEncoding = LibHelper.Files.HelperFiles.GetFileEncoding(FileName);
			if (FileEncoding is null)
			{
				FileEncoding = System.Text.Encoding.UTF8;
				FileWithBom = false;
			}
			else
				FileWithBom = true;
			// Carga el archivo
			Content = LibHelper.Files.HelperFiles.LoadTextFile(FileName);
		}
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public override void SaveDetails(bool newName)
	{
		string oldTabId = TabId;

			// Graba el archivo
			if (string.IsNullOrWhiteSpace(FileName) || newName)
			{
				string? newFileName = PluginsController.MainWindowController.DialogsController.OpenDialogSave(FileName, "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*",
																											  FileName, ".sql");

					// Cambia el nombre de archivo si es necesario
					if (!string.IsNullOrWhiteSpace(newFileName))
						FileName = newFileName;
			}
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				// Graba el archivo
				if (FileWithBom)
					LibHelper.Files.HelperFiles.SaveTextFile(FileName, Content, FileEncoding ?? System.Text.Encoding.UTF8);
				else
					LibHelper.Files.HelperFiles.SaveTextFileWithoutBom(FileName, Content);
				// Actualiza el nombre de archivo
				UpdateFileName(oldTabId);
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			}
	}

	/// <summary>
	///		Lanza un evento para solicitar el texto seleccionado al editor
	/// </summary>
	protected string? GetEditorSelectedText()
	{
		Controllers.EventArguments.EditorSelectedTextRequiredEventArgs eventArgs = new(string.Empty);

			// Lanza el evento
			SelectedTextRequired?.Invoke(this, eventArgs);
			// Recupera el texto
			return eventArgs.SelectedText;
	}

	/// <summary>
	///		Lanza un evento para colocar el editor en una línea
	/// </summary>
	public void GoToLine(string textSelected, int line)
	{
		GoToLineRequired?.Invoke(this, new Controllers.EventArguments.EditorGoToLineEventArgs(textSelected, line));
	}

	/// <summary>
	///		Trata el texto que se arrastra sobre el editor desde un árbol
	/// </summary>
	public virtual async Task<string> TreatTextDroppedAsync(string content, bool shiftPressed, CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
		return content;
	}

	/// <summary>
	///		Cierra la ventana
	/// </summary>
	public override void Close()
	{
		Content = string.Empty;
	}

	/// <summary>
	///		Codificación del archivo
	/// </summary>
	public System.Text.Encoding? FileEncoding
	{
		get { return _fileEncoding; }
		set { CheckObject(ref _fileEncoding, value); }
	}

	/// <summary>
	///		Indica si el archivo tiene una cabecera BOM
	/// </summary>
	public bool FileWithBom
	{
		get { return _fileWithBom; }
		set { CheckProperty(ref _fileWithBom, value); }
	}

	/// <summary>
	///		Contenido del archivo
	/// </summary>
	public string Content
	{
		get { return _content; }
		set { CheckProperty(ref _content, value); }
	}
}
