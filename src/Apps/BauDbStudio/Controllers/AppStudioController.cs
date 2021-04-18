using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	public class AppStudioController : Libraries.PluginsStudio.Views.Base.Interfaces.IAppViewsController
	{
		// Eventos públicos
		public event EventHandler<EventArguments.OpenDocumentEventArgs> OpenDocumentRequired;

		public AppStudioController(AppController appController)
		{
			AppController = appController;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(System.Windows.Window window)
		{
			return AppController.MainWindowController.HostHelperController.ShowDialog(AppController.MainWindowController.MainWindow, window);
		}

		/// <summary>
		///		Abre un documento
		/// </summary>
		public void OpenDocument(System.Windows.Controls.UserControl userControl, IDetailViewModel detailViewModel)
		{
			OpenDocumentRequired?.Invoke(this, new EventArguments.OpenDocumentEventArgs(userControl, detailViewModel));
		}

		/// <summary>
		///		Controlador de aplicación
		/// </summary>
		internal AppController AppController { get; }
	}
}
