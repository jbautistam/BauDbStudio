namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Clase con los datos de una conexión
/// </summary>
public class ConnectionModel
{
	/// <summary>
	///		Obtiene las cabeceras predeterminadas
	/// </summary>
	public ParametersCollectionModel GetDefaultHeaders(string agent)
	{
		ParametersCollectionModel headers = [];

			// Añade las cabeceras predeterminadas
			headers.Add("Content-Type", "application/json");
			headers.Add("User-Agent", agent);
			headers.Add("Accept", "*/*");
			headers.Add("Accept-Encoding", "gzip, deflate, br");
			headers.Add("Connection", "keep-alive");
			// Devuelve las cabeceras
			return headers;
	}

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
	///		Datos de autenticación
	/// </summary>
	public AuthenticationModel Authentication { get; } = new();

	/// <summary>
	///		Tiempo de espera
	/// </summary>
	public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);

	/// <summary>
	///		Cabeceras de la conexión (predeterminadas)
	/// </summary>
	public ParametersCollectionModel Headers { get; } = [];
}
