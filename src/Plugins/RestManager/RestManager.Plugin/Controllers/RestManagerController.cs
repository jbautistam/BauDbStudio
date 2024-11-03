using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.RestManager.Plugin.Controllers;

/// <summary>
///		Controlador de la ejecución de proyectos REST
/// </summary>
public class RestManagerController : ViewModel.Controllers.IRestManagerController
{
	public RestManagerController(RestManagerPlugin restManagerPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		RestManagerPlugin = restManagerPlugin;
		PluginController = pluginController;
	}

	/// <summary>
	///		Abre una ventana
	/// </summary>
	public SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel)
	{
		// Abre la ventana del editor
		switch (detailsViewModel)
		{
			case ViewModel.Project.RestFileViewModel viewModel:
					RestManagerPlugin.AppViewsController.OpenDocument(new Views.RestFileView(viewModel), viewModel);
				break;
		}
		// Devuelve el valor que indica que se ha abierto correctamente
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
				case ViewModel.Project.Connections.ConnectionViewModel viewModel:
						result = RestManagerPlugin.AppViewsController.OpenDialog(new Views.Connections.ConnectionView(viewModel));
					break;
				case ViewModel.Project.Parameters.ParameterViewModel viewModel:
						result = RestManagerPlugin.AppViewsController.OpenDialog(new Views.ParameterView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public RestManagerPlugin RestManagerPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
