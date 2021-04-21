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
		// Eventos públicos
		public event EventHandler<IDetailViewModel> OpenWindowRequired;

		public MainWindowController(AppController appController, string applicationName, MainWindow mainWindow, string appPath)
		{
			// Asigna las propiedades
			AppController = appController;
			HostController = new HostController(applicationName, mainWindow);
			HostHelperController = new HostHelperController(mainWindow);
			MainWindow = mainWindow;
			Logger = new Libraries.LibLogger.Core.LogManager();
			AppName = applicationName;
			// Directorio de aplicación
			AppPath = appPath;
			// Crea el directorio de aplicación
			Libraries.LibHelper.Files.HelperFiles.MakePath(appPath);
		}

		/// <summary>
		///		Abre una ventana de detalles
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.Yes;

				// Muestra la ventana adecuada
				switch (detailViewModel)
				{
					default:
							OpenWindowRequired?.Invoke(this, detailViewModel);
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(Libraries.BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

				// Muestra la ventana adecuada
				switch (dialogViewModel)
				{
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Abre el explorador de Windows sobre un directorio
		/// </summary>
		public void OpenExplorer(string path)
		{
			Libraries.LibSystem.Files.WindowsFiles.ExecuteApplication("explorer.exe", path);
		}

		/// <summary>
		///		Obtiene el viewModel activo de detalles
		/// </summary>
		public IDetailViewModel GetActiveDetails()
		{
			return MainWindow.GetActiveDetails();
		}

		/// <summary>
		///		Obtiene la lista de viewmodel de detalles abiertos
		/// </summary>
		public List<IDetailViewModel> GetOpenedDetails()
		{
			return MainWindow.GetOpenedDetails();
		}

		/// <summary>
		///		Modifica el TabId de un documento abierto
		/// </summary>
		public void UpdateTabId(string oldTabId, string newTabId, string newHeader)
		{
			MainWindow.UpdateTabId(oldTabId, newTabId, newHeader);
		}

		/// <summary>
		///		Cierra la ventana de un documento abierto
		/// </summary>
		public void CloseWindow(string tabId)
		{
			MainWindow.CloseTab(tabId);
		}

		/// <summary>
		///		Obtiene el nombre de la consola de ejecución de proyectos ETL
		/// </summary>
		public string GetEtlConsoleFileName()
		{
			return AppController.ConfigurationController.ConsoleExecutable;
		}

		/// <summary>
		///		Muestra una notificación: si está marcada como error o si la configuración así lo permite
		/// </summary>
		public void ShowNotification(SystemControllerEnums.NotificationType type, string title, string message)
		{
			if (AppController.ConfigurationController.ShowWindowNotifications || type == SystemControllerEnums.NotificationType.Error)
				AppController.MainWindowController.HostController.SystemController.ShowNotification(type, title, message, TimeSpan.FromSeconds(5));
			else
				AppController.MainWindowController.Logger.Default.LogItems.Add(new Libraries.LibLogger.Models.Log.LogModel(null, Libraries.LibLogger.Models.Log.LogModel.LogType.Info,
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
					Logger.Default.LogItems.Error("Error al graba el archivo del portapapeles", exception);
				}
				// Devuelve el valor que indica si se ha grabado
				return saved;
		}

		/// <summary>
		///		Controlador de aplicación
		/// </summary>
		internal AppController AppController { get; }

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

		/// <summary>
		///		Ventana principal
		/// </summary>
		internal MainWindow MainWindow { get; }

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		public string AppName { get; }

		/// <summary>
		///		Directorio de aplicación
		/// </summary>
		public string AppPath { get; }
	}
}
