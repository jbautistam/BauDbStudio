using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BlogReader.Views.Controllers;

/// <summary>
///		Controlador de blogs
/// </summary>
public class BlogController : LibBlogReader.ViewModel.Controllers.IBlogReaderController
{
	public BlogController(BlogReaderPlugin blogReaderPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		BlogReaderPlugin = blogReaderPlugin;
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
	public SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel)
	{
		SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

			// Abre la ventana
			switch (dialogViewModel)
			{
				case LibBlogReader.ViewModel.Blogs.BlogViewModel viewModel:
						result = BlogReaderPlugin.AppViewsController.OpenDialog(new Views.BlogView(viewModel));
					break;
				case LibBlogReader.ViewModel.Blogs.HyperlinkViewModel viewModel:
						result = BlogReaderPlugin.AppViewsController.OpenDialog(new Views.HyperlinkView(viewModel));
					break;
				case LibBlogReader.ViewModel.Blogs.FolderViewModel viewModel:
						result = BlogReaderPlugin.AppViewsController.OpenDialog(new Views.FolderView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public BlogReaderPlugin BlogReaderPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
