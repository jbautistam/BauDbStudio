namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Dto con la respuesta de procesamiento de una imagen
/// </summary>
public class ImageProcessResultDto
{
	/// <summary>
	/// The amount of finished jobs in this request.
	/// </summary>
	public int finished { get; set; }

	/// <summary>
	/// The amount of still processing jobs in this request.
	/// </summary>
	public int processing { get; set; }

	/// <summary>
	/// The amount of jobs that timed out and had to be restarted or were reported as failed by a worker.
	/// </summary>
	public int restarted { get; set; }

	/// <summary>
	/// The amount of jobs waiting to be picked up by a worker.
	/// </summary>
	public int waiting { get; set; }

	/// <summary>
	/// True when all jobs in this request are done. Else False.
	/// </summary>
	public bool done { get; set; }

	/// <summary>
	/// default: false
	/// True when this request caused an public server error and could not be completed.
	/// </summary>
	public bool faulted { get; set; }

	/// <summary>
	/// The expected amount to wait (in seconds) to generate all jobs in this request.
	/// </summary>
	public int wait_time { get; set; }

	/// <summary>
	/// The position in the requests queue. This position is determined by relative Kudos amounts.
	/// </summary>
	public int queue_position { get; set; }

	/// <summary>
	/// The amount of total Kudos this request has consumed until now.
	/// </summary>
	public double kudos { get; set; }

	/// <summary>
	/// default: true
	/// If False, this request will not be able to be completed with the pool of workers currently available.
	/// </summary>
	public bool is_possible { get; set; }

	/// <summary>
	/// If True, These images have been shared with LAION.
	/// </summary>
	public bool shared { get; set; }

	/// <summary>
	///		Imágenes generadas
	/// </summary>
	public List<ImageProcessResultGenerationDto>? generations { get; set; }

	/// <summary>
	///		Mensajes de salida (normalmente error)
	/// </summary>
	public string? message { get; set; }
}