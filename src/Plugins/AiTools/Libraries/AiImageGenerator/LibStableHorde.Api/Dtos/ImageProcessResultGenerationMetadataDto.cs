namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Metadatos del proceso de la imagen generada
/// </summary>
public class ImageProcessResultGenerationMetadataDto
{
	/// <summary>
	/// example: lora
	/// The relevance of the metadata field
	/// Enum: [ lora, ti, censorship, source_image, source_mask ]
	/// </summary>
	public string type { get; set; } = default!;

	/// <summary>
	/// example: download_failed
	/// The value of the metadata field
	/// Enum: [ download_failed, parse_failed, baseline_mismatch, csam, nsfw ]
	/// </summary>
	public string value { get; set; } = default!;

	/// <summary>
	/// maxLength: 255
	/// Optionally a reference for the metadata (e.g. a lora ID)
	/// </summary>
	public string @ref { get; set; } = default!;
}
