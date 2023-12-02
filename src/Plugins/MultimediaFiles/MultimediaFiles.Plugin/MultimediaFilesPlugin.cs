using Bau.Libraries.MultimediaFiles.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.MultimediaFiles.Plugin;

/// <summary>
///		Plugin para el visualizador de arhcivos multimerdia
/// </summary>
public class MultimediaFilesPlugin : IPlugin
{
	/// <summary>
	///		Inicializa el manager del visualizador
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new MultimediaFilesViewModel(new Controllers.MultimediaFilesController(this, pluginController));
		MainViewModel.Initialize();
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "MultimediaFiles";

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
						GetIcon(true, ".mp3"),
						GetIcon(true, ".wav"),
						GetIcon(false, ".mp4"),
						GetIcon(false, ".mkv"),
						GetIcon(false, ".avi")
					 };

		// Obtiene el icono asociado a una extensión
		FileAssignedModel GetIcon(bool isAudio, string extension)
		{
			if (isAudio)
				return new FileAssignedModel
				{
					Name = $"Audio {extension}",
					FileExtension = extension,
					Icon = "/MultimediaFiles.Plugin;component/Resources/AudioFile.png",
					CanCreate = false
				};
			else
				return new FileAssignedModel
				{
					Name = $"Vídeo {extension}",
					FileExtension = extension,
					Icon = "/MultimediaFiles.Plugin;component/Resources/VideoFile.png",
					CanCreate = false
				};

		}
	}

	/// <summary>
	///		Obtiene la vista de configuración del plugin
	/// </summary>
	public IPluginConfigurationView GetConfigurationView() => null;

	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	internal IAppViewsController AppViewsController { get; private set; }

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public MultimediaFilesViewModel MainViewModel { get; private set; }
}
