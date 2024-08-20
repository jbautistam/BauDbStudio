using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.JobsProcessor.Plugin.Controllers;

/// <summary>
///		Controlador del procesador de consolas
/// </summary>
public class JobsProcessorController : ViewModel.Controllers.IJobsProcessorController
{
	public JobsProcessorController(JobsProcessorPlugin jobsProcessorPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		JobsProcessorPlugin = jobsProcessorPlugin;
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
			case Bau.Libraries.JobsProcessor.ViewModel.Processor.ExecuteConsoleViewModel viewModel:
					JobsProcessorPlugin.AppViewsController.OpenDocument(new Views.Processor.ExecuteEtlConsoleView(viewModel), viewModel);
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
			//	case JobsProcessor.ViewModel.Blogs.BlogViewModel viewModel:
			//			result = JobsProcessorPlugin.AppViewsController.OpenDialog(new Views.BlogView(viewModel));
			//		break;
			//}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public JobsProcessorPlugin JobsProcessorPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
