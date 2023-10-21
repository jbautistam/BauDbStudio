using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.FileTools.ViewModel;

/// <summary>
///		ViewModel principal del procesador de trabajos
/// </summary>
public class FileToolsViewModel : BaseObservableObject
{
	// Constantes públicas
	public const string PatternFileExtension = ".pattern";

	public FileToolsViewModel(Controllers.IFileToolsController mainController)
	{
		// Asigna el controlador de vistas
		MainController = mainController;
		// Inicializa los comandos
		ExecuteFileCommand = new BaseCommand(_ => ExecuteFolderFile(_));
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Carga el directorio
	/// </summary>
	public void Load(string path)
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Abre un archivo (no hace nada, sólo implementa la interface)
	/// </summary>
	public bool OpenFile(string fileName)
	{
		bool open = false;

			// Abre el archivo
			if (!string.IsNullOrWhiteSpace(fileName)) 
			{
				if (fileName.EndsWith(PatternFileExtension, StringComparison.CurrentCultureIgnoreCase))
				{
					// Abre el archivo
					MainController.OpenWindow(new PatternsFile.PatternFileViewModel(this, fileName));
					// indica que ha podido abrir el archivo
					open = true;
				}
			}
			// Devuelve el valor que indica si se ha abierto
			return open;
	}


	/// <summary>
	///		Ejecuta el script de un archivo o una carpeta
	/// </summary>
	private void ExecuteFolderFile(object? parameter)
	{
		if (parameter is not null && parameter is string fileName && !string.IsNullOrWhiteSpace(fileName))
		{
			if (fileName.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase))
				ParseMarkdown(fileName);
		}
	}

	/// <summary>
	///		Interpreta el contenido de un archivo markdown
	/// </summary>
	private void ParseMarkdown(string fileName)
	{
		try
		{
			string html = new Processor.MarkdownProcessor().ParseFile(fileName);

				if (!string.IsNullOrWhiteSpace(html))
					MainController.HostPluginsController.OpenWebBrowserWithHtml(html);
		}
		catch (Exception exception)
		{
			MainController.HostController.SystemController.ShowMessage($"Error when parse markdown. {exception.Message}");
		}
	}

	/// <summary>
	///		Controlador de vistas principal
	/// </summary>
	public Controllers.IFileToolsController MainController { get; }

	/// <summary>
	///		Comando para ejecutar un archivo
	/// </summary>
	public BaseCommand ExecuteFileCommand { get; }
}
