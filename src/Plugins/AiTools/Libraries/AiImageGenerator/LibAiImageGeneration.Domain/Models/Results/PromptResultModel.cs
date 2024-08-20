namespace Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

/// <summary>
///		Resultado de una solicitud de generación
/// </summary>
public class PromptResultModel
{
	public PromptResultModel(bool canProcess, string? id, string? message)
	{
		CanProcess = canProcess;
		Id = id;
		Message = message;
	}

	/// <summary>
	///		Indica si se puede procesar
	/// </summary>
	public bool CanProcess { get; }

	/// <summary>
	///		Identificador del proceso
	/// </summary>
	public string? Id { get; }

	/// <summary>
	///		Mensaje
	/// </summary>
	public string? Message { get; }
}
