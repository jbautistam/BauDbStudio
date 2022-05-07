using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.Libraries.DbStudio.Views
{
	/// <summary>
	///		Manager de vistas de DbStudio
	/// </summary>
	public class DbStudioViewManager : PluginsStudio.Views.Base.Interfaces.IPlugin
	{
		/// <summary>
		///		Inicializa el manager de vistas de DbStudio
		/// </summary>
		public void Initialize(PluginsStudio.Views.Base.Interfaces.IAppViewsController appViewsController, 
							   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ViewModels.DbStudioViewModel("DbStudio", new Controllers.DbStudioController(this, pluginController));
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
			MainViewModel.Load(MainViewModel.PathData);
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
			List<PaneModel> panes = new List<PaneModel>();

				// Añade los paneles de la aplicación principal
				panes.Add(new PaneModel
								{
									Id = "TreeConnectionsExplorer",
									Title = "Connections",
									Position = PaneModel.PositionType.Left,
									View = new Explorers.TreeConnectionsExplorer(MainViewModel.TreeConnectionsViewModel)
								}
						 );
				panes.Add(new PaneModel
								{
									Id = "TreeReportingExplorer",
									Title = "Reporting",
									Position = PaneModel.PositionType.Left,
									View = new Reporting.Explorers.TreeReportingExplorer(MainViewModel.ReportingSolutionViewModel.TreeReportingViewModel)
								}
						 );
				// Devuelve la lista de paneles
				return panes;
		}

		/// <summary>
		///		Obtiene las barras de herramientas del plugin
		/// </summary>
		public List<ToolBarModel> GetToolBars()
		{
			List<ToolBarModel> toolBars = new();

				// Añade la barra de herramientas del control de ejecución
				toolBars.Add(new ToolBarModel
										{
											Id = "ExecutionToolBar",
											ToolBar = new Controls.ExecutionToolBar(MainViewModel.ConnectionExecutionViewModel)
										}
							);
				// Devuelve la lista
				return toolBars;
		}

		/// <summary>
		///		Obtiene los menús del plugin
		/// </summary>
		public List<MenuListModel> GetMenus()
		{
			List<MenuListModel> menus = new();

				// Crea la lista de menús de "Nuevo elemento"
				menus.Add(GetMenus(MenuListModel.SectionType.NewItem));
				menus.Add(GetMenus(MenuListModel.SectionType.Tools));
				// Devuelve la lista de menús
				return menus;
		}

		/// <summary>
		///		Obtiene los menús
		/// </summary>
		private MenuListModel GetMenus(MenuListModel.SectionType section)
		{
			MenuListModel menuList = new(section);

				// Obtiene los elementos del menú
				switch (section)
				{
					case MenuListModel.SectionType.NewItem:
							menuList.Add("_Conexión", MainViewModel.TreeConnectionsViewModel.NewConnectionCommand, GetIcon("Connection.png"));
							menuList.AddSeparator();
							menuList.Add("_Consulta", MainViewModel.TreeConnectionsViewModel.NewQueryCommand, GetIcon("Script.png"));
							menuList.AddSeparator();
							menuList.Add("_Xml de pruebas", MainViewModel.CreateTestXmlCommand, GetIcon("FileXml.png"));
						break;
					case MenuListModel.SectionType.Tools:
							menuList.Add("_Exportar tablas de datos ... ", MainViewModel.ConnectionExecutionViewModel.ExportDataBaseCommand, GetIcon("Export.png"));
							menuList.AddSeparator();
							menuList.Add("_Crear scripts validación ...", MainViewModel.CreateValidationScriptsCommand, GetIcon("FileSql.png"));
							menuList.Add("Crear script de importación de archivos ...", MainViewModel.CreateImportFilesScriptsCommand, GetIcon("FileSql.png"));
							menuList.AddSeparator();
							menuList.Add("Generar XML de esquema ...", MainViewModel.CreateSchemaXmlCommand, GetIcon("FileXml.png"));
							menuList.Add("Generar XML de reporting ...", MainViewModel.CreateSchemaReportingXmlCommand, GetIcon("FileXml.png"));
							menuList.Add("Generar scripts SQL de reporting ...", MainViewModel.CreateSchemaReportingSqlCommand, GetIcon("FileXml.png"));
						break;
				}
				// Devuelve la lista de menús
				return menuList;
		}

		/// <summary>
		///		Obtiene la URL completa de un icono
		/// </summary>
		private string GetIcon(string resource)
		{
			return $"pack://application:,,,/DbStudio.Views;component/Resources/Images/{resource}";
		}

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
		public List<PluginsStudio.ViewModels.Base.Models.FileAssignedModel> GetFilesAssigned()
		{
			List<PluginsStudio.ViewModels.Base.Models.FileAssignedModel> files = new List<FileAssignedModel>();

				// Asigna las extensions
				files.Add(new FileAssignedModel
									{
										FileExtension = ".sql",
										Icon = GetIcon("FileSql.png")
									}
						 );
				files.Add(new FileAssignedModel
									{
										FileExtension = ".sqlx",
										Icon = GetIcon("FileSqlExtended.png")
									}
						 );
				files.Add(new FileAssignedModel
									{
										FileExtension = ".xml",
										Icon = GetIcon("FileXml.png")
									}
						 );
				// Devuelve la lista de archivos asignados
				return files;
		}

		/// <summary>
		///		Obtiene la vista de configuración (en este caso, no devuelve nada)
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
		public ViewModels.DbStudioViewModel MainViewModel { get; private set; }
	}
}
