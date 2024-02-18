namespace Bau.Libraries.LibAiImageGeneration;

/// <summary>
///		Controlador del generador de imágenes
/// </summary>
public class ImageGenerationManager
{
	// Eventos públicos
	public event EventHandler<EventArguments.GenerationProgressArgs>? Progress;

	public ImageGenerationManager()
	{
		AiProvidersManager = new Controllers.AiProvidersManager(this);
	}

	/// <summary>
	///		Añade un proveedor
	/// </summary>
	public void AddProvider(string name, Domain.Interfaces.IImageGeneration imageGeneration)
	{
		AiProvidersManager.Add(name, imageGeneration);
	}

	/// <summary>
	///		Conecta los proveedores
	/// </summary>
	public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
	{
		//? Quitar el valor de retorno
		// Conecta los proveedores
		await AiProvidersManager.ConnectAsync(cancellationToken);
		// Devuelve el valor que indica si se ha conectado
		return true;
	}

	/// <summary>
	///		Envía un mensaje de generación de imágenes y obtiene la respuesta
	/// </summary>
	public async Task<Domain.Models.Results.PromptResultModel> PromptAsync(Domain.Models.Requests.PromptRequestModel prompt, CancellationToken cancellationToken)
	{
		return await AiProvidersManager.PromptAsync(prompt, cancellationToken);
	}

	/// <summary>
	///		Lanza el evento de finalización de generación
	/// </summary>
	internal void RaiseProgressEvent(Models.GenerationModel generation)
	{
		Progress?.Invoke(this, new EventArguments.GenerationProgressArgs(generation));
	}

	/// <summary>
	///		Manager de proveedores
	/// </summary>
	public Controllers.AiProvidersManager AiProvidersManager { get; }

	/// <summary>
	///		Indica si está conectado
	/// </summary>
	public bool Connected { get; private set; }
}