using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.ToDoManager.Plugin.Controllers;

/// <summary>
///		Controlador del manager de tareas
/// </summary>
public class ToDoManagerController : ViewModel.Controllers.IToDoManagerController
{
	public ToDoManagerController(ToDoManagerPlugin taskPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		TaskManagerPlugin = taskPlugin;
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
			case ViewModel.Reader.ToDoFileViewModel viewModel:
					TaskManagerPlugin.AppViewsController.OpenDocument(new Views.TodoFileView(viewModel), viewModel);
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
			//	case ViewModel.Generator.GeneratorViewModel viewModel:
			//			result = TaskManagerPlugin.AppViewsController.OpenDialog(new Views.GeneratorView(viewModel));
			//		break;
			//}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public ToDoManagerPlugin TaskManagerPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
