using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.ToDoManager.ViewModel.Configuration;

/// <summary>
///		ViewModel de la configuración
/// </summary>
public class ConfigurationViewModel : BaseObservableObject
{
	// Variables privadas
	private bool _hookGlobal;

	public ConfigurationViewModel(ToDoManagerViewModel mainViewModel)
	{
		MainViewModel =	mainViewModel;
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		HookGlobal = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(nameof(ToDoManagerViewModel), nameof(HookGlobal)).GetBool();
	}

	/// <summary>
	///		Graba los datos de configuración
	/// </summary>
	public void Save()
	{
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(nameof(ToDoManagerViewModel), nameof(HookGlobal), HookGlobal.ToString());
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Indica si se debe asociar al hook del teclado
	/// </summary>
	public bool HookGlobal
	{
		get { return _hookGlobal; }
		set { CheckProperty(ref _hookGlobal, value); }
	}
}
