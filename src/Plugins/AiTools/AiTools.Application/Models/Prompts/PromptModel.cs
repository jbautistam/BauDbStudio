namespace Bau.Libraries.AiTools.Application.Models.Prompts;

/// <summary>
///		Clase con los datos de un prompt
/// </summary>
public class PromptModel
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

	public PromptModel(string provider)
	{
		Provider = provider;
	}

	/// <summary>
	///		Clona una versión
	/// </summary>
	public PromptModel Clone(int version)
	{
		PromptModel prompt = new(Provider)
								{
									Version = version,
									Positive = Positive,
									Negative = Negative,
									Model = Model,
									Nsfw = Nsfw,
									Sampler = Sampler,
									CfgScale = CfgScale,
									DenoisingStrength = DenoisingStrength,
									Seed = Seed,
									Height = Height,
									Width = Width,
									Karras = Karras,
									Steps = Steps,
									ImagesToGenerate = ImagesToGenerate
								};

		// Añade los datos de postproceso
		prompt.PostProcessing.AddRange(PostProcessing);
		// Devuelve el prompt
		return prompt;
	}

	/// <summary>
	///		Proveedor
	/// </summary>
	public string Provider { get; }

	/// <summary>
	///		Versión
	/// </summary>
	public int Version { get; set; }

	/// <summary>
	///		Prompt positivo
	/// </summary>
	public string Positive { get; set; } = default!;

	/// <summary>
	///		Prompt negativo
	/// </summary>
	public string Negative { get; set; } = default!;

	/// <summary>
	///		Modelo
	/// </summary>
	public string Model { get; set; } = default!;

	/// <summary>
	///		Indica si la solicitud es NSFW
	/// </summary>
	public bool Nsfw { get; set; }

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
