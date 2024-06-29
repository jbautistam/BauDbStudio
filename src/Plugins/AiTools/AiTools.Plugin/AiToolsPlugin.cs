using Bau.Libraries.AiTools.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.AiTools.Plugin;

/// <summary>
///		Plugin para las herramientas de IA
/// </summary>
public class AiToolsPlugin : IPlugin
{
	/// <summary>
	///		Inicializa el manager del visualizador
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new AiToolsViewModel(new Controllers.AiToolsController(this, pluginController));
		MainViewModel.Initialize(); 
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "AiToolsPlugin";

	/// <summary>
	///		Carga los datos del directorio
	/// </summary>
	public void Load(string path)
	{
		MainViewModel.Load(path);
	}

	/// <summary>
	///		Actualiza los exploradores y ventanas
	/// </summary>
	public void Refresh()
	{
		MainViewModel.Load(string.Empty);
	}

	/// <summary>
	///		Intenta abrir un archivo en un plugin
	/// </summary>
	public bool OpenFile(string fileName) => MainViewModel.OpenFile(fileName);

	/// <summary>
	///		Obtiene los paneles del plugin
	/// </summary>
	public List<PaneModel> GetPanes() => new();

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	public List<ToolBarModel> GetToolBars() => new();

	/// <summary>
	///		Obtiene los menús del plugin
	/// </summary>
	public List<MenuListModel> GetMenus() => new();

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	public List<FileOptionsModel> GetFilesOptions() => new();

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned()
	{
		return new() {
						GetIcon("AI Images", AiToolsViewModel.ExtensionAiImageFile, "AiImageFile.png"),
						GetIcon("AI Chat", AiToolsViewModel.ExtensionAiChatFile, "AiChatFile.png")
					 };

		// Obtiene el icono asociado a una extensión
		FileAssignedModel GetIcon(string name, string extension, string image)
		{
			return new FileAssignedModel
				{
					Name = name,
					FileExtension = extension,
					Icon = $"/AiTools.Plugin;component/Resources/{image}",
					CanCreate = true
				};
		}
	}

	/// <summary>
	///		Obtiene la vista de configuración del plugin
	/// </summary>
	public IPluginConfigurationView? GetConfigurationView() => new Views.Configuration.ctlConfiguration(MainViewModel.ConfigurationViewModel);

	/// <summary>
	///		Cierra el plugin (en este caso simplemente implementa la interface)
	/// </summary>
	public bool ClosePlugin() => true;

	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	internal IAppViewsController AppViewsController { get; private set; } = default!;

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public AiToolsViewModel MainViewModel { get; private set; } = default!;
}
