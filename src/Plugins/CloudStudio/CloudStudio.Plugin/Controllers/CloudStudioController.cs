using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.CloudStudio.Plugin.Controllers
{
	/// <summary>
	///		Controlador de CloudStudio
	/// </summary>
	public class CloudStudioController : ViewModels.Controllers.ICloudStudioController
	{
		public CloudStudioController(CloudStudioPlugin cloudStudioViewManager, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			CloudStudioViewManager = cloudStudioViewManager;
			PluginController = pluginController;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailsViewModel)
		{
			// Devuelve el valor predeterminado
			return SystemControllerEnums.ResultType.Yes;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(BaseDialogViewModel dialogViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

				// Abre la ventana
				switch (dialogViewModel)
				{
					case ViewModels.Details.Cloud.StorageViewModel viewModel:
							result = CloudStudioViewManager.AppViewsController.OpenDialog(new Cloud.StorageView(viewModel));
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		ViewManager
		/// </summary>
		public CloudStudioPlugin CloudStudioViewManager { get; }

		/// <summary>
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
