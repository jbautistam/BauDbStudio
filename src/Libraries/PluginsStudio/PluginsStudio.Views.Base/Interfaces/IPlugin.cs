namespace Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

/// <summary>
///		Interface para los plugin
/// </summary>
public interface IPlugin
{
	/// <summary>
	///		Inicializa el plugin
	/// </summary>
	void Initialize(IAppViewsController appController, ViewModels.Base.Controllers.IPluginsController pluginsController);

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	string GetKey();

	/// <summary>
	///		Carga los datos de un plugin
	/// </summary>
	void Load(string path);

	/// <summary>
	///		Actualiza los exploradores y ventanas
	/// </summary>
	void Refresh();

	/// <summary>
	///		Intenta abrir un archivo en un plugin
	/// </summary>
	bool OpenFile(string fileName);

	/// <summary>
	///		Obtiene los paneles del plugin
	/// </summary>
	List<Models.PaneModel>? GetPanes();

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	List<Models.ToolBarModel>? GetToolBars();

	/// <summary>
	///		Obtiene los menús del plugin
	/// </summary>
	List<ViewModels.Base.Models.MenuListModel>? GetMenus();

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	List<ViewModels.Base.Models.FileOptionsModel>? GetFilesOptions();

	/// <summary>
	///		Obtiene los archivos asignados a un plugin
	/// </summary>
	List<ViewModels.Base.Models.FileAssignedModel>? GetFilesAssigned();

	/// <summary>
	///		Obtiene la vista de configuración
	/// </summary>
	IPluginConfigurationView? GetConfigurationView();

	/// <summary>
	///		Cierra el plugin cuando se cierra la aplicación
	/// </summary>
	bool ClosePlugin();
}