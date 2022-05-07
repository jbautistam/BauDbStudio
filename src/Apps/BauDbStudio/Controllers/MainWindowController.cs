using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.Views.Wpf.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador principal
	/// </summary>
	public class MainWindowController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IMainWindowController
	{
		public MainWindowController(DbStudioViewsManager dbStudioViewsManager)
		{
			DbStudioViewsManager = dbStudioViewsManager;
			HostController = new HostController(DbStudioViewsManager.AppName, DbStudioViewsManager.MainWindow);
			HostHelperController = new HostHelperController(DbStudioViewsManager.MainWindow);
			Logger = new Libraries.LibLogger.Core.LogManager();
		}

		/// <summary>
		///		Abre el explorador de Windows sobre un directorio
		/// </summary>
		public void OpenExplorer(string path)
		{
			Libraries.LibSystem.Files.WindowsFiles.ExecuteApplication("explorer.exe", path);
		}

		/// <summary>
		///		Abre el browser predeterminado sobre una URL
		/// </summary>
		public void OpenWindowsWebBrowser(Uri uri)
		{
			Libraries.LibSystem.Files.WindowsFiles.OpenBrowser(uri.ToString());
		}

		/// <summary>
		///		Obtiene el viewModel activo de detalles
		/// </summary>
		public IDetailViewModel GetActiveDetails()
		{
			return DbStudioViewsManager.MainWindow.GetActiveDetails();
		}

		/// <summary>
		///		Obtiene la lista de viewmodel de detalles abiertos
		/// </summary>
		public List<IDetailViewModel> GetOpenedDetails()
		{
			return DbStudioViewsManager.MainWindow.GetOpenedDetails();
		}

		/// <summary>
		///		Modifica el TabId de un documento abierto
		/// </summary>
		public void UpdateTabId(string oldTabId, string newTabId, string newHeader)
		{
			DbStudioViewsManager.MainWindow.UpdateTabId(oldTabId, newTabId, newHeader);
		}

		/// <summary>
		///		Cierra la ventana de un documento abierto
		/// </summary>
		public void CloseWindow(string tabId)
		{
			DbStudioViewsManager.MainWindow.CloseTab(tabId);
		}

		/// <summary>
		///		Muestra una notificación: si está marcada como error o si la configuración así lo permite
		/// </summary>
		public void ShowNotification(SystemControllerEnums.NotificationType type, string title, string message)
		{
			if (DbStudioViewsManager.ConfigurationController.ShowWindowNotifications || type == SystemControllerEnums.NotificationType.Error)
				DbStudioViewsManager.MainWindowsController.HostController.SystemController.ShowNotification(type, title, message, TimeSpan.FromSeconds(5));
			else
				DbStudioViewsManager.MainWindowsController.Logger.Default.LogItems.Add(new Libraries.LibLogger.Models.Log.LogModel(null, Libraries.LibLogger.Models.Log.LogModel.LogType.Info,
																																   title + ". " + message));
		}

		/// <summary>
		///		Comprueba si en el portapapeles hay alguna imagen
		/// </summary>
		public bool ClipboardContainImage()
		{
			return new Helpers.ClipboardHelper().ContainsImage();
		}

		/// <summary>
		///		Graba la imagen del portapapeles
		/// </summary>
		public bool SaveClipboardImage(string fileName)
		{
			bool saved = false;

				// Graba la imagen
				try
				{
					saved = new Helpers.ClipboardHelper().SaveImage(fileName);
				}
				catch (Exception exception)
				{
					Logger.Default.LogItems.Error("Error al grabar el archivo del portapapeles", exception);
				}
				// Devuelve el valor que indica si se ha grabado
				return saved;
		}

		/// <summary>
		///		Manager de vistas principal
		/// </summary>
		internal DbStudioViewsManager DbStudioViewsManager { get; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public IHostController HostController { get; }

		/// <summary>
		///		Controlador de Windows
		/// </summary>
		public HostHelperController HostHelperController { get; }

		/// <summary>
		///		Logger
		/// </summary>
		public Libraries.LibLogger.Core.LogManager Logger { get; }
	}
}
