namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Resultado de generación de una imagen
/// </summary>
public class ImageProcessResultGenerationDto
{
	/// <summary>
	/// title: Worker ID
	/// The UUID of the worker which generated this image.
	/// </summary>
	public string worker_id	{ get; set; } = default!;

	/// <summary>
	/// title: Worker Name
	/// The name of the worker which generated this image.
	/// </summary>
	public string worker_name { get; set; } = default!;

	/// <summary>
	/// title: Generation Model
	/// The model which generated this image.
	/// </summary>
	public string model	{ get; set; } = default!;

	/// <summary>
	/// title: Generation State
	/// default: ok
	/// example: ok
	/// OBSOLETE (Use the gen_metadata field). The state of this generation.
	/// Enum: [ ok, censored ]
	/// </summary>
	public string? state { get; set; }

	/// <summary>
	/// title: Generated Image
	/// The generated image as a Base64-encoded .webp file.
	/// </summary>
	public string img { get; set; } = default!;

	/// <summary>
	/// title: Generation Seed
	/// The seed which generated this image.
	/// </summary>
	public string seed { get; set; } = default!;

	/// <summary>
	/// title: Generation ID
	/// The ID for this image.
	/// </summary>
	public string id { get; set; } = default!;

	/// <summary>
	/// When true this image has been censored by the worker's safety filter.
	/// </summary>
	public bool censored { get; set; }

	/// <summary>
	///		Metadatos de la generación
	/// </summary>
	public List<ImageProcessResultGenerationMetadataDto>? gen_metadata { get; set; }
}