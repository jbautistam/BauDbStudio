using System;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Files;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio.Controllers.PluginsStudio.Controllers
{
	/// <summary>
	///		Controlador del host de plugins
	/// </summary>
	public class HostPluginsController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IHostPluginsController
	{
		public HostPluginsController(PluginsStudioViewManager pluginsStudioViewManager, PluginsStudioController pluginsStudioController)
		{
			PluginsStudioViewManager = pluginsStudioViewManager;
			PluginsStudioController = pluginsStudioController;
		}

		/// <summary>
		///		Añade el archivo utilizado a la lista
		/// </summary>
		public void AddFileUsed(string fileName)
		{
			PluginsStudioController.PluginsStudioViewManager.PluginsStudioViewModel.LastFilesViewModel.Add(fileName);
		}

		/// <summary>
		///		Abre el editor de un archivo
		/// </summary>
		public void OpenFile(string fileName)
		{
			PluginsStudioController.PluginsStudioViewManager.OpenFile(fileName);
		}

		/// <summary>
		///		Abre un editor sobre un archivo de texto
		/// </summary>
		public void OpenEditor(BaseTextFileViewModel viewModel)
		{
			PluginsStudioController.AppController.OpenWindow(viewModel);
		}

		/// <summary>
		///		Abre un navegador sobre una URL
		/// </summary>
		public void OpenWebBrowser(string url)
		{
			PluginsStudioController.PluginsStudioViewManager.PluginsStudioViewModel.OpenWebBrowser(url);
		}

		/// <summary>
		///		Actualiza el árbol de archivos
		/// </summary>
		public void RefreshFiles()
		{
			PluginsStudioController.PluginsStudioViewManager.PluginsStudioViewModel.RefreshFiles();
		}

		/// <summary>
		///		Ejecuta un comando sobre un plugin
		/// </summary>
		public void ExecutePluginCommand(string plugin, string viewModel, string command)
		{
			PluginsStudioController.PluginsStudioViewManager.PluginsManager.ExecutePluginCommand(plugin, viewModel, command);
		}

		/// <summary>
		///		Comprueba si se puede ejecutar un comando sobre un plugin
		/// </summary>
		public bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command)
		{
			return PluginsStudioViewManager.PluginsManager.CheckCanExecutePluginCommand(plugin, viewModel, command);
		}

		/// <summary>
		///		Manager de vistas
		/// </summary>
		public PluginsStudioViewManager PluginsStudioViewManager { get; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public PluginsStudioController PluginsStudioController { get; }

		/// <summary>
		///		ViewModel del documento de detalles seleccionado
		/// </summary>
		public IDetailViewModel SelectedDetailsViewModel
		{
			get { return PluginsStudioController.PluginsStudioViewManager.PluginsStudioViewModel.SelectedDetailsViewModel; }
		}
	}
}
