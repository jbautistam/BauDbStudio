namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Paso de un proyecto
/// </summary>
public class RestStepModel : BaseStepModel
{
	/// <summary>
	///		Método de llamada a Rest
	/// </summary>
	public enum RestMethod
	{
		Post = 1,
		Put,
		Patch,
		Get,
		Delete,
		Options
	}

	/// <summary>
	///		Nombre del paso
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción del paso
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Conexión a la que se asocia el paso
	/// </summary>
	public string ConnectionId { get; set; } = default!;

	/// <summary>
	///		Método
	/// </summary>
	public RestMethod Method { get; set; }

	/// <summary>
	///		Url
	/// </summary>
	public string Url { get; set; } = default!;

	/// <summary>
	///		Cabeceras
	/// </summary>
	public ParametersCollectionModel Headers { get; } = [];

	/// <summary>
	///		Contenido
	/// </summary>
	public string? Content { get; set; }
}
