namespace StableHorde.Console.Controller.Models;

/// <summary>
///		Datos de una imagen generada
/// </summary>
public class GenerationImageModel
{
	public GenerationImageModel(string id, string model, string seed, bool censored, string image)
	{
		Id = id;
		Model = model;
		Seed = seed;
		Censored = censored;
		Image = image;
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
	public string Image { get; }

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
