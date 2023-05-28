using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.MultimediaFiles.Plugin.Controllers;

/// <summary>
///		Controlador del visulaizador de archivos multimedia
/// </summary>
public class MultimediaFilesController : ViewModel.Controllers.IMultimediaFilesController
{
	public MultimediaFilesController(MultimediaFilesPlugin multimediaFilesPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		MultimediaFilesPlugin = multimediaFilesPlugin;
		PluginController = pluginController;
	}

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel)
	{
		return SystemControllerEnums.ResultType.Yes;
	}

	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	public SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel)
	{
		SystemControllerEnums.ResultType result = SystemControllerEnums.ResultType.No;

			// Abre la ventana
			switch (dialogViewModel)
			{
				case ViewModel.Reader.MediaFileViewModel viewModel:
						MultimediaFilesPlugin.AppViewsController.OpenNoModalDialog(new Views.MediaFileView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public MultimediaFilesPlugin MultimediaFilesPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
