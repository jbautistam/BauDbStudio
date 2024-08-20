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
	private string _stableHordeApiUrl = default!, _stableHordeApiKey = default!, _ollamaUrl = default!;
	private int? _ollamaTimeout = 3;

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
		OllamaUrl = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(OllamaUrl));
		OllamaTimeout = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(OllamaTimeout)).GetInt(3);
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
		else if (string.IsNullOrWhiteSpace(OllamaUrl))
			error = "Enter the Ollama Url (default http://localhost:11434)";
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
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(OllamaUrl), OllamaUrl);
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(OllamaTimeout), 
																								OllamaTimeout.ToString());
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

	/// <summary>
	///		Url de Ollama
	/// </summary>
	public string OllamaUrl
	{
		get
		{
			// Asigna la URL predeterminada
			if (string.IsNullOrWhiteSpace(_ollamaUrl) || !Uri.TryCreate(_ollamaUrl, UriKind.Absolute, out Uri? _))
				_ollamaUrl = "http://localhost:11434";
			// Devuelve la URL
			return _ollamaUrl;
		}
		set { CheckProperty(ref _ollamaUrl, value); }
	}

	/// <summary>
	///		Timeout de las llamadas a Ollama
	/// </summary>
	public int OllamaTimeout
	{
		get { return _ollamaTimeout ?? 3; }
		set { _ollamaTimeout = value; }
	}
}
