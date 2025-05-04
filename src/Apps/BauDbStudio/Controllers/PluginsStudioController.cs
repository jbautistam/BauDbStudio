using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.DbStudio.Controllers;

/// <summary>
///		Controlador de plugins principal
/// </summary>
public class PluginsStudioController : Libraries.PluginsStudio.ViewModels.Controllers.IPluginsStudioController
{
	public PluginsStudioController(DbStudioViewsManager dbStudioViewManager)
	{
		DbStudioViewManager = dbStudioViewManager;
		PluginsController = new PluginsController(dbStudioViewManager);
	}

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(Libraries.PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailViewModel)
	{
		// Abre la ventana
		switch (detailViewModel)
		{
			case Libraries.PluginsStudio.ViewModels.Base.Files.BaseTextFileViewModel viewModel:
					DbStudioViewManager.AppViewController.OpenDocument(new Views.Files.FileTextView(viewModel), viewModel);
				break;
			case Libraries.PluginsStudio.ViewModels.Tools.Web.WebViewModel viewModel:
					DbStudioViewManager.AppViewController.OpenDocument(new Views.Tools.Web.WebExplorerView(viewModel), viewModel);
				break;
		}
		// Devuelve el resultado
		return SystemControllerEnums.ResultType.Yes;
	}

	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	public SystemControllerEnums.ResultType OpenDialog(BaseDialogViewModel dialogViewModel)
	{
		return dialogViewModel switch
					{
						Libraries.PluginsStudio.ViewModels.Tools.CreateFileViewModel viewModel => DbStudioViewManager.AppViewController.OpenDialog(new Views.Files.CreateFileView(viewModel)),
						Libraries.PluginsStudio.ViewModels.Tools.SaveOpenFilesViewModel viewModel => DbStudioViewManager.AppViewController.OpenDialog(new Views.Files.SaveOpenFilesView(viewModel)),
						_ => SystemControllerEnums.ResultType.No,
					};
	}

	/// <summary>
	///		Llama a los diferentes plugins para que actualicen exploradores y ventanas
	/// </summary>
	public void Refresh()
	{
		DbStudioViewManager.PluginsManager.Refresh();
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	public DbStudioViewsManager DbStudioViewManager { get; }

	/// <summary>
	///		Controlador de plugins
	/// </summary>
	public IPluginsController PluginsController { get; }
}
