namespace Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

/// <summary>
///		Resultado del procesamiento de imágenes
/// </summary>
public class ProcessResultModel
{
	/// <summary>
	///		Tipo de estado
	/// </summary>
	public enum StatusType
	{
		/// <summary>Ha finalizado correctamente</summary>
		Success = 1,
		/// <summary>Error en la generación</summary>
		Error,
		/// <summary>Se está procesando</summary>
		Processing
	}

	/// <summary>
	///		Estado del procesamiento
	/// </summary>
	public StatusType Status { get; set; }

	/// <summary>
	///		Trabajos finalizados
	/// </summary>
	public int Finished { get; set; }

	/// <summary>
	///		Trabajos en proceso
	/// </summary>
	public int Processing { get; set; }

	/// <summary>
	///		Posición en la cola
	/// </summary>
	public int QueuePosition { get; set; }

	/// <summary>
	///		Tiempo estimado de espera
	/// </summary>
	public TimeSpan Expected { get; set; }

	/// <summary>
	///		Mensaje
	/// </summary>
	public string? Message { get; set; }

	/// <summary>
	///		Imágenes generados
	/// </summary>
	public List<ProcessImageResultModel> Images { get; } = new();
}
