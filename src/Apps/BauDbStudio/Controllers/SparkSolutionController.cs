using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.Views.Wpf.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador principal
	/// </summary>
	public class SparkSolutionController : Libraries.DbStudio.ViewModels.Controllers.ISparkSolutionController
	{
		// Eventos públicos
		public event EventHandler<IDetailViewModel> OpenWindowRequired;

		public SparkSolutionController(string applicationName, MainWindow mainWindow, string appPath)
		{
			// Asigna las propiedades
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
					case Libraries.DbStudio.ViewModels.Solutions.Details.Connections.ConnectionParametersExecutionViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Connections.ConnectionParametersView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.Files.CsvFilePropertiesViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.Files.CsvFilePropertiesView(viewModel));
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
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.ExecuteEtlConsoleViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.ExecuteEtlConsoleView(viewModel));
						break;
					case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.ExportDatabaseViewModel viewModel:
							result = HostHelperController.ShowDialog(MainWindow, new Views.EtlProjects.ExportDatabaseView(viewModel));
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
			return MainWindow.MainController.ConfigurationController.ConsoleExecutable;
		}

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
