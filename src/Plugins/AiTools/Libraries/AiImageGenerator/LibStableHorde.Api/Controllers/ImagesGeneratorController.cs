using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;
using Bau.Libraries.LibStableHorde.Api.Dtos;

namespace Bau.Libraries.LibStableHorde.Api.Controllers;

/// <summary>
///		Controlador para llamadas a rutinas relacionadas con generación de imágenes
/// </summary>
internal class ImagesGeneratorController : BaseController
{
	internal ImagesGeneratorController(StableHordeManager manager) : base(manager) {}

	/// <summary>
	///		Genera una imagen
	/// </summary>
	internal async Task<PromptResultModel> GenerateAsync(PromptRequestModel prompt, CancellationToken cancellationToken)
	{
		ImageGeneratorDto generator = CreateGenerator(prompt);

			// Genera la imagen
			try
			{
				ImageGeneratorResponseDto? response = await Manager.RestManager.PostAndParseNoCheckStatusAsync<ImageGeneratorResponseDto>("/api/v2/generate/async", generator, cancellationToken);

					if (response is not null)
					{
						if (!string.IsNullOrWhiteSpace(response.id))
							return new PromptResultModel(true, response.id, "Processing");
						else
							return new PromptResultModel(true, null, response.message);
					}
					else
						return new PromptResultModel(false, null, "Can't process");
			}
			catch (Exception exception)
			{
				return new PromptResultModel(false, null, $"Error when generate image {exception.Message}");
			}
	}

	/// <summary>
	///		Crea la solicitud de generación
	/// </summary>
	private ImageGeneratorDto CreateGenerator(PromptRequestModel prompt)
	{
		ImageGeneratorDto dto = new();

			// Asigna los datos
			dto.prompt = prompt.Prompt;
			if (!string.IsNullOrWhiteSpace(prompt.NegativePrompt))
				dto.prompt += " ### " + prompt.NegativePrompt;
			dto.nsfw = prompt.Nsfw;
			dto.models.AddRange(prompt.Models);
			dto.@params.sampler_name = prompt.Sampler.ToString();
			dto.@params.cfg_scale = prompt.CfgScale;
			dto.@params.denoising_strength = prompt.DenoisingStrength;
			dto.@params.seed = prompt.Seed;
			dto.@params.height = prompt.Height;
			dto.@params.width = prompt.Width;
			if (prompt.PostProcessing.Count > 0)
				foreach (PromptRequestModel.PostProcessType processType in prompt.PostProcessing)
					dto.@params.post_processing.Add(processType.ToString());
			dto.@params.karras = prompt.Karras;
			dto.@params.steps = prompt.Steps;
			dto.@params.n = prompt.ImagesToGenerate;
			// Devuelve la solicitud
			return dto;
	}

	/// <summary>
	///		Comprueba el resultado
	/// </summary>
	internal async Task<ProcessResultModel?> CheckAsync(string id, CancellationToken cancellationToken)
	{
		ImageProcessResultDto? dto = await CheckProcessAsync(id, cancellationToken);
		ProcessResultModel result = new();

			// Genera el resultado
			if (dto is not null)
			{
				// Asigna el estado
				if (dto.done)
				{
					result.Status = ProcessResultModel.StatusType.Success;
					result.Message = "Done";
				}
				else if (dto.faulted)
				{
					result.Status = ProcessResultModel.StatusType.Error;
					result.Message = "Error";
				}
				else if (!dto.is_possible)
				{
					result.Status = ProcessResultModel.StatusType.Error;
					result.Message = "Is not possible";
				}
				else
				{
					result.Status = ProcessResultModel.StatusType.Processing;
					result.Message = "Processing";
				}
				// Asigna las propiedades
				result.Finished = dto.finished;
				result.Processing = dto.processing;
				result.QueuePosition = dto.queue_position;
				result.Expected = TimeSpan.FromSeconds(dto.wait_time);
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Comprueba si se ha terminado el proceso de una imagen
	/// </summary>
	private async Task<ImageProcessResultDto?> CheckProcessAsync(string id, CancellationToken cancellationToken)
	{
		try
		{
			return await Manager.RestManager.GetResponseNoCheckStatusDataAsync<ImageProcessResultDto>($"/api/v2/generate/check/{id}", cancellationToken);
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine($"Error when check image. {exception.Message}");
			return null;
		}
	}

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	internal async Task<ProcessResultModel?> DownloadAsync(string id, CancellationToken cancellationToken)
	{
		ProcessResultModel? process = null;
		ImageProcessResultDto? result = await GetGenerationAsync(id, cancellationToken);

			if (result is not null)
			{
				// Crea el objeto de salida
				process = new ProcessResultModel();
				// Asigna las propiedades
				process.Finished = result.finished;
				process.Processing = result.processing;
				process.QueuePosition = result.queue_position;
				// Asigna el estado
				if (result.faulted)
				{
					process.Status = ProcessResultModel.StatusType.Error;
					process.Message = "Error";
				}
				else if (!result.is_possible)
				{
					process.Status = ProcessResultModel.StatusType.Error;
					process.Message = "It's not possible";
				}
				else if (result.done)
				{
					// Cambia el estado
					process.Status = ProcessResultModel.StatusType.Success;
					// Añade los datos generados
					if (result.generations is not null)
						foreach (ImageProcessResultGenerationDto generationDto in result.generations)
							process.Images.Add(new ProcessImageResultModel(generationDto.id, generationDto.model, generationDto.seed, 
																	  generationDto.censored, generationDto.img, ".webp"));
				}
			}
			// Devueve los resultados
			return process;
	}

	/// <summary>
	///		Obtiene la generación de una imagen
	/// </summary>
	private async Task<ImageProcessResultDto?> GetGenerationAsync(string id, CancellationToken cancellationToken)
	{
		try
		{
			return await Manager.RestManager.GetResponseNoCheckStatusDataAsync<ImageProcessResultDto>($"/api/v2/generate/status/{id}", cancellationToken);
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine($"Error when check image. {exception.Message}");
			return null;
		}
	}
}
