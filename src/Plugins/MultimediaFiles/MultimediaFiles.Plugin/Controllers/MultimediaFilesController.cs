using System;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.MultimediaFiles.ViewModel.Reader;

namespace Bau.Libraries.MultimediaFiles.Plugin.Controllers
{
	/// <summary>
	///		Controlador del lector de cómics
	/// </summary>
	public class MultimediaFilesController : ViewModel.Controllers.IMultimediaFilesController
	{
		public MultimediaFilesController(MultimediaFilesPlugin comicReaderPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			MultimediaFilesPlugin = comicReaderPlugin;
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
			//	case ViewModel.Reader.MediaFileViewModel viewModel:
			//			MultimediaFilesPlugin.AppViewsController.OpenDocument(new Views.ComicContentView(viewModel), viewModel);
			//		break;
			//}
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
				//	case ComicsReader.ViewModel.Blogs.BlogViewModel viewModel:
				//			result = ComicsReaderPlugin.AppViewsController.OpenDialog(new Views.BlogView(viewModel));
				//		break;
				//}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		ViewManager
		/// </summary>
		public MultimediaFilesPlugin MultimediaFilesPlugin { get; }

		/// <summary>
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
