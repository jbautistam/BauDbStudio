using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.Libraries.CloudStudio.Plugin
{
	/// <summary>
	///		Manager de vistas de CloudStudio
	/// </summary>
	public class CloudStudioPlugin : PluginsStudio.Views.Base.Interfaces.IPlugin
	{
		/// <summary>
		///		Inicializa el manager de vistas de CloudStudio
		/// </summary>
		public void Initialize(PluginsStudio.Views.Base.Interfaces.IAppViewsController appViewsController, 
							   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ViewModels.CloudStudioViewModel("CloudStudio", new Controllers.CloudStudioController(this, pluginController));
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
									Id = "TreeStorageExplorer",
									Title = "Storage",
									Position = PaneModel.PositionType.Right,
									View = new Explorers.TreeStoragesExplorer(MainViewModel.TreeStoragesViewModel)
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
		}

		/// <summary>
		///		Obtiene los menús del plugin
		/// </summary>
		public List<MenuListModel> GetMenus()
		{
			List<MenuListModel> menus = new();

				// Crea la lista de menús de "Nuevo elemento"
				menus.Add(GetMenus(MenuListModel.SectionType.NewItem));
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
							menuList.Add("_Storage", MainViewModel.TreeStoragesViewModel.NewStorageCommand, GetIcon("Search.png"));
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
			return $"pack://application:,,,/CloudStudio.Views;component/Resources/Images/{resource}";
		}

		/// <summary>
		///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
		/// </summary>
		public List<FileOptionsModel> GetFilesOptions()
		{
			return new();
		}

		/// <summary>
		///		Obtiene las extensiones de archivo asociadas al plugin
		/// </summary>
		public List<PluginsStudio.ViewModels.Base.Models.FileAssignedModel> GetFilesAssigned()
		{
			return new();
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
		public ViewModels.CloudStudioViewModel MainViewModel { get; private set; }
	}
}
