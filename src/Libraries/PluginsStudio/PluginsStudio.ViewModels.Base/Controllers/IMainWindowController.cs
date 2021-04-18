using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers
{
	/// <summary>
	///		Interface para los controladores de plugin
	/// </summary>
	public interface IMainWindowController
	{
		/// <summary>
		///		Controlador principal
		/// </summary>
		IHostController HostController { get; }

		/// <summary>
		///		Controlador de diálogos de sistema
		/// </summary>
		IHostDialogsController DialogsController => HostController.DialogsController;

        /// <summary>
        ///     Controlador con funciones del sistema
        /// </summary>
        IHostSystemController SystemController => HostController.SystemController;

        /// <summary>
        ///     Controlador con funciones del sistema asíncrono
        /// </summary>
        IHostSystemControllerAsync SystemControllerAsync => HostController.SystemControllerAsync;

        /// <summary>
        ///     Controlador de mensajes
        /// </summary>
        IHostMessengerController MessengerController => HostController.MessengerController;

		/// <summary>
		///		Controlador de log
		/// </summary>
		LibLogger.Core.LogManager Logger { get; }

		/// <summary>
		///		Nombre de la aplicación
		/// </summary>
		string AppName { get; }

		/// <summary>
		///		Directorio de aplicación
		/// </summary>
		string AppPath { get; }

		/// <summary>
		///		Abre el explorador sobre un directorio
		/// </summary>
		void OpenExplorer(string path);

		/// <summary>
		///		Obtiene la ventana de detalles activa
		/// </summary>
		Interfaces.IDetailViewModel GetActiveDetails();

		/// <summary>
		///		Obtiene la lista de ventanas de detalles abiertas
		/// </summary>
		List<Interfaces.IDetailViewModel> GetOpenedDetails();

		/// <summary>
		///		Indica a la ventana principal que cambie el Id de un documento
		/// </summary>
		void UpdateTabId(string oldTabId, string newTabId, string newHeader);

		/// <summary>
		///		Indica a la ventana principal que cierre un documento
		/// </summary>
		void CloseWindow(string tabId);

		/// <summary>
		///		Muestra una notificación (sólo si la configuración lo permite)
		/// </summary>
		void ShowNotification(SystemControllerEnums.NotificationType type, string title, string message);

		/// <summary>
		///		Comprueba si en el portapapeles hay una imagen
		/// </summary>
		bool ClipboardContainImage();

		/// <summary>
		///		Graba la imagen del portapapeles
		/// </summary>
		bool SaveClipboardImage(string fileName);
	}
}
