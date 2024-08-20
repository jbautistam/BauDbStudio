using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

/// <summary>
///		Interface para los controladores de plugin
/// </summary>
public interface IMainWindowController
{
	/// <summary>
	///		Abre el explorador sobre un directorio
	/// </summary>
	void OpenExplorer(string path);

	/// <summary>
	///		Abre el browser predeterminado sobre una URL
	/// </summary>
	void OpenWindowsWebBrowser(Uri uri);

	/// <summary>
	///		Obtiene la ventana de detalles activa
	/// </summary>
	Interfaces.IDetailViewModel? GetActiveDetails();

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
	///		Copia un valor al portapapeles
	/// </summary>
	void CopyToClipboard(object value);

	/// <summary>
	///		Comprueba si en el portapapeles hay una imagen
	/// </summary>
	bool ClipboardContainImage();

	/// <summary>
	///		Muestra el cursor de espera
	/// </summary>
	void ShowWaitingCursor();

	/// <summary>
	///		Oculta el cursor de espera
	/// </summary>
	void HideWaitingCursor();

	/// <summary>
	///		Graba la imagen del portapapeles
	/// </summary>
	bool SaveClipboardImage(string fileName);

	/// <summary>
	///		Encola un proceso
	/// </summary>
	Task EnqueueProcessAsync(ProcessModel process, CancellationToken cancellationToken);

	/// <summary>
	///		Controlador principal
	/// </summary>
	//TODO: esto se debería poder eliminar
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
	///		Controlador de log (Microsoft Logger)
	/// </summary>
	ILogger<IMainWindowController> Logger { get; }
}
