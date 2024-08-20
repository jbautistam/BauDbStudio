using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.DbStudio.Views.Controllers;

/// <summary>
///		Controlador de DbStudio
/// </summary>
public class DbStudioController : ViewModels.Controllers.IDbStudioController
{
	public DbStudioController(DbStudioViewManager dbStudioViewManager, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		DbStudioViewManager = dbStudioViewManager;
		PluginController = pluginController;
	}

	/// <summary>
	///		Obtiene el nombre del archivo de la consola de ejecución de proyectos ETL
	/// </summary>
	public string GetEtlConsoleFileName() => string.Empty;

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailsViewModel)
	{
		// Abre la ventana
		switch (detailsViewModel)
		{
			case ViewModels.Details.Connections.ExecuteFilesViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Connections.ExecuteFilesView(viewModel), viewModel);
				break;
			case ViewModels.Details.Files.ScriptFileViewModel viewModel:
					PluginController.HostPluginsController.OpenEditor(viewModel);
				break;
			case ViewModels.Details.Queries.ExecuteQueryViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Queries.ExecuteQueryView(viewModel), viewModel);
				break;
			case ViewModels.Details.Reporting.DataSources.DataSourceSqlViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.DataSources.DataSourceSqlView(viewModel), viewModel);
				break;
			case ViewModels.Details.Reporting.DataSources.DataSourceTableViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.DataSources.DataSourceTableView(viewModel), viewModel);
				break;
			case ViewModels.Details.Reporting.Dimension.DimensionViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.Dimensions.DimensionView(viewModel), viewModel);
				break;
			case ViewModels.Details.Reporting.Reports.ReportViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Details.Reports.ReportView(viewModel), viewModel);
				break;
			case ViewModels.Details.Reporting.Queries.ReportQueryViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Reporting.Queries.ReportView(viewModel), viewModel);
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
				case ViewModels.Details.Connections.ConnectionViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Connections.ConnectionView(viewModel));
					break;
				case ViewModels.Details.EtlProjects.CreateTestXmlViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateTestXmlView(viewModel));
					break;
				case ViewModels.Details.EtlProjects.CreateValidationScriptsViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateValidationScriptView(viewModel));
					break;
				case ViewModels.Details.EtlProjects.CreateImportFilesScriptViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.CreateImportFilesScriptView(viewModel));
					break;
				case ViewModels.Details.EtlProjects.ExportDatabaseViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.ExportDatabaseView(viewModel));
					break;
				case ViewModels.Details.EtlProjects.ImportDatabaseViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new EtlProjects.ImportDatabaseView(viewModel));
					break;
				case ViewModels.Details.Reporting.Relations.DimensionRelationViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Details.Relations.DimensionRelationView(viewModel));
					break;
				case ViewModels.Details.Reporting.Tools.CreateScriptsSqlReportingViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Tools.CreateReportingSqlView(viewModel));
					break;
				case ViewModels.Details.Reporting.Dimension.DimensionChildViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Details.Dimensions.DimensionChildView(viewModel));
					break;
				case ViewModels.Details.Reporting.Queries.ListReportColumnFilterViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Reporting.Queries.ListFilterColumnView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public DbStudioViewManager DbStudioViewManager { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
