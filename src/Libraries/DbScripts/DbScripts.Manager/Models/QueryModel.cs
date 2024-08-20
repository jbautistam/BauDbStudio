using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbScripts.Manager.Models;

/// <summary>
///		Clase con los datos de una consulta
/// </summary>
public class QueryModel
{
	public QueryModel(ConnectionModel connection)
	{
		Connection = connection;
	}

	/// <summary>
	///		Conexión
	/// </summary>
	public ConnectionModel Connection { get; }

	/// <summary>
	///		Consulta SQL o script
	/// </summary>
	public string Sql { get; set; } = string.Empty;

	/// <summary>
	///		Indica si se debe interpretar la consulta
	/// </summary>
	public bool ParseQuery { get; set; }

	/// <summary>
	///		Argumentos / parámetros de la consulta
	/// </summary>
	public ArgumentListModel Arguments { get; } = new();

	/// <summary>
	///		Tiempo de espera de la consulta
	/// </summary>
	public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

	/// <summary>
	///		Modo de paginación
	/// </summary>
	public PaginationModel Pagination { get; } = new();
}
