using System;
using System.Collections.Generic;

using Bau.Libraries.LibBlogReader.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.BlogReader.Views
{
	/// <summary>
	///		Plugin para el lector de blogs
	/// </summary>
	public class BlogReaderPlugin : IPlugin
	{ 
		/// <summary>
		///		Inicializa el manager de vistas de DbStudio
		/// </summary>
		public void Initialize(IAppViewsController appViewsController, 
							   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new BlogReaderViewModel(new Controllers.BlogController(this, pluginController));
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
			return false;
		}

		/// <summary>
		///		Obtiene los paneles del plugin
		/// </summary>
		public List<PaneModel> GetPanes()
		{
			return new()
						{
							new PaneModel
									{
										Id = "TreeBlogs",
										Title = "Blogs",
										Position = PaneModel.PositionType.Left,
										View = new Views.BlogTreeControlView(MainViewModel.TreeBlogs)
									}
						};
		}

		/// <summary>
		///		Obtiene las barras de herramientas del plugin
		/// </summary>
		public List<ToolBarModel> GetToolBars() => new();

		/// <summary>
		///		Obtiene los menús del plugin
		/// </summary>
		public List<MenuListModel> GetMenus() => new();

		/// <summary>
		///		Obtiene las extensiones de archivo asociadas al plugin
		/// </summary>
		public List<FileAssignedModel> GetFilesAssigned() => new();

		/// <summary>
		///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
		/// </summary>
		public List<FileOptionsModel> GetFilesOptions() => null;

		/// <summary>
		///		Obtiene la vista de configuración del plugin
		/// </summary>
		public IPluginConfigurationView GetConfigurationView()
		{
			return new Views.Configuration.ctlConfigurationBlogReader(MainViewModel.ConfigurationViewModel);
		}

		/// <summary>
		///		Controlador de aplicación
		/// </summary>
		internal IAppViewsController AppViewsController { get; private set; }

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public BlogReaderViewModel MainViewModel { get; private set; }
	}
}
