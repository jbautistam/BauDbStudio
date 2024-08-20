using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.Libraries.StructuredFilesStudio.Views;

/// <summary>
///		Manager de vistas del plugin de manejo de archivos estructurados (CSV, Parquet, Excel)
/// </summary>
public class StructuredFilesStudioViewManager : IPlugin
{
	/// <summary>
	///		Inicializa el manager de vistas de DbStudio
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, 
						   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new ViewModels.StructuredFilesStudioViewModel("StructuredFilesStudio", 
																	  new Controllers.StructuredFilesStudioController(this, pluginController));
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "StructuredFiles";

	/// <summary>
	///		Carga los datos del directorio
	/// </summary>
	public void Load(string path)
	{
		// ... no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Actualiza los exploradores y ventanas
	/// </summary>
	public void Refresh()
	{
		// ... no hace nada, simplemente implementa la interface
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
	///		Obtiene la URL completa de un icono
	/// </summary>
	private string GetIcon(string resource) => $"pack://application:,,,/StructuredFilesStudio.Views;component/Resources/Images/{resource}";

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	public List<FileOptionsModel> GetFilesOptions() => new();

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned()
	{
        return new PluginsStudio.ViewModels.Base.Models.Builders.FileAssignedBuilder()
							.WithFile("Csv file", ".csv", GetIcon("FileCsv.png"))
								.WithCanCreate(false)
							.WithFile("Excel file", ".xls", GetIcon("FileExcel.png"))
								.WithCanCreate(false)
							.WithFile("Excel file", ".xlsx", GetIcon("FileExcel.png"))
								.WithCanCreate(false)
							.WithFile("Parquet file", ".parquet", GetIcon("FileParquet.png"))
								.WithCanCreate(false)
						.Build();
	}

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
	public ViewModels.StructuredFilesStudioViewModel MainViewModel { get; private set; } = default!;
}
