using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.ChessDataBase.Plugin.Controllers;

/// <summary>
///		Controlador de motor de bases de datos de ajedrez
/// </summary>
public class ChessDataBaseController : ViewModels.Controllers.IChessDataBaseController
{
	public ChessDataBaseController(ChessDataBasePlugin plugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		ChessDataBasePlugin = plugin;
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
			case ViewModels.Games.GameBoardPgnViewModel viewModel:
					ChessDataBasePlugin.AppViewsController.OpenDocument(new Views.ChessBoard.ChessboardPgnView(viewModel), viewModel);
				break;
			case ViewModels.Games.GameBoardViewModel viewModel:
					ChessDataBasePlugin.AppViewsController.OpenDocument(new Views.ChessBoard.ChessboardGameView(viewModel), viewModel);
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
			//switch (dialogViewModel)
			//{
			//	case ComicsReader.ViewModel.Blogs.BlogViewModel viewModel:
			//			result = ComicsReaderPlugin.AppViewsController.OpenDialog(new Views.BlogView(viewModel));
			//		break;
			//}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public ChessDataBasePlugin ChessDataBasePlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
