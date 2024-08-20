namespace Bau.Libraries.AiTools.ViewModels;

/// <summary>
///		ViewModel principal de las herramientas de IA
/// </summary>
public class AiToolsViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Constantes públicas
	public const string ExtensionAiImageFile = ".imgprompt";
	public const string ExtensionAiChatFile = ".chatprompt";

	public AiToolsViewModel(Controllers.IAiToolsController mainController)
	{
		ViewsController = mainController;
		ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
		AiImageGenerationManager = new Controllers.Processors.AiImageGenerationManager(this);
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
	}

	/// <summary>
	///		Carga el directorio
	/// </summary>
	public void Load(string path)
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Abre un archivo de prompt
	/// </summary>
	public bool OpenFile(string fileName)
	{
		// Abre el viewModel correspondiente
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			if (fileName.EndsWith(ExtensionAiImageFile, StringComparison.CurrentCultureIgnoreCase))
			{
				// Abre la ventana
				ViewsController.OpenWindow(new Prompts.PromptFileViewModel(this, fileName));
				// e indica que ha podido abrir el archivo
				return true;
			}
			else if (fileName.EndsWith(ExtensionAiChatFile, StringComparison.CurrentCultureIgnoreCase))
			{
				// Abre la ventana
				ViewsController.OpenWindow(new TextPrompt.ChatViewModel(this, fileName));
				// e indica que ha podido abrir el archivo
				return true;
			}
		}
		// No es un archivo definido
		return false;
	}

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IAiToolsController ViewsController { get; }

	/// <summary>
	///		ViewModel de configuración
	/// </summary>
	public Configuration.ConfigurationViewModel ConfigurationViewModel { get; }

	/// <summary>
	///		Manager de la generación de imágenes
	/// </summary>
	internal Controllers.Processors.AiImageGenerationManager AiImageGenerationManager { get; }
}