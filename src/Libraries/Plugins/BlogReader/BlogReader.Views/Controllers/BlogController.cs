using System;

namespace Bau.Libraries.BlogReader.Views.Controllers
{
	/// <summary>
	///		Controlador de blogs
	/// </summary>
	public class BlogController : LibBlogReader.ViewModel.Controllers.IBlogReaderController
	{
		public BlogController(BlogReaderPlugin blogReaderPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			BlogReaderPlugin = blogReaderPlugin;
			AppController = new AppController(BlogReaderPlugin);
			PluginController = pluginController;
		}

		/// <summary>
		///		ViewManager
		/// </summary>
		public BlogReaderPlugin BlogReaderPlugin { get; }

		/// <summary>
		///		Controlador de vistas de la aplicación
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IAppController AppController { get; }

		/// <summary>
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
