using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.Views.Base.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.Libraries.StructuredFilesStudio.Views
{
	/// <summary>
	///		Manager de vistas de DbStudio
	/// </summary>
	public class StructuredFilesStudioViewManager : PluginsStudio.Views.Base.Interfaces.IPlugin
	{
		/// <summary>
		///		Inicializa el manager de vistas de DbStudio
		/// </summary>
		public void Initialize(PluginsStudio.Views.Base.Interfaces.IAppViewsController appViewsController, 
							   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ViewModels.StructuredFilesStudioViewModel("StructuredFilesStudio", 
																		  new Controllers.StructuredFilesStudioController(this, pluginController));
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
		///		Obtiene la URL completa de un icono
		/// </summary>
		private string GetIcon(string resource)
		{
			return $"pack://application:,,,/StructuredFilesStudio.Views;component/Resources/Images/{resource}";
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
			List<PluginsStudio.ViewModels.Base.Models.FileAssignedModel> files = new List<FileAssignedModel>();

				// Asigna las extensions
				files.Add(new FileAssignedModel
									{
										FileExtension = ".csv",
										Icon = GetIcon("FileCsv.png")
									}
						 );
				files.Add(new FileAssignedModel
									{
										FileExtension = ".xls",
										Icon = GetIcon("FileExcel.png")
									}
						 );
				files.Add(new FileAssignedModel
									{
										FileExtension = ".xlsx",
										Icon = GetIcon("FileExcel.png")
									}
						 );
				files.Add(new FileAssignedModel
									{
										FileExtension = ".parquet",
										Icon = GetIcon("FileParquet.png")
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
		public ViewModels.StructuredFilesStudioViewModel MainViewModel { get; private set; }
	}
}
