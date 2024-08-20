namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Respuesta a la generación de una imagen
/// </summary>
public class ImageGeneratorResponseDto
{
	/// <summary>
	/// The UUID of the request. Use this to retrieve the request status in the future.
	/// </summary>
	public string id { get; set; } = default!;

	/// <summary>
	/// The expected kudos consumption for this request.
	/// </summary>
	public double kudos	{ get; set; }

	/// <summary>
	/// Any extra information from the horde about this request.
	/// </summary>
	public string? message { get; set; }

	/// <summary>
	///		Errores
	/// </summary>
	public ImageGeneratorResponseErrorDto? Errors { get; set; }
}
