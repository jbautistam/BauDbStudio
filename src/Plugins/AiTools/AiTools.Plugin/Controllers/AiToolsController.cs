using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.AiTools.Plugin.Controllers;

/// <summary>
///		Controlador del visualizador de las herramientas de IA
/// </summary>
public class AiToolsController : ViewModels.Controllers.IAiToolsController
{
	public AiToolsController(AiToolsPlugin aiToolsPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		AiToolsPlugin = aiToolsPlugin;
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
			case ViewModels.Prompts.PromptFileViewModel viewModel:
					AiToolsPlugin.AppViewsController.OpenDocument(new Views.ImagesPrompt.ImagePromptView(viewModel), viewModel);
				break;
			case ViewModels.TextPrompt.ChatViewModel viewModel:
					AiToolsPlugin.AppViewsController.OpenDocument(new Views.TextPrompt.ChatView(viewModel), viewModel);
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

			//// Abre la ventana
			//switch (dialogViewModel)
			//{
			//	case ViewModels.Prompts.PromptFileViewModel viewModel:
			//			AiToolsPlugin.AppViewsController.OpenNoModalDialog(new Views.MediaFileView(viewModel));
			//		break;
			//}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public AiToolsPlugin AiToolsPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
