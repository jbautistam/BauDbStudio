using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.RestStudio.Views
{
	/// <summary>
	///		Manager de vistas de RestStudio
	/// </summary>
	public class RestStudioViewManager : IPlugin
	{
		/// <summary>
		///		Inicializa el manager de vistas de DbStudio
		/// </summary>
		public void Initialize(IAppViewsController appViewsController, 
							   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ViewModels.RestStudioViewModel(new Controllers.RestStudioController(this, pluginController));
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
			MainViewModel.Load(MainViewModel.Path);
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
									Id = "TreeRestApiExplorer",
									Title = "RestApi",
									Position = PaneModel.PositionType.Left,
									View = new Explorers.TreeRestApiExplorer(MainViewModel.TreeRestApiViewModel)
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
			return new();
			//List<ToolBarModel> toolBars = new();

			//	// Añade la barra de herramientas del control de ejecución
			//	toolBars.Add(new ToolBarModel
			//							{
			//								Id = "ExecutionToolBar",
			//								ToolBar = new Controls.ExecutionToolBar(MainViewModel.ConnectionExecutionViewModel)
			//							}
			//				);
			//	// Devuelve la lista
			//	return toolBars;
		}

		/// <summary>
		///		Obtiene los menús del plugin
		/// </summary>
		public List<PluginsStudio.ViewModels.Base.Models.MenuListModel> GetMenus()
		{
			return new();
			//List<MenuListModel> menus = new();

			//	// Crea la lista de menús de "Nuevo elemento"
			//	menus.Add(GetMenus(MenuListModel.SectionType.NewItem));
			//	menus.Add(GetMenus(MenuListModel.SectionType.Tools));
			//	// Devuelve la lista de menús
			//	return menus;
		}

/*
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
							menuList.Add("_Distribución", MainViewModel.TreeConnectionsViewModel.NewDeploymentCommand, GetIcon("Deployment.png"));
							menuList.Add("_Storage", MainViewModel.TreeStoragesViewModel.NewStorageCommand, GetIcon("Search.png"));
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
*/
		/// <summary>
		///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
		/// </summary>
		public List<PluginsStudio.ViewModels.Base.Models.FileOptionsModel> GetFilesOptions()
		{
			return new();
			//return new PluginsStudio.ViewModels.Base.Models.Builders.FileOptionsBuilder()
			//					.WithOption()
			//						.WithFolder()
			//						.WithExtension("sql")
			//						.WithExtension("xml")
			//						.WithMenu(new MenuModel
			//											{
			//												Header = "Ejecutar",
			//												Icon = GetIcon("ArrowRight.png"),
			//												Command = MainViewModel.ConnectionExecutionViewModel.ExecuteFileCommand
			//											}
			//								 )
			//					.Build();
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
		public ViewModels.RestStudioViewModel MainViewModel { get; private set; }
	}
}