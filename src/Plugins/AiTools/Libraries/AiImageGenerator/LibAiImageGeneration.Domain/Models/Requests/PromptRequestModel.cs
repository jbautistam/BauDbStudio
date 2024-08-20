namespace Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;

/// <summary>
///		Modelo con los datos de solicitud de generación
/// </summary>
public class PromptRequestModel
{
	/// <summary>
	///		Tipo de sampleado
	/// </summary>
	public enum SamplerType
	{
		k_euler, k_dpm_fast, k_heun, k_dpmpp_sde, dpmsolver, k_dpm_2_a, k_dpm_adaptive, k_dpmpp_2s_a, k_lms, k_euler_a, lcm, DDIM, k_dpmpp_2m, k_dpm_2
	}
	/// <summary>
	///		Tipo de postproceso
	/// </summary>
	public enum PostProcessType
	{
		GFPGAN, RealESRGAN_x4plus, RealESRGAN_x2plus, RealESRGAN_x4plus_anime_6B, NMKD_Siax, x4_AnimeSharp, CodeFormers, strip_background
	}

	public PromptRequestModel(string provider, string folder, string prefixFileName)
	{
		Provider = provider;
		Folder = folder;
		PrefixFileName = prefixFileName;
	}

	/// <summary>
	///		Proveedor sobre el que se genera el prompt
	/// </summary>
	public string Provider { get; }

	/// <summary>
	///		Carpeta donde se deben descargar los archivos
	/// </summary>
	public string Folder { get; }

	/// <summary>
	///		Prefijo de los nombres de archivo
	/// </summary>
	public string PrefixFileName { get; }

	/// <summary>
	///		Prompt enviado para generación de imagen
	/// </summary>
	public string Prompt { get; set; } = default!;

	/// <summary>
	///		Prompt negativo
	/// </summary>
	public string NegativePrompt { get; set; } = default!;

	/// <summary>
	///		Indica si la solicitud es NSFW
	/// </summary>
	public bool Nsfw { get; set; }

	/// <summary>
	///		Especifica los modelos permitidos para utilizar en esta solicitud
	/// </summary>
	public List<string> Models { get; } = new();

	/// <summary>
	///		Tipo de sampleado
	/// </summary>
	public SamplerType Sampler { get; set; } = SamplerType.k_euler;

	/// <summary>
	///		Escala de (valores de 0 a 100)
	/// </summary>
	public double CfgScale { get; set; } = 7.5;

	/// <summary>
	///		Intensidad de eliminación de ruido (de 0.01 a 1)
	/// </summary>
	public double DenoisingStrength { get; set; } = 0.75;

	/// <summary>
	///		Semilla utilizada para la generación
	/// </summary>
	public string Seed { get; set; } = string.Empty;

	/// <summary>
	///		Altura de la imagen generada (de 64 a 3072, múltiplo de 64)
	/// </summary>
	public int Height { get; set; } = 512;

	/// <summary>
	///		Anchura de la imagen generada (de 64 a 3072, múltiplo de 64)
	/// </summary>
	public int Width { get; set; } = 512;

	/// <summary>
	///		Tipos de postproceso
	/// </summary>
	public List<PostProcessType> PostProcessing { get; } = new();

	/// <summary>
	///		Ajustes de programación de ruido karras.
	/// </summary>
	public bool Karras { get; set; } = true;

	/// <summary>
	///		Pasos (predeterminado 30, de 1 a 500)
	/// </summary>
	public int Steps { get; set; } = 30;

	/// <summary>
	///		Número de imágenes a generar (predeterminado 1, de 1 a 20)
	/// </summary>
	public int ImagesToGenerate { get; set; } = 1;
}
