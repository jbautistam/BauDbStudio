using System;
using System.Collections.Generic;

using Bau.Libraries.ComicsReader.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.ComicsReader.Plugin
{
	/// <summary>
	///		Plugin para el lector de cómics
	/// </summary>
	public class ComicReaderPlugin : IPlugin
	{ 
		/// <summary>
		///		Inicializa el manager de vistas del lector de cómics
		/// </summary>
		public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ComicReaderViewModel(new Controllers.ComicReaderController(this, pluginController));
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
		public List<FileOptionsModel> GetFilesOptions() => null;

		/// <summary>
		///		Obtiene las extensiones de archivo asociadas al plugin
		/// </summary>
		public List<FileAssignedModel> GetFilesAssigned()
		{
			return new List<FileAssignedModel>()
								{
									GetIcon(".cbr"),
									GetIcon(".cbz"),
									GetIcon(".zip"),
									GetIcon(".rar")
								};

			// Obtiene los datos del archivo asignado
			FileAssignedModel GetIcon(string extension)
			{
				return new FileAssignedModel
								{
									Name = $"Comic {extension}",
									FileExtension = extension,
									Icon = "/ComicsReader.Plugin;component/Resources/FileCbr.png",
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
		public ComicReaderViewModel MainViewModel { get; private set; }
	}
}
