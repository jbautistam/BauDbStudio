using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.FileTools.Plugin.Controllers;

/// <summary>
///		Controlador para las herramientas de archivos
/// </summary>
public class FileToolsController : ViewModel.Controllers.IFileToolsController
{
	public FileToolsController(FileToolsPlugin fileToolsPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		FileToolsPlugin = fileToolsPlugin;
		PluginController = pluginController;
	}

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel)
	{
		// Abre la ventana
		switch (detailsViewModel)
		{
			case ViewModel.PatternsFile.PatternFileViewModel viewModel:
					FileToolsPlugin.AppViewsController.OpenDocument(new Views.PatternFile.PatternFileView(viewModel), viewModel);
				break;
		}
		// Devuelve el valor predeterminado
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
				case ViewModel.Multimedia.MediaFileViewModel viewModel:
						FileToolsPlugin.AppViewsController.OpenNoModalDialog(new Views.Multimedia.MediaFileView(viewModel), true);
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public FileToolsPlugin FileToolsPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
