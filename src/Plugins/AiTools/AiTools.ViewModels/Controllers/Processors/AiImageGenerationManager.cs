namespace Bau.Libraries.AiTools.ViewModels.Controllers.Processors;

/// <summary>
///		Manager del generador de imágenes
/// </summary>
internal class AiImageGenerationManager
{
	internal AiImageGenerationManager(AiToolsViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
		ImageGenerationManager = new LibAiImageGeneration.ImageGenerationManager();
	}

	/// <summary>
	///		Inicializa el generador
	/// </summary>
	internal async Task InitializeAsync(CancellationToken cancellationToken)
	{
		if (!IsInitialized)
		{
			// Inicializa el controlador
			if (!ImageGenerationManager.AiProvidersManager.Providers.ContainsKey("Horde"))
				ImageGenerationManager.AddProvider("Horde", new LibStableHorde.Api.StableHordeManager(new Uri(MainViewModel.ConfigurationViewModel.StableHordeApiUrl), 
																									  MainViewModel.ConfigurationViewModel.StableHordeApiKey));
			if (!ImageGenerationManager.AiProvidersManager.Providers.ContainsKey("Fake"))
				ImageGenerationManager.AddProvider("Fake", new LibFakeAi.Api.FakeAiManager());
			await ImageGenerationManager.ConnectAsync(cancellationToken);
			// Indica que se ha inicializado
			IsInitialized = true;
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	internal AiToolsViewModel MainViewModel { get; }

	/// <summary>
	///		Manager de generación
	/// </summary>
	internal LibAiImageGeneration.ImageGenerationManager ImageGenerationManager { get; }

	/// <summary>
	///		Indica si se ha inicializado el generador
	/// </summary>
	internal bool IsInitialized { get; private set; }
}
