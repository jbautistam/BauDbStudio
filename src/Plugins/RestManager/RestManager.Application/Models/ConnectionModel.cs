namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Clase con los datos de una conexión
/// </summary>
public class ConnectionModel
{
	/// <summary>
	///		Id de la conexión
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Nombre de la conexión
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción de la conexión
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	///		Url de la colección
	/// </summary>
	public Uri? Url { get; set; }

	/// <summary>
	///		Cabeceras de la conexión (predeterminadas)
	/// </summary>
	public ParametersCollectionModel Headers { get; } = new();
}
