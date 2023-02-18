using System;
using System.Collections.Generic;

using Bau.Libraries.MultimediaFiles.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.MultimediaFiles.Plugin
{
	/// <summary>
	///		Plugin para el lector de blogs
	/// </summary>
	public class MultimediaFilesPlugin : IPlugin
	{ 
		/// <summary>
		///		Inicializa el manager de vistas del lector de cómics
		/// </summary>
		public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new MultimediaFilesViewModel(new Controllers.MultimediaFilesController(this, pluginController));
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
		public List<PaneModel> GetPanes()
		{
			return new()
						{
							new PaneModel
								{
									Id = "MultimediaFiles",
									Title = "Files multimedia",
									Position = PaneModel.PositionType.Right,
									View = new Views.MediaPlayerView(MainViewModel.MediaFileListViewModel)
								}
						 };
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
		public List<FileAssignedModel> GetFilesAssigned()
		{
			return new() {
							GetIcon(true, ".mp3"),
							GetIcon(true, ".wav"),
							GetIcon(false, ".mp4"),
							GetIcon(false, ".mkv"),
							GetIcon(false, ".avi")
						 };

				// Obtiene el icono asociado a una extensión
				FileAssignedModel GetIcon(bool isAudio, string extension)
				{
					if (isAudio)
						return new FileAssignedModel
										{
											Name = $"Audio {extension}",
											FileExtension = extension,
											Icon = "/MultimediaFiles.Plugin;component/Resources/AudioFile.png",
											CanCreate = false
										};
					else
						return new FileAssignedModel
										{
											Name = $"Vídeo {extension}",
											FileExtension = extension,
											Icon = "/MultimediaFiles.Plugin;component/Resources/VideoFile.png",
											CanCreate = false
										};

				}
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
		public MultimediaFilesViewModel MainViewModel { get; private set; }
	}
}
