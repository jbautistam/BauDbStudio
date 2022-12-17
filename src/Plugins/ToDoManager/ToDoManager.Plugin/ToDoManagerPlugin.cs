using System.Collections.Generic;

using Bau.Libraries.ToDoManager.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.ToDoManager.Plugin;

/// <summary>
///		Plugin para el administrador de contraseñas
/// </summary>
public class ToDoManagerPlugin : IPlugin
{ 
	/// <summary>
	///		Inicializa el manager de vistas del administrador de contraseñas
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new ToDoManagerViewModel(new Controllers.ToDoManagerController(this, pluginController));
		MainViewModel.Initialize();
	}

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
	public bool OpenFile(string fileName)
	{
		return MainViewModel.OpenFile(fileName);
	}

	/// <summary>
	///		Obtiene los paneles del plugin
	/// </summary>
	public List<PaneModel> GetPanes()
	{
		return new();
	}

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	public List<ToolBarModel> GetToolBars()
	{
		return new();
	}

	/// <summary>
	///		Obtiene los menús del plugin
	/// </summary>
	public List<MenuListModel> GetMenus()
	{
		return new();
	}

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	public List<FileOptionsModel> GetFilesOptions()
	{
		return null;
	}

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned()
	{
		return new()
				{
					new FileAssignedModel
							{
								Name = "ToDo file",
								FileExtension = ToDoManagerViewModel.ToDoFileExtension,
								Icon = "/ToDoManager.Plugin;component/Resources/ToDoFile.png"
							}
				};
	}

	/// <summary>
	///		Obtiene la vista de configuración del plugin
	/// </summary>
	public IPluginConfigurationView GetConfigurationView()
	{
		return null;
	}

	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	internal IAppViewsController AppViewsController { get; private set; }

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; private set; }
}
