namespace Bau.Libraries.LibAiImageGeneration.Models;

/// <summary>
///		Datos de una imagen generada
/// </summary>
public class GenerationImageModel
{
	internal GenerationImageModel(string id, string model, string seed, bool censored, string image, string extension)
	{
		Id = id;
		Model = model;
		Seed = seed;
		Censored = censored;
		UrlImage = image;
		Extension = extension;
	}

	/// <summary>
	///		Id de la imagen
	/// </summary>
	public string Id { get; }

	/// <summary>
	///		Modelo con el que se ha generado
	/// </summary>
	public string Model { get; }

	/// <summary>
	///		Semilla de generación
	/// </summary>
	public string Seed { get; }

	/// <summary>
	///		Indica que está censurado
	/// </summary>
	public bool Censored { get; }

	/// <summary>
	///		Imagen (Url de la imagen)
	/// </summary>
	public string UrlImage { get; }

	/// <summary>
	///		Extensión de la imagen (si la imagen viene una URL)
	/// </summary>
	public string Extension { get; }

	/// <summary>
	///		Nombre de archivo descargado
	/// </summary>
	public string? FileName { get; set; }

	/// <summary>
	///		Indica si se ha descargado la imagen
	/// </summary>
	public bool Downloaded { get; set; }

	/// <summary>
	///		Error de descarga o generación
	/// </summary>
	public string? Error { get; set; }

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; } = DateTime.UtcNow;
}