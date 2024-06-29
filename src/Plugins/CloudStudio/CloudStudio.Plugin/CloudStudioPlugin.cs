using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.Libraries.CloudStudio.Plugin;

/// <summary>
///		Manager de vistas de CloudStudio
/// </summary>
public class CloudStudioPlugin : IPlugin
{
	/// <summary>
	///		Inicializa el manager de vistas de CloudStudio
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, 
						   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new ViewModels.CloudStudioViewModel("CloudStudio", new Controllers.CloudStudioController(this, pluginController));
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "CloudStudio";

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
		MainViewModel.Load(MainViewModel.PathData);
	}

	/// <summary>
	///		Intenta abrir un archivo en un plugin
	/// </summary>
	public bool OpenFile(string fileName) => MainViewModel.OpenFile(fileName);

	/// <summary>
	///		Obtiene los paneles del plugin
	/// </summary>
	public List<PaneModel> GetPanes()
	{
		return new List<PaneModel>
						{ 
							new PaneModel
									{
										Id = "TreeStorageExplorer",
										Title = "Storage",
										Position = PaneModel.PositionType.Right,
										View = new Explorers.TreeStoragesExplorer(MainViewModel.TreeStoragesViewModel)
									}
						};
	}

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	public List<ToolBarModel> GetToolBars() => new();

	/// <summary>
	///		Obtiene los menús del plugin
	/// </summary>
	public List<MenuListModel> GetMenus()
	{
		return new PluginsStudio.ViewModels.Base.Models.Builders.MenuBuilder()
						.WithMenu(MenuListModel.SectionType.NewItem)
							.WithItem("Storage", MainViewModel.TreeStoragesViewModel.NewStorageCommand, GetIcon("Storage.png"))
					.Build();
	}

	/// <summary>
	///		Obtiene la URL completa de un icono
	/// </summary>
	private string GetIcon(string resource) => $"pack://application:,,,/CloudStudio.Plugin;component/Resources/Images/{resource}";

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	public List<FileOptionsModel> GetFilesOptions() => new();

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned() => new();

	/// <summary>
	///		Obtiene la vista de configuración (en este caso, no devuelve nada)
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
	public ViewModels.CloudStudioViewModel MainViewModel { get; private set; } = default!;
}
