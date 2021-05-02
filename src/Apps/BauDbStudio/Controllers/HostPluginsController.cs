using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Files;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador del host de plugins
	/// </summary>
	public class HostPluginsController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IHostPluginsController
	{
		public HostPluginsController(DbStudioViewsManager dbStudioViewManager)
		{
			DbStudioViewManager = dbStudioViewManager;
		}

		/// <summary>
		///		Añade el archivo utilizado a la lista
		/// </summary>
		public void AddFileUsed(string fileName)
		{
			DbStudioViewManager.PluginsStudioViewModel.LastFilesViewModel.Add(fileName);
		}

		/// <summary>
		///		Abre el editor de un archivo
		/// </summary>
		public void OpenFile(string fileName)
		{
			DbStudioViewManager.OpenFile(fileName);
		}

		/// <summary>
		///		Abre un editor sobre un archivo de texto
		/// </summary>
		public void OpenEditor(BaseTextFileViewModel viewModel)
		{
			DbStudioViewManager.PluginStudioController.OpenWindow(viewModel);
		}

		/// <summary>
		///		Abre un navegador sobre una URL
		/// </summary>
		public void OpenWebBrowser(string url)
		{
			DbStudioViewManager.PluginsStudioViewModel.OpenWebBrowser(url);
		}

		/// <summary>
		///		Actualiza el árbol de archivos
		/// </summary>
		public void RefreshFiles()
		{
			DbStudioViewManager.PluginsStudioViewModel.RefreshFiles();
		}

		/// <summary>
		///		Ejecuta un comando sobre un plugin
		/// </summary>
		public void ExecutePluginCommand(string plugin, string viewModel, string command)
		{
			DbStudioViewManager.PluginsManager.ExecutePluginCommand(plugin, viewModel, command);
		}

		/// <summary>
		///		Comprueba si se puede ejecutar un comando sobre un plugin
		/// </summary>
		public bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command)
		{
			return DbStudioViewManager.PluginsManager.CheckCanExecutePluginCommand(plugin, viewModel, command);
		}

		/// <summary>
		///		Manager de vistas
		/// </summary>
		public DbStudioViewsManager DbStudioViewManager { get; }

		/// <summary>
		///		ViewModel del documento de detalles seleccionado
		/// </summary>
		public IDetailViewModel SelectedDetailsViewModel
		{
			get { return DbStudioViewManager.PluginsStudioViewModel.SelectedDetailsViewModel; }
		}
	}
}
