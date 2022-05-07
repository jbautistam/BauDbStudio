using System;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.PasswordManager.Plugin.Controllers
{
	/// <summary>
	///		Controlador del lector de cómics
	/// </summary>
	public class PasswordManagerController : ViewModel.Controllers.IPasswordManagerController
	{
		public PasswordManagerController(PasswordManagerPlugin comicReaderPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			ComicReaderPlugin = comicReaderPlugin;
			PluginController = pluginController;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel)
		{
			// Abre la ventana
			switch (detailsViewModel)
			{
				case Bau.Libraries.PasswordManager.ViewModel.Reader.PasswordFileViewModel viewModel:
						ComicReaderPlugin.AppViewsController.OpenDocument(new Views.PasswordFileView(viewModel), viewModel);
					break;
			}
			// Devuelve el valor predeterminado
			return SystemControllerEnums.ResultType.Yes;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

				// Abre la ventana
				//switch (dialogViewModel)
				//{
				//	case PasswordManager.ViewModel.Blogs.BlogViewModel viewModel:
				//			result = PasswordManagerPlugin.AppViewsController.OpenDialog(new Views.BlogView(viewModel));
				//		break;
				//}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		ViewManager
		/// </summary>
		public PasswordManagerPlugin ComicReaderPlugin { get; }

		/// <summary>
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
