namespace Bau.Libraries.LibStableHorde.Api.Dtos;

/// <summary>
///		Datos de un modelo
/// </summary>
public class ModelDto
{
	/// <summary>
	///		Tipo del modelo
	/// </summary>
	public enum ModelType
	{
		/// <summary>Modelo para generación de texto</summary>
		Text,
		/// <summary>Modelo para generación de imagen</summary>
		Image
	}

	/// <summary>
	///		Nombre del modelo
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Número de nodos que están ejecutando este modelo
	/// </summary>
	public int Count { get; set; }

	/// <summary>
	///		Velocidad media de generación de este modelo
	/// </summary>
	public double Performance { get; set; }

	/// <summary>
	///		Tiempo medio de espera para la generación de este modelo
	/// </summary>
	public double Queued { get; set; }

	/// <summary>
	///		Número de trabajos esperando a generarse con este modelo
	/// </summary>
	public double Jobs { get; set; }

	/// <summary>
	///		Tiempo de vida estimado en segundos que se mantiene el modelo en la cola antes de limpiarse
	/// </summary>
	public int Eta { get; set; }

	/// <summary>
	///		Tipo de modelo
	/// </summary>
	public ModelType Type { get; set; } = default!;
}
