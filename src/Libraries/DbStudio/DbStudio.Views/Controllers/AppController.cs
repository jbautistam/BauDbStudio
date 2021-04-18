using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.DbStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	public class AppController : PluginsStudio.ViewModels.Base.Controllers.IAppController
	{
		public AppController(DbStudioController dbStudioController)
		{
			DbStudioController = dbStudioController;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailsViewModel)
		{
			// Abre la ventana
			switch (detailsViewModel)
			{
				case ViewModels.Solutions.Details.Connections.ExecuteFilesViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Connections.ExecuteFilesView(viewModel), viewModel);
					break;
				case ViewModels.Solutions.Details.Files.Structured.BaseFileViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Files.DataTableFileView(viewModel), viewModel);
					break;
			}
			// Devuelve el valor predeterminado
			return SystemControllerEnums.ResultType.Yes;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(BaseDialogViewModel dialogViewModel)
		{
			SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

				// Abre la ventana
				switch (dialogViewModel)
				{
					case ViewModels.Solutions.Details.Cloud.StorageViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Cloud.StorageView(viewModel));
						break;
					case ViewModels.Solutions.Details.Connections.ConnectionViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Connections.ConnectionView(viewModel));
						break;
					case ViewModels.Solutions.Details.Deployments.DeploymentViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Deployments.DeploymentView(viewModel));
						break;
					case ViewModels.Solutions.Details.Files.Structured.CsvFilePropertiesViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Files.CsvFilePropertiesView(viewModel));
						break;
					case ViewModels.Solutions.Details.Files.Structured.ParquetFilePropertiesViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Files.ParquetFilePropertiesView(viewModel));
						break;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Controlador principal de la aplicación
		/// </summary>
		public DbStudioController DbStudioController { get; }
	}
}
