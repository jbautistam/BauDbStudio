using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.PasswordManager.Plugin.Controllers;

/// <summary>
///		Controlador del lector de cómics
/// </summary>
public class PasswordManagerController : ViewModel.Controllers.IPasswordManagerController
{
	public PasswordManagerController(PasswordManagerPlugin passwordManagerPlugin, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
	{
		PasswordManagerPlugin = passwordManagerPlugin;
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
			case ViewModel.Reader.PasswordFileViewModel viewModel:
					PasswordManagerPlugin.AppViewsController.OpenDocument(new Views.PasswordFileView(viewModel), viewModel);
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
				case ViewModel.Generator.GeneratorViewModel viewModel:
						result = PasswordManagerPlugin.AppViewsController.OpenDialog(new Views.GeneratorView(viewModel));
					break;
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		ViewManager
	/// </summary>
	public PasswordManagerPlugin PasswordManagerPlugin { get; }

	/// <summary>
	///		Controlador de plugin
	/// </summary>
	public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
}
