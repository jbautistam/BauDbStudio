namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Dto de parámetros para la generación de imágenes
/// </summary>
public class ImageGeneratorParametersDto
{
	/// <summary>
	/// default: k_euler_a
	/// example: k_euler
	/// Enum: [ k_euler, k_dpm_fast, k_heun, k_dpmpp_sde, dpmsolver, k_dpm_2_a, k_dpm_adaptive, k_dpmpp_2s_a, k_lms, k_euler_a, lcm, DDIM, k_dpmpp_2m, k_dpm_2 ]
	/// </summary>
	public string sampler_name { get; set; } = "k_euler";

	/// <summary>
	/// default: 7.5
	/// minimum: 0
	/// maximum: 100
	/// </summary>
	public double cfg_scale { get; set; } = 7.5;

	/// <summary>
	/// example: 0.75
	/// minimum: 0.01
	/// maximum: 1
	/// </summary>
	public double denoising_strength { get; set; } = 0.75;

	/// <summary>
	/// example: The little seed that could
	/// The seed to use to generate this request. You can pass text as well as numbers.
	/// </summary>
	public string seed { get; set; } = string.Empty;

	/// <summary>
	/// default: 512
	/// minimum: 64
	/// maximum: 3072
	/// multipleOf: 64
	/// The height of the image to generate.
	/// </summary>
	public int height { get; set; } = 512;

	/// <summary>
	/// default: 512
	/// minimum: 64
	/// maximum: 3072
	/// multipleOf: 64
	/// The width of the image to generate.
	/// </summary>
	public int width { get; set; } = 512;

	/// <summary>
	/// example: 1
	/// minimum: 1
	/// maximum: 1000
	/// If passed with multiple n, the provided seed will be incremented every time by this value.
	/// </summary>
	public int seed_variation { get; set; } = 1_000;

	/// <summary>
	/// uniqueItems: true
	/// string
	/// example: GFPGAN
	/// The list of post-processors to apply to the image, in the order to be applied.
	/// Enum:[ GFPGAN, RealESRGAN_x4plus, RealESRGAN_x2plus, RealESRGAN_x4plus_anime_6B, NMKD_Siax, 4x_AnimeSharp, CodeFormers, strip_background ]
	/// </summary>
	public List<string> post_processing { get; } = new();

	/// <summary>
	/// default: false
	/// Set to True to enable karras noise scheduling tweaks.
	/// </summary>
	public bool karras { get; set; } = true;

	/// <summary>
	/// default: false
	/// Set to True to create images that stitch together seamlessly.
	/// </summary>
	public bool tiling { get; set; }

	/// <summary>
	/// default: false
	/// Set to True to process the image at base resolution before upscaling and re-processing.
	/// </summary>
	public bool hires_fix { get; set; }

	/// <summary>
	/// example: 1
	/// minimum: 1
	/// maximum: 12
	/// The number of CLIP language processor layers to skip.
	/// </summary>
	public int clip_skip { get; set; } = 1;

	///// <summary>
	///// example: canny
	///// Enum: [ canny, hed, depth, normal, openpose, seg, scribble, fakescribbles, hough ]
	///// </summary>
	//public string? control_type { get; set; } = "canny";

	///// <summary>
	///// default: false
	///// Set to True if the image submitted is a pre-generated control map for ControlNet use.
	///// </summary>
	//public bool image_is_control { get; set; }

	///// <summary>
	///// default: false
	///// Set to True if you want the ControlNet map returned instead of a generated image.
	///// </summary>
	//public bool return_control_map { get; set; }

	/// <summary>
	/// example: 0.75
	/// minimum: 0
	/// maximum: 1
	/// </summary>
	public double facefixer_strength { get; set; } = 0.75;

	///// <summary>
	/////		Loras asociados al modelo
	///// </summary>
	//public List<ImageGeneratorParametersLoraDto> loras { get; } = new();

	///// <summary>
	/////		Parámetros de inversión textual
	///// </summary>
	//public List<ImageGeneratorParametersTextualInversionsDto> tis { get; } = new();

	///// <summary>
	///// 	Aquí debería ir un objeto de tipo ModelSpecialPayloadStable
	///// </summary>
	//public object? special { get; set; } = null;

	/// <summary>
	///		Pasos (predeterminado 30, de 1 a 500)
	/// </summary>
	public int steps { get; set; } = 30;

	/// <summary>
	///		Número de imágenes a generar (predeterminado 1, de 1 a 20)
	/// </summary>
	public int n { get; set; } = 1;
}