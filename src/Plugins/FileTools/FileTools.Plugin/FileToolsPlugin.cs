using Bau.Libraries.FileTools.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.FileTools.Plugin;

/// <summary>
///		Plugin para el procesador de consolas
/// </summary>
public class FileToolsPlugin : IPlugin
{ 
	/// <summary>
	///		Inicializa el manager de vistas del procesador
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new FileToolsViewModel(new Controllers.FileToolsController(this, pluginController));
		MainViewModel.Initialize();
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "FileTools";

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
	public List<FileOptionsModel> GetFilesOptions()
	{
		return new PluginsStudio.ViewModels.Base.Models.Builders.FileOptionsBuilder()
							.WithOption()
								.WithExtension(".md")
								.WithMenu(new MenuModel
													{
														Header = "Show markdown",
														Icon = GetIconName("ShowMarkdown.png", true),
														Command = MainViewModel.ExecuteFileCommand
													}
										 )
							.WithOption()
								.WithExtension(".json")
								.WithMenu(new MenuModel
													{
														Header = "Prettifier",
														Icon = GetIconName("Format.png", true),
														Command = MainViewModel.ExecuteFileCommand
													}
										 )
							.Build();
	}

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned()
	{
		List<FileAssignedModel> files = [
											GetFileAssigned("Pattern text file", FileToolsViewModel.PatternFileExtension, "PatternFile.png", true),
											GetMediaFileAssigned(true, ".mp3"),
											GetMediaFileAssigned(true, ".wav"),
											GetMediaFileAssigned(false, ".mp4"),
											GetMediaFileAssigned(false, ".mkv"),
											GetMediaFileAssigned(false, ".avi")
										];

			// Añade la lista de archivos de imágenes
			foreach ((string name, string extension) in MainViewModel.ImageTypeFiles)
				files.Add(GetImageFileAssigned(name, extension));
			// Devuelve la lista de archivos
			return files;

		// Obtiene el archivo asociado a una extensión
		FileAssignedModel GetFileAssigned(string name, string extension, string icon, bool canCreate)
		{
			return new FileAssignedModel
							{
								Name = name,
								FileExtension = extension,
								Icon = GetIconName(icon, true),
								CanCreate = canCreate
							};
		}

		// Obtiene el archivo asociado a una extensión
		FileAssignedModel GetMediaFileAssigned(bool isAudio, string extension)
		{
			if (isAudio)
				return GetFileAssigned($"Audio {extension}", extension, "AudioFile.png", false);
			else
				return GetFileAssigned($"Vídeo {extension}", extension, "VideoFile.png", false);
		}

		// Obtiene el archivo asociado a una extensión de un archivo de imagen
		FileAssignedModel GetImageFileAssigned(string name, string extension)
		{
			return new FileAssignedModel
							{
								Name = name,
								FileExtension = extension,
								Icon = GetIconName("FileImage.png", true),
								CanCreate = false
							};
		}
	}

	/// <summary>
	///		Obtiene un nombre de recurso
	/// </summary>
	private string GetIconName(string icon, bool packApplication)
	{
		if (packApplication)
			return $"pack://application:,,,/FileTools.Plugin;component/Resources/{icon}";
		else
			return $"/FileTools.Plugin;component/Resources/{icon}";
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
	public FileToolsViewModel MainViewModel { get; private set; } = default!;
}