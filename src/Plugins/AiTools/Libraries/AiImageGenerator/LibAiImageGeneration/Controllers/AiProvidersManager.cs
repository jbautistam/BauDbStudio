using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;

namespace Bau.Libraries.LibAiImageGeneration.Controllers;

/// <summary>
///		Manager de proveedores de generación de imágenes de AI
/// </summary>
public class AiProvidersManager
{
	public AiProvidersManager(ImageGenerationManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Añade un proveedor
	/// </summary>
	public void Add(string name, Domain.Interfaces.IImageGeneration imageGeneration)
	{
		if (Providers.ContainsKey(name))
			Providers[name] = new AiProvider(this, name, imageGeneration);
		else
			Providers.Add(name, new AiProvider(this, name, imageGeneration));
	}

	/// <summary>
	///		Conecta los proveedores
	/// </summary>
	public async Task ConnectAsync(CancellationToken cancellationToken)
	{
		foreach (KeyValuePair<string, AiProvider> keyValue in Providers)
			if (!cancellationToken.IsCancellationRequested)
				await keyValue.Value.ConnectAsync(cancellationToken);
	}

	/// <summary>
	///		Ejecuta un prompt
	/// </summary>
	public async Task<Domain.Models.Results.PromptResultModel> PromptAsync(PromptRequestModel prompt, CancellationToken cancellationToken)
	{
		if (Providers.TryGetValue(prompt.Provider, out AiProvider? aiProvider))
			return await aiProvider.PromptAsync(prompt, cancellationToken);
		else
			return new Domain.Models.Results.PromptResultModel(false, null, $"Can't find the provider {prompt.Provider}");
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ImageGenerationManager Manager { get; }

	/// <summary>
	///		Proveedores
	/// </summary>
	public Dictionary<string, AiProvider> Providers { get; } = new(StringComparer.CurrentCultureIgnoreCase);
}