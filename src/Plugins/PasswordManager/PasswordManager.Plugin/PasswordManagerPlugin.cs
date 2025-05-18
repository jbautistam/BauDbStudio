using Bau.Libraries.PasswordManager.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.PasswordManager.Plugin;

/// <summary>
///		Plugin para el administrador de contraseñas
/// </summary>
public class PasswordManagerPlugin : IPlugin
{ 
	/// <summary>
	///		Inicializa el manager de vistas del administrador de contraseñas
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new PasswordManagerViewModel(new Controllers.PasswordManagerController(this, pluginController));
		MainViewModel.Initialize();
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "PasswordManager";

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
	public List<FileOptionsModel>? GetFilesOptions() => null;

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel>? GetFilesAssigned()
	{
		 return new List<FileAssignedModel>
							{
								GetFileAssigned(".bau.enc", false)
							};

			// Crea un archivo asignado
			FileAssignedModel GetFileAssigned(string extension, bool canSearch)
			{
				return new FileAssignedModel
								{
									Name = "Password manager file",
									FileExtension = extension,
									Icon = "/PasswordManager.Plugin;component/Resources/FileEnc.png",
									CanSearch = canSearch
								};
			}
	}

	/// <summary>
	///		Obtiene la vista de configuración del plugin
	/// </summary>
	public IPluginConfigurationView? GetConfigurationView() => null;

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
	public PasswordManagerViewModel MainViewModel { get; private set; } = default!;
}
