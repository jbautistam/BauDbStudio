using Bau.Libraries.LibStableHorde.Api;
using Bau.Libraries.LibStableHorde.Api.Dtos;

namespace StableHorde.Console.Controller;

/// <summary>
///		Controlador del generador de imágenes de StableHorde
/// </summary>
public class ImageGenerationController
{
	// Eventos públicos
	public event EventHandler<EndGenerationArgs>? ChatReceived;
	// Variables privadas
	private string? _model;

	public ImageGenerationController(string? url, string? apiKey, TimeSpan? timeout = null)
	{
		// Url por defecto si no se ha pasado nada
		if (string.IsNullOrWhiteSpace(url))
			url = "https://stablehorde.net";
		// ApiKey predeterminada
		if (string.IsNullOrWhiteSpace(apiKey))
			apiKey = "0000000000";
		// Crea el manager y el streamer principal
		Manager = new StableHordeManager(new Uri(url), apiKey, timeout);
		// Crea el timer
		CheckStatusController = new CheckStatusController(this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));
	}

	/// <summary>
	///		Conecta al manager
	/// </summary>
	public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
	{
		// Conecta si es necesario
		if (!Connected)
			try
			{
				List<ModelDto>? models = await Manager.ModelsController.GetAsync(cancellationToken);

					if (models?.Count > 0)
					{
						// Guarda los modelos
						Models.Clear();
						Models.AddRange(models);
						// Indica que se ha conectado
						Connected = true;
					}
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error: {exception.Message}");
			}
		// Devuelve el valor que indica si se ha conectado
		return Connected;
	}

	/// <summary>
	///		Envía un mensaje de generación de imágenes y obtiene la respuesta
	/// </summary>
	public async Task PromptAsync(string prompt, CancellationToken cancellationToken)
	{
		if (!string.IsNullOrWhiteSpace(prompt) && !ParseScapedPrompt(prompt.Trim()) && Connected && !string.IsNullOrWhiteSpace(Model))
		{
			ImageGeneratorResponseDto? response = await Manager.ImagesGeneratorController.GenerateAsync(GetGenerationPrompt(prompt), cancellationToken);

				// Trata la respuesta
				if (response is null || string.IsNullOrWhiteSpace(response.id))
					RaiseResponseEvent($"Error when generate image: {response?.message}. {response?.Errors?.additionalProp1} {response?.Errors?.additionalProp2} {response?.Errors?.additionalProp3}", true);
				else
				{
					CheckStatusController.AddGeneration(response.id);
					RaiseResponseEvent($"Waiting image generation: {response.id}", true);
				}
		}
	}

	/// <summary>
	///		Obtiene el prompt de generación
	/// </summary>
	private ImageGeneratorDto GetGenerationPrompt(string prompt)
	{
		ImageGeneratorDto dto = new();

			// Asigna las propiedades
			dto.prompt = prompt;
			if (!string.IsNullOrWhiteSpace(Model))
				dto.models.Add(Model);
			// Devuelve el Dto
			return dto;
	}

	/// <summary>
	///		Interpreta el prompt para tratar los prompts predefinidos
	/// </summary>
	private bool ParseScapedPrompt(string prompt)
	{
		bool scaped = true;

			// Interpreta el prompt entre los predefinidos
			if (prompt.StartsWith("/model", StringComparison.CurrentCultureIgnoreCase))
			{
				string model = prompt.Substring("/model".Length + 1);

					// Busca el modelo en la lista
					if (!string.IsNullOrWhiteSpace(model))
					{
						bool found = false;

							// Quita los espacios
							model = model.Trim();
							// Busca el modelo
							foreach (ModelDto item in Models)
								if (item.Name.StartsWith(model, StringComparison.CurrentCultureIgnoreCase))
								{
									// Cambia el modelo
									Model = item.Name;
									// Indica que se ha cambiado
									found = true;
									RaiseResponseEvent($"You are talking to {Model} now.", true);
								}
							// Indica al usuario que no ha podido encontrar el modelo buscado
							if (!found)
								RaiseResponseEvent($"Can't find the model {model}. You are talking to {Model} now.", true);
					}
			}
			else // Indica que este es un valor desconocido
				scaped = false;
			// Devuelve el valor que indica si se ha tratado el prompt
			return scaped;
	}

	/// <summary>
	///		Lanza el evento de respuesta
	/// </summary>
	private void RaiseResponseEvent(string message, bool isEnd)
	{
		ChatReceived?.Invoke(this, new EndGenerationArgs(message, isEnd));
	}

	/// <summary>
	///		Manager de StableHorde
	/// </summary>
	internal StableHordeManager Manager { get; }

	/// <summary>
	///		Controlador de estados
	/// </summary>
	internal CheckStatusController CheckStatusController { get; }

	/// <summary>
	///		Indica si está conectado
	/// </summary>
	public bool Connected { get; private set; }

	/// <summary>
	///		Modelos del motor
	/// </summary>
	public List<ModelDto> Models { get; } = new();

	/// <summary>
	///		Modelo actual
	/// </summary>
	public string? Model 
	{ 
		get 
		{
			// Si no se ha definido el modelo, se coge el primero
			if (string.IsNullOrWhiteSpace(_model) && Models.Count > 0)
				_model = Models[0].Name;
			// Devuelve el modelo
			return _model;
		}
		private set { _model = value; }
	}
}