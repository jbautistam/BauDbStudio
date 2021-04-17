using System;

namespace Bau.Libraries.PluginsStudio.Views.Controllers
{
	/// <summary>
	///		Controlador del host de plugins
	/// </summary>
	public class HostPluginsController : ViewModels.Base.Controllers.IHostPluginsController
	{
		public HostPluginsController(PluginsStudioController pluginsStudioController)
		{
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
			return PluginsStudioController.PluginsStudioViewManager.PluginsManager.CheckCanExecutePluginCommand(plugin, viewModel, command);
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public PluginsStudioController PluginsStudioController { get; }

		/// <summary>
		///		ViewModel del documento de detalles seleccionado
		/// </summary>
		public ViewModels.Base.Interfaces.IDetailViewModel SelectedDetailsViewModel
		{
			get { return PluginsStudioController.PluginsStudioViewManager.PluginsStudioViewModel.SelectedDetailsViewModel; }
		}
	}
}
