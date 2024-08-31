using System.Windows.Threading;
using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.Views.Wpf.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.DbStudio.Controllers;

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
		Logger = (App.MainHost.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory)
							.CreateLogger<Libraries.PluginsStudio.ViewModels.Base.Controllers.IMainWindowController>();
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
	public IDetailViewModel? GetActiveDetails() => DbStudioViewsManager.MainWindow.GetActiveDetails();

	/// <summary>
	///		Obtiene el viewModel del panel activo
	/// </summary>
	public IPaneViewModel? GetActivePane() => DbStudioViewsManager.MainWindow.GetActivePane();

	/// <summary>
	///		Obtiene la lista de viewmodel de detalles abiertos
	/// </summary>
	public List<IDetailViewModel> GetOpenedDetails() => DbStudioViewsManager.MainWindow.GetOpenedDetails();

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
			Logger.LogInformation($"{title}.{message}");
	}

	/// <summary>
	///		Copia un objeto al portapapeles
	/// </summary>
	public void CopyToClipboard(object value)
	{
		new Helpers.ClipboardHelper().SetData(value);
	}

	/// <summary>
	///		Comprueba si en el portapapeles hay alguna imagen
	/// </summary>
	public bool ClipboardContainImage() => new Helpers.ClipboardHelper().ContainsImage();

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
				Logger.LogError(exception, "Error when save the clipboard file");
			}
			// Devuelve el valor que indica si se ha grabado
			return saved;
	}

	/// <summary>
	///		Encola un proceso
	/// </summary>
	public async Task EnqueueProcessAsync(ProcessModel process, CancellationToken cancellationToken)
	{
		 await DbStudioViewsManager.MainWindow.Dispatcher.InvokeAsync
				(async () => await DbStudioViewsManager.PluginsStudioViewModel.TasksQueueListViewModel.EnqueueProcessAsync(process, cancellationToken), 
				 DispatcherPriority.Normal, 
				 cancellationToken);
	}

	/// <summary>
	///		Muestra el cursor de espera
	/// </summary>
	public void ShowWaitingCursor()
	{
		DbStudioViewsManager.MainWindow.Cursor = System.Windows.Input.Cursors.Wait;
	}

	/// <summary>
	///		Oculta el cursor de espera
	/// </summary>
	public void HideWaitingCursor()
	{
		DbStudioViewsManager.MainWindow.Cursor = null;
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
	///		Controlador de log
	/// </summary>
	public ILogger<Libraries.PluginsStudio.ViewModels.Base.Controllers.IMainWindowController> Logger { get; }
}
