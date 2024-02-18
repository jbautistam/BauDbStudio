using Bau.Libraries.LibAiImageGeneration.Models;

namespace Bau.Libraries.LibAiImageGeneration.Controllers;

/// <summary>
///		Controlador de estado de la generación de imágenes
/// </summary>
internal class CheckStatusController : IDisposable
{
	// Variables privadas
	private object _lockImages = new();
	private Timer _timer;
	private SemaphoreSlim _semaphore;

	internal CheckStatusController(AiProvider provider, TimeSpan timeStart, TimeSpan timePeriod)
	{
		Provider = provider;
		_timer = new Timer(async (_) => await CheckStatusAsync(), null, (int) timeStart.TotalMilliseconds, (int) timePeriod.TotalMilliseconds);
		_semaphore = new SemaphoreSlim(1, 1);
	}

	/// <summary>
	///		Añade un identificador de generación
	/// </summary>
	internal void AddGeneration(Domain.Models.Requests.PromptRequestModel request, string id)
	{
		lock (_lockImages)
		{
			Generations.Add(new GenerationModel(id, request));
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
			foreach (GenerationModel generation in Generations)
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
	private async Task CheckGenerationAsync(GenerationModel generation, CancellationToken cancellationToken)
	{
		Domain.Models.Results.ProcessResultModel? result = await Provider.CheckAsync(generation.Id, cancellationToken);

			// Crea un resultado
			if (result is null)
				result = new Domain.Models.Results.ProcessResultModel
										{
											Status = Domain.Models.Results.ProcessResultModel.StatusType.Processing,
											Expected = TimeSpan.FromMinutes(1)
										};
			// Marca el resultado
			generation.Mark(result, true);
			// Llama al evento de generación
			Provider.ProvidersManager.Manager.RaiseProgressEvent(generation);
	}

	/// <summary>
	///		Descarga las imágenes generadas
	/// </summary>
	private async Task DownloadGenerationAsync(GenerationModel generation, CancellationToken cancellationToken)
	{
		Domain.Models.Results.ProcessResultModel? result = await Provider.GetGenerationAsync(generation.Id, cancellationToken);

			// Crea un resultado
			if (result is not null)
				switch (result.Status)
				{
					case Domain.Models.Results.ProcessResultModel.StatusType.Error:
							// Marca el estado
							generation.MarkGenerated("Error", true);
							// Llama al evento de progreso
							Provider.ProvidersManager.Manager.RaiseProgressEvent(generation);
						break;
					case Domain.Models.Results.ProcessResultModel.StatusType.Success:
							// Añade los datos generados
							if (result.Images is not null)
							{
								// Guarda las imágenenes generadas
								foreach (Domain.Models.Results.ProcessImageResultModel imageDto in result.Images)
									generation.AddImage(imageDto.Id, imageDto.Model, imageDto.Seed, imageDto.Censored, imageDto.UrlImage, ".webp");
								// Descarga las imágenes
								await DownloadImagesAsync(generation, cancellationToken);
							}
							// Marca la generación como descargada
							generation.MarkDownloaded("Done", false);
							// Llama al evento de fin de generación
							Provider.ProvidersManager.Manager.RaiseProgressEvent(generation);
						break;
				}
	}

	/// <summary>
	///		Descarga las imágenes
	/// </summary>
	private async Task DownloadImagesAsync(GenerationModel generation, CancellationToken cancellationToken)
	{
		foreach (GenerationImageModel image in generation.Images)
			if (!image.Censored && (image.UrlImage.StartsWith("https:", StringComparison.CurrentCultureIgnoreCase) ||
									image.UrlImage.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase)))
				await DownloadImageAsync(GetFileName(generation.Request, image.Extension), image, cancellationToken);
	}

	/// <summary>
	///		Obtiene el nombre de un archivo
	/// </summary>
	private string GetFileName(Domain.Models.Requests.PromptRequestModel request, string extension)
	{
		int index = 1;
		string fileName = GetNextFile(request.Folder, request.PrefixFileName, index, extension);

			// Si existe el archivo, genera el siguiente nombre
			while (File.Exists(fileName))
				fileName = GetNextFile(request.Folder, request.PrefixFileName, ++index, extension);
			// Devuelve el nombre de archivo
			return fileName;

			// Obtiene el siguiente nombre de archivo
			string GetNextFile(string folder, string prefix, int index, string extension)
			{
				// Quita el carácter final del directorio
				if (folder.EndsWith("/") || folder.EndsWith("\\"))
					folder = folder.Substring(0, folder.Length - 1);
				// Añade el punto a la extensión
				if (!string.IsNullOrWhiteSpace(extension) && !extension.StartsWith('.'))
					extension = $".{extension}";
				// Quita el punto final del prefijo
				if (!string.IsNullOrWhiteSpace(prefix) && prefix.EndsWith('.'))
					prefix = prefix.Substring(0, prefix.Length - 1);
				// Devuelve el nombre de archivo
				return $"{folder}/{prefix}.{index}{extension}";
			}
	}

	/// <summary>
	///		Descarga una imagen
	/// </summary>
	private async Task DownloadImageAsync(string fileName, GenerationImageModel image, CancellationToken cancellationToken)
	{
		try
		{
			// Descarga la imagen
			await new HttpWebClient().DownloadFileAsync(image.UrlImage, fileName, cancellationToken);
			// Indica que se ha descargado
			image.Downloaded = true;
		}
		catch (Exception exception)
		{
			image.Downloaded = false;
			image.Error = $"Error when download image {exception.Message}";
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
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///		Proveedor
	/// </summary>
	internal AiProvider Provider { get; }

	/// <summary>
	///		Ids de las imágenes generadas
	/// </summary>
	private List<GenerationModel> Generations { get; } = new();

	/// <summary>
	///		Indica si se ha liberado la memoria
	/// </summary>
	internal bool IsDisposed { get; private set; }
}
