using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.StructuredFilesStudio.Views.Controllers;

/// <summary>
///		Controlador de DbStudio
/// </summary>
public class StructuredFilesStudioController : ViewModels.Controllers.IStructuredFilesStudioController
{
	public StructuredFilesStudioController(StructuredFilesStudioViewManager dbStudioViewManager, 
										  PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		DbStudioViewManager = dbStudioViewManager;
		PluginController = pluginController;
	}

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(IDetailViewModel detailsViewModel)
	{
		// Abre la ventana
		switch (detailsViewModel)
		{
			case ViewModels.Details.Files.BaseFileViewModel viewModel:
					DbStudioViewManager.AppViewsController.OpenDocument(new Files.DataTableFileView(viewModel), viewModel);
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
				case ViewModels.Details.Files.CsvFilePropertiesViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Files.CsvFilePropertiesView(viewModel));
					break;
				case ViewModels.Details.Files.ParquetFilePropertiesViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Files.ParquetFilePropertiesView(viewModel));
					break;
				case ViewModels.Details.Filters.ListFileFilterViewModel viewModel:
						result = DbStudioViewManager.AppViewsController.OpenDialog(new Files.ListFilterView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public StructuredFilesStudioViewManager DbStudioViewManager { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
