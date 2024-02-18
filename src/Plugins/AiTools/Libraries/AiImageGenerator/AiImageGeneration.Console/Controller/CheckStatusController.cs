using Bau.Libraries.LibStableHorde.Api.Dtos;

namespace StableHorde.Console.Controller;

/// <summary>
///		Controlador de estado de la generación de imágenes
/// </summary>
internal class CheckStatusController : IDisposable
{
	// Variables privadas
	private object _lockImages = new(), _lockStatus = new();
	private Timer _timer;
	private SemaphoreSlim _semaphore;

	internal CheckStatusController(ImageGenerationController stableHordeController, TimeSpan timeStart, TimeSpan timePeriod)
	{
		StableHordeController = stableHordeController;
		_timer = new Timer(async (_) => await CheckStatusAsync(), null, (int) timeStart.TotalMilliseconds, (int) timePeriod.TotalMilliseconds);
		_semaphore = new SemaphoreSlim(1, 1);
	}

	/// <summary>
	///		Añade un identificador de generación
	/// </summary>
	internal void AddGeneration(string id)
	{
		lock (_lockImages)
		{
			Generations.Add(new Models.GenerationModel(id));
		}
	}

	/// <summary>
	///		Comprueba el estado de las generaciones de las imágenes
	/// </summary>
	private async Task CheckStatusAsync()
	{
		// Espera al semáforo
		_semaphore.Wait();
		// Comprueba las imágenes generadas
		if (Generations.Count > 0)
			foreach (Models.GenerationModel generation in Generations)
				if (generation.MustCheck())
					await CheckGenerationAsync(generation, CancellationToken.None);
				else if (generation.MustDownload())
					await DownloadGenerationAsync(generation, CancellationToken.None);
		// Libera el semáforo
		_semaphore.Release();
	}

	/// <summary>
	///		Comprueba el resultado de la generación de una imagen
	/// </summary>
	private async Task CheckGenerationAsync(Models.GenerationModel generation, CancellationToken cancellationToken)
	{
		try
		{
			ImageProcessResultDto? result = await StableHordeController.Manager.ImagesGeneratorController.CheckProcessAsync(generation.Id, cancellationToken);

				if (result is not null)
				{
					if (result.done)
						generation.MarkGenerated("Done", false);
					else if (result.faulted)
						generation.MarkGenerated("Error", true);
					else if (!result.is_possible)
						generation.MarkGenerated("Is not possible", true);
					else
						generation.MarkCheck(result.finished, result.processing, result.queue_position, TimeSpan.FromSeconds(result.wait_time));
				}
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("Error: " + exception.Message);
		}
	}

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	private async Task DownloadGenerationAsync(Models.GenerationModel generation, CancellationToken cancellationToken)
	{
		try
		{
			ImageProcessResultDto? result = await StableHordeController.Manager.ImagesGeneratorController.GetGenerationAsync(generation.Id, cancellationToken);

				if (result is not null)
				{
					if (result.done)
					{
						// Añade los datos generados
						if (result.generations is not null)
							foreach (ImageProcessResultGenerationDto generationDto in result.generations)
								generation.AddImage(generationDto.id, generationDto.model, generationDto.seed, generationDto.censored, generationDto.img);
						// Marca la generación como descargada
						generation.MarkDownloaded("Done", false);
					}
					else if (result.faulted)
						generation.MarkGenerated("Error", true);
					else if (!result.is_possible)
						generation.MarkGenerated("Is not possible", true);
				}
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("Error: " + exception.Message);
		}
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			// Libera la memoria
			if (disposing)
			{
				if (_timer is not null)
					_timer.Dispose();
			}
			// Indica si se ha liberado
			IsDisposed = true;
		}
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	public void Dispose()
	{
		// No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///		Controlador
	/// </summary>
	internal ImageGenerationController StableHordeController { get; }

	/// <summary>
	///		Ids de las imágenes generadas
	/// </summary>
	private List<Models.GenerationModel> Generations { get; } = new();

	/// <summary>
	///		Indica si se ha liberado la memoria
	/// </summary>
	internal bool IsDisposed { get; private set; }
}
