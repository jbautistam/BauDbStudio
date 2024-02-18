namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Dto de parámetros para la generación de imágenes
/// </summary>
public class ImageGeneratorParametersLoraDto
{
	/// <summary>
	/// example: GlowingRunesAIV6
	/// minLength: 1
	/// maxLength: 255
	/// The exact name or CivitAI ID of the LoRa.
	/// </summary>
	public string name { get; set; } = default!;

	/// <summary>
	/// default: 1
	/// minimum: -5
	/// maximum: 5
	/// The strength of the LoRa to apply to the SD model.
	/// </summary>
	public double model { get; set; }

	/// <summary>
	/// default: 1
	/// minimum: -5
	/// maximum: 5
	/// The strength of the LoRa to apply to the clip model.
	/// </summary>
	public double clip { get; set; }

	/// <summary>
	/// minLength: 1
	/// maxLength: 30
	/// If set, will try to discover a trigger for this LoRa which matches or is similar to this string and inject it into the prompt. If 'any' is specified it will be pick the first trigger.
	/// </summary>
	public string? inject_trigger { get; set; }

	/// <summary>
	/// default: false
	/// If true, will consider the LoRa ID as a CivitAI version ID and search accordingly. Ensure the name is an integer.
	/// </summary>
	public bool is_version { get; set; }
}
