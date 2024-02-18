using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;

namespace Bau.Libraries.LibAiImageGeneration.Models;

/// <summary>
///		Datos de una generación
/// </summary>
public class GenerationModel
{
	/// <summary>
	///		Estados de la generación
	/// </summary>
	public enum GenerationStatus
	{
		/// <summary>En proceso</summary>
		Processing = 1,
		/// <summary>Error</summary>
		Error,
		/// <summary>Generación finalizada</summary>
		Generated,
		/// <summary>Descargado</summary>
		Success
	}

	internal GenerationModel(string id, Domain.Models.Requests.PromptRequestModel request)
	{
		Id = id;
		Request = request;
	}

	/// <summary>
	///		Comprueba si se debe comprobar la generación
	/// </summary>
	internal bool MustCheck() => Status == GenerationStatus.Processing && NextSearch < DateTime.UtcNow;

	/// <summary>
	///		Comprueba si se debe descargar
	/// </summary>
	internal bool MustDownload() => Status == GenerationStatus.Generated && NextSearch < DateTime.UtcNow;

	/// <summary>
	///		Marca que se ha finalizado la generación de la imagen
	/// </summary>
	internal void MarkGenerated(string message, bool error)
	{
		Message = message;
		if (error)
			Status = GenerationStatus.Error;
		else
			Status = GenerationStatus.Generated;
	}

	/// <summary>
	///		Marca que se ha finalizado la descarga de la imagen
	/// </summary>
	internal void MarkDownloaded(string message, bool error)
	{
		Message = message;
		if (error)
			Status = GenerationStatus.Error;
		else
			Status = GenerationStatus.Success;
	}

	/// <summary>
	///		Añade una imagen descargada
	/// </summary>
	internal void AddImage(string id, string model, string seed, bool censored, string image, string extension)
	{
		Images.Add(new GenerationImageModel(id, model, seed, censored, image, extension));
	}

	/// <summary>
	///		Marca los datos
	/// </summary>
	internal void Mark(ProcessResultModel result, bool pendingDownload)
	{
		// Cambia los datos
		Message = result.Message ?? result.Status.ToString();
		// Cambia los datos dependiendo del estado
		switch (result.Status)
		{
			case ProcessResultModel.StatusType.Processing:
					Status = GenerationStatus.Processing;
					QueuePosition = result.QueuePosition;
					Finished = result.Finished;
					Processing = result.Processing;
					NextSearch = DateTime.UtcNow + result.Expected;
				break;
			case ProcessResultModel.StatusType.Success:
					QueuePosition = result.QueuePosition;
					Finished = result.Finished;
					Processing = result.Processing;
					if (pendingDownload)
						Status = GenerationStatus.Generated;
					else
						Status = GenerationStatus.Success;
				break;
			case ProcessResultModel.StatusType.Error:
					Status = GenerationStatus.Error;
				break;
		}
	}

	/// <summary>
	///		Código de generación
	/// </summary>
	public string Id { get; }

	/// <summary>
	///		Solicitud que ha generado este resultado
	/// </summary>
	public Domain.Models.Requests.PromptRequestModel Request { get; }

	/// <summary>
	///		Estado
	/// </summary>
	public GenerationStatus Status { get; private set; } = GenerationStatus.Processing;

	/// <summary>
	///		Mensaje
	/// </summary>
	public string Message { get; private set; } = default!;

	/// <summary>
	///		Trabajos finalizados
	/// </summary>
	public int Finished { get; set; }

	/// <summary>
	///		Trabajos en proceso
	/// </summary>
	public int Processing { get; set; }

	/// <summary>
	///		Posición en la cola
	/// </summary>
	public int QueuePosition { get; set; }

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; } = DateTime.UtcNow;

	/// <summary>
	///		Fecha de siguiente consulta
	/// </summary>
	public DateTime NextSearch { get; private set; } = DateTime.UtcNow;

	/// <summary>
	///		Imágenes generadas
	/// </summary>
	public List<GenerationImageModel> Images { get; } = new();
}