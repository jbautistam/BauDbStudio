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
				case ViewModels.Details.Connections.ExecuteFilesViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Connections.ExecuteFilesView(viewModel), viewModel);
					break;
				case ViewModels.Details.Files.Structured.BaseFileViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Files.DataTableFileView(viewModel), viewModel);
					break;
				case ViewModels.Details.Queries.ExecuteQueryViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Queries.ExecuteQueryView(viewModel), viewModel);
					break;
				case ViewModels.Details.EtlProjects.ExecuteEtlConsoleViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new EtlProjects.ExecuteEtlConsoleView(viewModel), viewModel);
					break;
				case ViewModels.Details.Reporting.DataSources.DataSourceSqlViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.DataSources.DataSourceSqlView(viewModel), viewModel);
					break;
				case ViewModels.Details.Reporting.DataSources.DataSourceTableViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.DataSources.DataSourceTableView(viewModel), viewModel);
					break;
				case ViewModels.Details.Reporting.Dimension.DimensionViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.Dimensions.DimensionView(viewModel), viewModel);
					break;
				case ViewModels.Details.Reporting.Queries.ReportViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Queries.ReportView(viewModel), viewModel);
					break;
				case ViewModels.Details.Reporting.Reports.ReportViewModel viewModel:
						DbStudioController.DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.Reports.ReportView(viewModel), viewModel);
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
					case ViewModels.Details.Cloud.StorageViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Cloud.StorageView(viewModel));
						break;
					case ViewModels.Details.Connections.ConnectionViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Connections.ConnectionView(viewModel));
						break;
					case ViewModels.Details.Deployments.DeploymentViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Deployments.DeploymentView(viewModel));
						break;
					case ViewModels.Details.Files.Structured.CsvFilePropertiesViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Files.CsvFilePropertiesView(viewModel));
						break;
					case ViewModels.Details.Files.Structured.ParquetFilePropertiesViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Files.ParquetFilePropertiesView(viewModel));
						break;
					case ViewModels.Details.EtlProjects.CreateTestXmlViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateTestXmlView(viewModel));
						break;
					case ViewModels.Details.EtlProjects.CreateValidationScriptsViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateValidationScriptView(viewModel));
						break;
					case ViewModels.Details.EtlProjects.CreateImportFilesScriptViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateImportFilesScriptView(viewModel));
						break;
					case ViewModels.Details.EtlProjects.ExportDatabaseViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.ExportDatabaseView(viewModel));
						break;
					case ViewModels.Details.EtlProjects.CreateSchemaXmlViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateSchemaXmlView(viewModel));
						break;
					case ViewModels.Details.Reporting.Relations.DimensionRelationViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Details.Relations.DimensionRelationView(viewModel));
						break;
					case ViewModels.Details.Reporting.Tools.CreateSchemaReportingXmlViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Tools.CreateSchemaReportingXmlView(viewModel));
						break;
					case ViewModels.Details.Reporting.Tools.CreateScriptsSqlReportingViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Tools.CreateReportingSqlView(viewModel));
						break;
					case ViewModels.Details.Reporting.Queries.ListReportColumnFilterViewModel viewModel:
							result = DbStudioController.DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Queries.ListFilterColumnView(viewModel));
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
