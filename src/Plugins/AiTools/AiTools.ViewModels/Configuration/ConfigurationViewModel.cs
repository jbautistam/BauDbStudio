using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.AiTools.ViewModels.Configuration;

/// <summary>
///		ViewModel para la configuración
/// </summary>
public class ConfigurationViewModel : BauMvvm.ViewModels.BaseObservableObject
{   
	// Constantes privadas
	private const string ApplicationName = "AiTools";
	// Variables privadas
	private string _stableHordeApiUrl = default!, _stableHordeApiKey = default!;

	public ConfigurationViewModel(AiToolsViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Carga los datos de configuración
	/// </summary>
	internal void Load()
	{
		StableHordeApiUrl = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(StableHordeApiUrl));
		StableHordeApiKey = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(StableHordeApiKey));
	}

	/// <summary>
	///		Comprueba si los datos son correctos
	/// </summary>
	public bool ValidateData(out string error)
	{ 
		// Inicializa los argumentos de salida
		error = string.Empty;
		// Comprueba los datos
		if (string.IsNullOrWhiteSpace(StableHordeApiUrl))
			error = "Enter the Stable Horde Url (default https://stablehorde.net)";
		else if (string.IsNullOrWhiteSpace(StableHordeApiKey))
			error = "Enter the Stable Horde Api Key (default 0000000000)";
		// Devuelve el valor que indica si los datos son correctos
		return error.IsEmpty();
	}

	/// <summary>
	///		Graba los datos (y actualiza el árbol)
	/// </summary>
	public void Save()
	{
		// Grava la configuración
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(StableHordeApiUrl), 
																								StableHordeApiUrl);
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(StableHordeApiKey), 
																								StableHordeApiKey);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public AiToolsViewModel MainViewModel { get; }

	/// <summary>
	///		Url Api Stable Horde
	/// </summary>
	public string StableHordeApiUrl
	{
		get 
		{ 
			if (string.IsNullOrWhiteSpace(_stableHordeApiUrl) || !Uri.TryCreate(_stableHordeApiUrl, UriKind.Absolute, out Uri? _))
				_stableHordeApiUrl = "https://stablehorde.net";
			return _stableHordeApiUrl; 
		}
		set { CheckProperty(ref _stableHordeApiUrl, value); }
	}

	/// <summary>
	///		Api key de StableHorde
	/// </summary>
	public string StableHordeApiKey
	{
		get 
		{ 
			if (string.IsNullOrWhiteSpace(_stableHordeApiKey))
				_stableHordeApiKey = "0000000000";
			return _stableHordeApiKey; 
		}
		set { CheckProperty(ref _stableHordeApiKey, value); }
	}
}
