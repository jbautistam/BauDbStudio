using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.BlogReader.Views.Controllers
{
	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	public class AppController : PluginsStudio.ViewModels.Base.Controllers.IAppController
	{
		public AppController(BlogReaderPlugin blogReaderPlugin)
		{
			BlogReaderPlugin = blogReaderPlugin;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailsViewModel)
		{
			// Abre la ventana
			switch (detailsViewModel)
			{
				case LibBlogReader.ViewModel.Blogs.BlogSeeNewsViewModel viewModel:
						BlogReaderPlugin.AppViewsController.OpenDocument(new Views.BlogSeeNewsControlView(viewModel), viewModel);
					break;
			}
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
					case LibBlogReader.ViewModel.Blogs.BlogViewModel viewModel:
							result = BlogReaderPlugin.AppViewsController.OpenDialog(new Views.BlogView(viewModel));
						break;
					case LibBlogReader.ViewModel.Blogs.FolderViewModel viewModel:
							result = BlogReaderPlugin.AppViewsController.OpenDialog(new Views.FolderView(viewModel));
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Controlador principal de la aplicación
		/// </summary>
		public BlogReaderPlugin BlogReaderPlugin { get; }
	}
}
