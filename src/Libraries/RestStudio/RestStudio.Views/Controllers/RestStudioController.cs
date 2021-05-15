using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;

namespace Bau.Libraries.RestStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de RestStudio
	/// </summary>
	public class RestStudioController : ViewModels.Controllers.IRestStudioController
	{
		public RestStudioController(RestStudioViewManager restStudioViewManager, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			RestStudioViewManager = restStudioViewManager;
			PluginController = pluginController;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel)
		{
			// Abre la ventana
			//switch (detailsViewModel)
			//{
			//	case ViewModels.Details.Connections.ExecuteFilesViewModel viewModel:
			//			DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Connections.ExecuteFilesView(viewModel), viewModel);
			//		break;
			//}
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
					case ViewModels.Solution.RestApiViewModel viewModel:
							result = RestStudioViewManager.AppViewsController.OpenDialog(new Solution.RestView(viewModel));
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		ViewManager
		/// </summary>
		public RestStudioViewManager RestStudioViewManager { get; }

		/// <summary>
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
