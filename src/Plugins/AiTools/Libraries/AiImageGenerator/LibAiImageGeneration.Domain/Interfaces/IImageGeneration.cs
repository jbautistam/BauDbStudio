using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

namespace Bau.Libraries.LibAiImageGeneration.Domain.Interfaces;

/// <summary>
///		Interface para los Api de generación de imágenes
/// </summary>
public interface IImageGeneration
{
	/// <summary>
	///		Obtiene la lista de definiciones de la API
	/// </summary>
	Task<List<DefinitionResultModel>> GetDefinitionsAsync(CancellationToken cancellationToken);

	/// <summary>
	///		Ejecuta el prompt sobre el generador de imágenes
	/// </summary>
	Task<PromptResultModel> PromptAsync(PromptRequestModel prompt, CancellationToken cancellationToken);

	/// <summary>
	///		Comprueba el estado de generación de una imagen
	/// </summary>
	Task<ProcessResultModel?> CheckAsync(string id, CancellationToken cancellationToken);

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	Task<ProcessResultModel?> DownloadAsync(string id, CancellationToken cancellationToken);
}
