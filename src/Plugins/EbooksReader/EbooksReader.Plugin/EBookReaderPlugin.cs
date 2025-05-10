using Bau.Libraries.EbooksReader.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.EbooksReader.Plugin;

/// <summary>
///		Plugin para el lector de eBooks y cómics
/// </summary>
public class EBookReaderPlugin : IPlugin
{ 
	/// <summary>
	///		Inicializa el manager de vistas del lector de ebooks y cómics
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new EBookReaderViewModel(new Controllers.EBookReaderController(this, pluginController));
		MainViewModel.Initialize();
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "EBookReader";

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
	public List<FileAssignedModel> GetFilesAssigned()
	{
		return new List<FileAssignedModel>
						{
							GetFileAssigned("Ebook", ".epub", "FileEpub"),
							GetFileAssigned("Ebook", ".mobi", "FileMobi"),
							GetFileComic(".cbr"),
							GetFileComic(".cbz"),
							GetFileComic(".zip"),
							GetFileComic(".rar"),
						};

		// Obtiene un archivo de cómic
		FileAssignedModel GetFileComic(string extension) => GetFileAssigned($"Comic {extension}", extension, "FileCbr"); 

		// Obtiene un archivo
		FileAssignedModel GetFileAssigned(string name, string extension, string icon)
		{
			return new FileAssignedModel
							{
								Name = name,
								FileExtension = extension,
								Icon = $"/EbooksReader.Plugin;component/Resources/{icon}.png",
								CanCreate = false
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
	public EBookReaderViewModel MainViewModel { get; private set; } = default!;
}
