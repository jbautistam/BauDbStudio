namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Dto para la generación de imágenes
/// </summary>
public class ImageGeneratorDto
{
	/// <summary>
	/// minLength: 1
	/// The prompt which will be sent to Stable Diffusion to generate an image.
	/// Los prompts negativos, se unen al prompt normal, separado por ###, por ejemplo: woman blondy\n ### mi negative prompt
	/// </summary>
	public string prompt { get; set; } = default!;

	/// <summary>
	///		Parámetros de generación
	/// </summary>
	public ImageGeneratorParametersDto @params { get; } = new();

	/// <summary>
	/// default: false
	/// Set to true if this request is NSFW. This will skip workers which censor images.
	/// </summary>
	public bool nsfw { get; set; } = true;

	/// <summary>
	/// default: false
	/// When true, only trusted workers will serve this request. When False, Evaluating workers will also be used which can increase speed but adds more risk!
	/// </summary>
	public bool trusted_workers { get; set; }

	/// <summary>
	/// default: true
	/// When True, allows slower workers to pick up this request. Disabling this incurs an extra kudos cost.
	/// </summary>
	public bool slow_workers { get; set; } = true;

	/// <summary>
	/// default: false
	/// If the request is SFW, and the worker accidentally generates NSFW, it will send back a censored image.
	/// </summary>
	public bool censor_nsfw { get; set; }

	/// <summary>
	/// Specify up to 5 workers which are allowed to service this request.
	/// </summary>
	public List<string> workers { get; } = new();

	/// <summary>
	/// default: false
	/// If true, the worker list will be treated as a blacklist instead of a whitelist.
	/// </summary>
	public bool worker_blacklist { get; set; }

	/// <summary>
	/// Specify which models are allowed to be used for this request.
	/// </summary>
	public List<string> models { get; } = new();

	///// <summary>
	///// The Base64-encoded webp to use for img2img.
	///// </summary>
	//public string? source_image { get; set; }

	///// <summary>
	///// default: img2img
	///// example: img2img
	///// If source_image is provided, specifies how to process it.
	///// Enum:
	///// [ img2img, inpainting, outpainting ]
	///// </summary>
	//public string? source_processing { get; set; }

	///// <summary>
	///// If source_processing is set to 'inpainting' or 'outpainting', this parameter can be optionally provided as the Base64-encoded webp mask of the areas to inpaint. If this arg is not passed, the inpainting/outpainting mask has to be embedded as alpha channel.
	///// </summary>
	//public string? source_mask { get; set; }

	/// <summary>
	/// default: true
	/// If True, the image will be sent via cloudflare r2 download link.
	/// </summary>
	public bool r2 { get; set; } = true;

	/// <summary>
	/// default: false
	/// If True, The image will be shared with LAION for improving their dataset. This will also reduce your kudos consumption by 2. For anonymous users, this is always True.
	/// </summary>
	public bool shared { get; set; }

	/// <summary>
	/// default: true
	/// If enabled, suspicious prompts are sanitized through a string replacement filter instead.
	/// </summary>
	public bool replacement_filter { get; set; } = false;

	///// <summary>
	///// default: false
	///// When false, the endpoint will simply return the cost of the request in kudos and exit.
	///// </summary>
	//public bool dry_run { get; set; }

	///// <summary>
	///// If using a service account as a proxy, provide this value to identify the actual account from which this request is coming from.
	///// </summary>
	//public string? proxied_account { get; set; }

	public string jobId { get; set; } = string.Empty;

	public bool gathered { get; set; } = false;

	public int started { get; set; } = 0;
}
