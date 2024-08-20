using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

namespace Bau.Libraries.LibAiImageGeneration.Controllers;

/// <summary>
///		Proveedor AI para generación de imágenes
/// </summary>
public class AiProvider
{
	public AiProvider(AiProvidersManager providersManager, string name, Domain.Interfaces.IImageGeneration aiGenerator)
	{
		ProvidersManager = providersManager;
		Name = name;
		AiGenerator = aiGenerator;
		CheckStatusController = new CheckStatusController(this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));
	}

	/// <summary>
	///		Conecta al proveedor
	/// </summary>
	public async Task ConnectAsync(CancellationToken cancellationToken)
	{
		// Indica que no se ha conectado
		IsConnected = false;
		// Limpia los modelos
		Models.Clear();
		// Carga los datos
		try
		{
			// Añade los modelos
			Models.AddRange(await AiGenerator.GetDefinitionsAsync(cancellationToken));
			// y los ordena
			Models.Sort();
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine($"Connect error. {exception.Message}");
		}
	}

	/// <summary>
	///		Lanza un prompt sobre el proveedor
	/// </summary>
	public async Task<PromptResultModel> PromptAsync(PromptRequestModel prompt, CancellationToken cancellationToken)
	{
		PromptResultModel result = await AiGenerator.PromptAsync(prompt, cancellationToken);

			// Si se ha podido procesar, añade la solicitud a la cola de procesamiento
			if (result.CanProcess && !string.IsNullOrWhiteSpace(result.Id))
				CheckStatusController.AddGeneration(prompt, result.Id);
			// Devuelve el valor que indica si se ha ejecutado
			return result;
	}

	/// <summary>
	///		Comprueba el resultado de la generación de una imagen
	/// </summary>
	public async Task<ProcessResultModel?> CheckAsync(string id, CancellationToken cancellationToken)
	{
		ProcessResultModel? result = null;

			// Compruebe el resultado
			try
			{
				result = await AiGenerator.CheckAsync(id, cancellationToken);
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error when check result. Id: {id}. {exception.Message}");
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	public async Task<ProcessResultModel?> GetGenerationAsync(string id, CancellationToken cancellationToken)
	{
		ProcessResultModel? result = null;

			// Compruebe el resultado
			try
			{
				result = await AiGenerator.DownloadAsync(id, cancellationToken);
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error when download result. Id: {id}. {exception.Message}");
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Manager de proveedores
	/// </summary>
	internal AiProvidersManager ProvidersManager { get; }

	/// <summary>
	///		Indica si se ha conectado
	/// </summary>
	public bool IsConnected { get; private set; }

	/// <summary>
	///		Nombre del proveedor
	/// </summary>
	public string Name { get; }

	/// <summary>
	///		Generador
	/// </summary>
	private Domain.Interfaces.IImageGeneration AiGenerator { get; }

	/// <summary>
	///		Controlador de estados
	/// </summary>
	private CheckStatusController CheckStatusController { get; }

	/// <summary>
	///		Modelos
	/// </summary>
	public List<DefinitionResultModel> Models { get; } = new();
}
