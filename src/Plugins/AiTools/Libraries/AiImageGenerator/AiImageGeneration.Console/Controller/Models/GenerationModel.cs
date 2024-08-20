namespace StableHorde.Console.Controller.Models;

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

	public GenerationModel(string id)
	{
		Id = id;
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
	///		Marca que se ha consultado el resultado
	/// </summary>
	internal void MarkCheck(int jobsFinished, int jobsProcessing, int queuePosition, TimeSpan next)
	{
		NextSearch = DateTime.UtcNow + next;
	}

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
	internal void AddImage(string id, string model, string seed, bool censored, string image)
	{
		Images.Add(new GenerationImageModel(id, model, seed, censored, image));
	}

	/// <summary>
	///		Código de generación
	/// </summary>
	public string Id { get; }

	/// <summary>
	///		Estado
	/// </summary>
	public GenerationStatus Status { get; private set; } = GenerationStatus.Processing;

	/// <summary>
	///		Mensaje
	/// </summary>
	public string Message { get; private set; } = default!;

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
