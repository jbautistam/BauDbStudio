using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.Libraries.DbStudio.Views;

/// <summary>
///		Manager de vistas de DbStudio
/// </summary>
public class DbStudioViewManager : IPlugin
{
	/// <summary>
	///		Inicializa el manager de vistas de DbStudio
	/// </summary>
	public void Initialize(IAppViewsController appViewsController, 
						   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AppViewsController = appViewsController;
		MainViewModel = new ViewModels.DbStudioViewModel("DbStudio", new Controllers.DbStudioController(this, pluginController));
	}

	/// <summary>
	///		Obtiene la clave del plugin
	/// </summary>
	public string GetKey() => "DbStudio";

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
		return new()
				{
					new PaneModel
							{
								Id = "TreeConnectionsExplorer",
								Title = "Connections",
								Position = PaneModel.PositionType.Left,
								View = new Explorers.TreeConnectionsExplorer(MainViewModel.TreeConnectionsViewModel)
							},
					new PaneModel
							{
								Id = "TreeReportingExplorer",
								Title = "Reporting",
								Position = PaneModel.PositionType.Left,
								View = new Reporting.Explorers.TreeReportingExplorer(MainViewModel.ReportingSolutionViewModel.TreeReportingViewModel)
							}
				};
	}

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	public List<ToolBarModel> GetToolBars()
	{
		return new()
				{
					new ToolBarModel
									{
										Id = "ExecutionToolBar",
										ToolBar = new Controls.ExecutionToolBar(MainViewModel.ConnectionExecutionViewModel)
									}
				};
	}

	/// <summary>
	///		Obtiene los menús del plugin
	/// </summary>
	public List<MenuListModel> GetMenus()
	{
		return new PluginsStudio.ViewModels.Base.Models.Builders.MenuBuilder()
						.WithMenu(MenuListModel.SectionType.NewItem)
							.WithItem("_Conexión", MainViewModel.TreeConnectionsViewModel.NewConnectionCommand, GetIcon("Connection.png"))
							.WithSeparator()
							.WithItem("_Consulta", MainViewModel.TreeConnectionsViewModel.NewQueryCommand, GetIcon("Script.png"))
							.WithSeparator()
							.WithItem("_Xml de pruebas", MainViewModel.CreateTestXmlCommand, GetIcon("FileSql.png"))
						.WithMenu(MenuListModel.SectionType.Tools)
							.WithItem("_Crear scripts validación ...", MainViewModel.CreateValidationScriptsCommand, GetIcon("FileSql.png"))
							.WithItem("Crear script de importación de archivos ...", MainViewModel.CreateImportFilesScriptsCommand, GetIcon("FileSql.png"))
					.Build();
	}

	/// <summary>
	///		Obtiene la URL completa de un icono
	/// </summary>
	private string GetIcon(string icon) => $"pack://application:,,,/DbStudio.Views;component/Resources/Images/{icon}";

	/// <summary>
	///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
	/// </summary>
	public List<FileOptionsModel> GetFilesOptions()
	{
		return new PluginsStudio.ViewModels.Base.Models.Builders.FileOptionsBuilder()
							.WithOption()
								.WithFolder()
								.WithExtension("sql")
								.WithMenu(new MenuModel
													{
														Header = "Ejecutar",
														Icon = GetIcon("ArrowRight.png"),
														Command = MainViewModel.ConnectionExecutionViewModel.ExecuteFileCommand
													}
										 )
							.Build();
	}

	/// <summary>
	///		Obtiene las extensiones de archivo asociadas al plugin
	/// </summary>
	public List<FileAssignedModel> GetFilesAssigned()
	{
		return new PluginsStudio.ViewModels.Base.Models.Builders.FileAssignedBuilder()
						.WithFile("Sql Script", ".sql", GetIcon("FileSql.png"))
						.WithFile("Sql Extended", ".sqlx", GetIcon("FileSqlExtended.png"))
					.Build();
	}

	/// <summary>
	///		Obtiene la vista de configuración (en este caso, no devuelve nada)
	/// </summary>
	public IPluginConfigurationView GetConfigurationView() => new Configuration.ctlConfiguration(MainViewModel.ConfigurationViewModel);

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
	public ViewModels.DbStudioViewModel MainViewModel { get; private set; } = default!;
}
