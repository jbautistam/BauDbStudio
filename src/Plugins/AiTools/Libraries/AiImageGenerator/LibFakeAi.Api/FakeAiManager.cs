using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

namespace Bau.Libraries.LibFakeAi.Api;

/// <summary>
///		Cliente fake para pruebas de <see cref="LibAiImageGeneration.Domain.Interfaces.IImageGeneration"/>
/// </summary>
public class FakeAiManager : LibAiImageGeneration.Domain.Interfaces.IImageGeneration
{
	// Variables privadas
	private Random _random = new();

	/// <summary>
	///		Obtiene las definiciones
	/// </summary>
	public async Task<List<DefinitionResultModel>> GetDefinitionsAsync(CancellationToken cancellationToken)
	{
		List<DefinitionResultModel> definitions = new();

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Obtiene los modelos
			for (int index = 1; index < 10; index++)
				definitions.Add(new DefinitionResultModel(DefinitionResultModel.DefinitionType.Model, $"Model {index.ToString()}"));
			// Devuelve las definiciones
			return definitions;
	}

	/// <summary>
	///		Genera la imagen asociada a un <see cref="PromptRequestModel"/>
	/// </summary>
	public async Task<PromptResultModel> PromptAsync(PromptRequestModel prompt, CancellationToken cancellationToken)
	{
		if (_random.Next(100) > 75)
			return new PromptResultModel(false, null, "Random error");
		else
		{
			PromptResultModel result = new(true, Guid.NewGuid().ToString(), "Processing");

				// Evita las advertencias
				await Task.Delay(1, cancellationToken);
				// Añade la generación solicitada
				Generations.Add(result.Id, (prompt, 0));
				// Devuelve el Id
				return result;
		}
	}

	/// <summary>
	///		Comprueba la generación de una imagen
	/// </summary>
	public async Task<ProcessResultModel?> CheckAsync(string id, CancellationToken cancellationToken)
	{
		ProcessResultModel result = new();

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Obtiene los datos de la generación
			if (Generations.TryGetValue(id, out (PromptRequestModel prompt, int calls) generation))
			{
				// Dependiendo del número de llamadas
				if (generation.calls < 2)
					result.Status = ProcessResultModel.StatusType.Processing;
				else
					result.Status = ProcessResultModel.StatusType.Success;
				// Asigna las propiedades
				result.QueuePosition = _random.Next(100, 200);
				result.Expected = TimeSpan.FromSeconds(_random.Next(2) + 1);
				// Incrementa el número de llamada
				Generations[id] = (generation.prompt, generation.calls + 1);
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	public async Task<ProcessResultModel?> DownloadAsync(string id, CancellationToken cancellationToken)
	{
		ProcessResultModel result = new();

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Busca la generación correspondiente
			if (Generations.TryGetValue(id, out (PromptRequestModel prompt, int calls) generation))
			{
				// Cambia las propiedades del resultado
				result.Status = ProcessResultModel.StatusType.Success;
				result.Finished = generation.prompt.ImagesToGenerate;
				// Añade las imágeneso
				for (int index = 0; index < generation.prompt.ImagesToGenerate; index++)
					result.Images.Add(new ProcessImageResultModel(Guid.NewGuid().ToString(), generation.prompt.Models[0], generation.prompt.Seed,
															 false, GetImage(index), ".png"));
			}
			else
			{
				result.Status = ProcessResultModel.StatusType.Error;
				result.Message = $"Can't find the task {id}";
			}
			// Devuelve el resultado
			return result;

			// Obtiene una imagen "aleatoria" para descargar
			string GetImage(int index)
			{
				if (index % 2 == 0)
					return "https://jbautistam.github.io/images/cyborg.png";
				else
					return "https://jbautistam.github.io/images/hacker.png";
			}
	}

	/// <summary>
	///		Generaciones solicitadas
	/// </summary>
	private Dictionary<string, (PromptRequestModel prompt, int calls)> Generations { get; } = new();
}