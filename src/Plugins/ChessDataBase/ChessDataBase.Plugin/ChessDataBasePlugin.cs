using System;
using System.Collections.Generic;

using Bau.Libraries.ChessDataBase.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.ChessDataBase.Plugin
{
	/// <summary>
	///		Plugin para el lector de archivos PGN
	/// </summary>
	public class ChessDataBasePlugin : IPlugin
	{ 
		/// <summary>
		///		Inicializa el manager de vistas del lector de cómics
		/// </summary>
		public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ChessDataBase.ViewModels.MainViewModel(new Controllers.EBookReaderController(this, pluginController));
			MainViewModel.Initialize();
		}

		/// <summary>
		///		Carga los datos del directorio
		/// </summary>
		public void Load(string path)
		{
			// No hace nada excepto implementar la interface
		}

		/// <summary>
		///		Actualiza los exploradores y ventanas
		/// </summary>
		public void Refresh()
		{
			// No hace nada excepto implementar la interface
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
			return new();
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
			return new();
		}

		/// <summary>
		///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
		/// </summary>
		public List<FileOptionsModel> GetFilesOptions()
		{
			return null;
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
										FileExtension = ".pgn",
										Icon = "/ChessDataBase.Plugin;component/Resources/FilePgn.png"
									}
						 );
				// Devuelve la lista de archivos asignados
				return files;
		}

		/// <summary>
		///		Obtiene la vista de configuración del plugin
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
		public ChessDataBase.ViewModels.MainViewModel MainViewModel { get; private set; }
	}
}
