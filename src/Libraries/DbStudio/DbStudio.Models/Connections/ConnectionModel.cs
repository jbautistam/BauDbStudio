namespace Bau.Libraries.DbStudio.Models.Connections;

/// <summary>
///		Clase con los datos de la conexión
/// </summary>
public class ConnectionModel : LibDataStructures.Base.BaseExtendedModel
{
	/// <summary>
	///		Tipo de conexión
	/// </summary>
	public enum ConnectionType
	{
		/// <summary>Spark</summary>
		Spark,
		/// <summary>Sql server</summary>
		SqlServer,
		/// <summary>Odbc</summary>
		Odbc,
		/// <summary>PostgreSql</summary>
		PostgreSql,
		/// <summary>SqLite</summary>
		SqLite,
		/// <summary>MySql</summary>
		MySql,
		/// <summary>DuckDb</summary>
		DuckDb
	}

	public ConnectionModel(SolutionModel solution)
	{
		Solution = solution;
	}

	/// <summary>
	///		Clona la conexión
	/// </summary>
	public ConnectionModel Clone()
	{
		ConnectionModel connection = new(Solution);

			// Asigna las propiedades
			connection.Name = Name;
			connection.Description = Description;
			connection.Type = Type;
			connection.Parameters.AddRange(Parameters);
			connection.TimeoutExecuteScript = TimeoutExecuteScript;
			// Devuelve la nueva conexión
			return connection;
	}

	/// <summary>
	///		Solución a la que se asocia la conexión
	/// </summary>
	public SolutionModel Solution { get; }

	/// <summary>
	///		Tipo de conexión
	/// </summary>
	public ConnectionType Type { get; set; } = ConnectionType.SqlServer;

	/// <summary>
	///		Parámetros de la conexión
	/// </summary>
	public LibDataStructures.Collections.NormalizedDictionary<string> Parameters { get; } = new();

	/// <summary>
	///		Timeout para la ejecución de scripts
	/// </summary>
	public TimeSpan TimeoutExecuteScript { get; set; }

	/// <summary>
	///		Tablas
	/// </summary>
	public List<ConnectionTableModel> Tables { get; } = [];

	/// <summary>
	///		Vistas
	/// </summary>
	public List<ConnectionTableModel> Views { get; } = [];
}
