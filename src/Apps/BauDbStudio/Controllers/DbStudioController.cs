using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.Views.Wpf.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.DbStudio.ViewModels.Core.Interfaces;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador principal
	/// </summary>
	public class DbStudioController : Libraries.DbStudio.ViewModels.Controllers.IDbStudioController
	{
		// Eventos públicos
		public event EventHandler<IDetailViewModel> OpenWindowRequired;

		public DbStudioController(AppController appController, string applicationName, MainWindow mainWindow, string appPath)
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
					case Libraries.DbStudio.ViewModels.Solutions.Details.Connections.ConnectionViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Connections.ConnectionView(viewModel));
						break;
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
					case Libraries.DbStudio.ViewModels.Solutions.Details.Cloud.StorageViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Cloud.StorageView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Connections.ConnectionViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Connections.ConnectionView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured.CsvFilePropertiesViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Files.CsvFilePropertiesView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured.ParquetFilePropertiesViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Files.ParquetFilePropertiesView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Deployments.DeploymentViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Deployments.DeploymentView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.CreateTestXmlViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.CreateTestXmlView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.CreateValidationScriptsViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.CreateValidationScriptView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.CreateImportFilesScriptViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.CreateImportFilesScriptView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.ExportDatabaseViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.ExportDatabaseView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.CreateSchemaXmlViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.CreateSchemaXmlView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Tools.CreateFileViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Tools.CreateFileView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Relations.DimensionRelationViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Reporting.Details.Relations.DimensionRelationView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Tools.CreateSchemaReportingXmlViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Reporting.Tools.CreateSchemaReportingXmlView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Tools.CreateScriptsSqlReportingViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Reporting.Tools.CreateReportingSqlView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries.ListReportColumnFilterViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Reporting.Queries.ListFilterColumnView(viewModel));
						break;
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
				AppController.SparkSolutionController.HostController.SystemController.ShowNotification(type, title, message, TimeSpan.FromSeconds(5));
			else
				AppController.SparkSolutionController.Logger.Default.LogItems.Add(new Libraries.LibLogger.Models.Log.LogModel(null, Libraries.LibLogger.Models.Log.LogModel.LogType.Info,
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
