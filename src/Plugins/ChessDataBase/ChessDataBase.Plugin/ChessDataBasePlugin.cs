using Bau.Libraries.ChessDataBase.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.ChessDataBase.Plugin;

/// <summary>
///		Plugin para el lector de archivos PGN
/// </summary>
public class ChessDataBasePlugin : IPlugin
{ 
	/// <summary>
	///		Inicializa el manager de vistas del lector de cómics
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new MainViewModel(new Controllers.ChessDataBaseController(this, pluginController));
		MainViewModel.Initialize();
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "ChessDataBase";

	/// <summary>
	///		Carga los datos del directorio
	/// </summary>
	public void Load(string path)
	{
		// No hace nada excepto implementar la interface
	}

	/// <summary>
	///		Actualiza los exploradores y ventanas
	/// </summary>
	public void Refresh()
	{
		// No hace nada excepto implementar la interface
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
		return new List<FileAssignedModel>()
							{
								new FileAssignedModel
										{
											Name = "Chess game",
											FileExtension = ".pgn",
											Icon = GetIcon("FilePgn.png"),
											CanCreate = false
										}
							};
	}

	/// <summary>
	///		Obtiene la dirección de un icono
	/// </summary>
	private string GetIcon(string name) => $"/ChessDataBase.Plugin;component/Resources/{name}";

	/// <summary>
	///		Obtiene la vista de configuración del plugin
	/// </summary>
	public IPluginConfigurationView GetConfigurationView() => new Views.Configuration.ctlConfiguration(MainViewModel.ConfigurationViewModel);

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
	public MainViewModel MainViewModel { get; private set; } = default!;

	/// <summary>
	///		Caché de imágenes
	/// </summary>
	public static BauMvvm.Views.Wpf.Tools.ImagesCache ImagesCache = new();
}
