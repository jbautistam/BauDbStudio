using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;
using Bau.Libraries.LibStableHorde.Api.Communications;

namespace Bau.Libraries.LibStableHorde.Api;

/// <summary>
///		Cliente de la api de Horde AI
/// </summary>
public class StableHordeManager : LibAiImageGeneration.Domain.Interfaces.IImageGeneration
{
	public StableHordeManager(Uri url, string apiKey, TimeSpan? timeout = null)
	{
		RestManager = new RestManager(url, apiKey, timeout ?? TimeSpan.FromMinutes(2));
		ModelsController = new Controllers.ModelsController(this);
		GeneratorController = new Controllers.ImagesGeneratorController(this);
	}

	/// <summary>
	///		Obtiene las definiciones
	/// </summary>
	public async Task<List<DefinitionResultModel>> GetDefinitionsAsync(CancellationToken cancellationToken)
	{
		List<DefinitionResultModel> definitions = new();
		List<Dtos.ModelDto>? models = await ModelsController.GetAsync(cancellationToken);

			// Obtiene los modelos
			if (models is not null)
				foreach (Dtos.ModelDto model in models)
					definitions.Add(new DefinitionResultModel(DefinitionResultModel.DefinitionType.Model, model.Name));
			// Devuelve las definiciones
			return definitions;
	}

	/// <summary>
	///		Genera la imagen asociada a un <see cref="PromptRequestModel"/>
	/// </summary>
	public async Task<PromptResultModel> PromptAsync(PromptRequestModel prompt, CancellationToken cancellationToken) => await GeneratorController.GenerateAsync(prompt, cancellationToken);

	/// <summary>
	///		Comprueba la generación de una imagen
	/// </summary>
	public async Task<ProcessResultModel?> CheckAsync(string id, CancellationToken cancellationToken) => await GeneratorController.CheckAsync(id, cancellationToken);

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	public async Task<ProcessResultModel?> DownloadAsync(string id, CancellationToken cancellationToken) => await GeneratorController.DownloadAsync(id, cancellationToken);

	/// <summary>
	///		Controlador de las APIS de modelos
	/// </summary>
	private Controllers.ModelsController ModelsController { get; }

	/// <summary>
	///		Controlador de la API para generación de imágenes
	/// </summary>
	private Controllers.ImagesGeneratorController GeneratorController { get; }

	/// <summary>
	///		Manager de comunicaciones Rest
	/// </summary>
	internal RestManager RestManager { get; }
}