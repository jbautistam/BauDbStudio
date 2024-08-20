namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Dto de parámetros de inversión de texto para la generación de imágenes
/// </summary>
public class ImageGeneratorParametersTextualInversionsDto
{
	/// <summary>
	///		The exact name or CivitAI ID of the Textual Inversion.
	/// </summary>
	public string name { get; set; } = default!;

	/// <summary>
	/// If set, Will automatically add this TI filename to the prompt or negative prompt accordingly using the provided strength.
	/// If this is set to None, then the user will have to manually add the embed to the prompt themselves.
	/// Valores posibles: prompt, negprompt
	/// </summary>
	public string? inject_ti { get; set; }

	/// <summary>
	/// The strength with which to apply the TI to the prompt. Only used when inject_ti is not None (de -5 a 5)
	/// </summary>
	public double strength { get; set; } = 1;
}