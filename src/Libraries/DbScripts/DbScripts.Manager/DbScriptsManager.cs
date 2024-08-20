using System.Data;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Interpreter;
using Bau.Libraries.DbScripts.Manager.Models;
using Microsoft.Extensions.Logging;

namespace Bau.Libraries.DbScripts.Manager;

/// <summary>
///		Manager para la ejecución de scripts
/// </summary>
public class DbScriptsManager
{
	public DbScriptsManager(ILogger logger)
	{
		Logger = logger;
		ConnectionManager = new Connections.ConnectionManager(this);
	}

	/// <summary>
	///		Carga el esquema de una conexión
	/// </summary>
	public async Task LoadSchemaAsync(ConnectionModel connection, bool includeSystemTables, CancellationToken cancellationToken)
	{
		await ConnectionManager.LoadSchemaAsync(connection, includeSystemTables, cancellationToken);
	}

	/// <summary>
	///		Ejecuta una consulta sobre una conexión
	/// </summary>
	public async Task ExecuteQueryAsync(QueryModel query, CancellationToken cancellationToken)
	{
		await Task.Run(() => new SqlCommandController(this).ExecuteAsync(query, cancellationToken));
	}

	/// <summary>
	///		Obtiene un <see cref="DataTable"/> paginada con una consulta sobre una conexión
	/// </summary>
	public async Task<DataTable?> GetDatatableAsync(QueryModel query, CancellationToken cancellationToken)
	{
		return await Task.Run(() => new SqlCommandController(this).GetDataTableAsync(query, cancellationToken));
	}

	/// <summary>
	///		Obtiene el datareader de una consulta
	/// </summary>
	public async Task<System.Data.Common.DbDataReader?> ExecuteReaderAsync(QueryModel query, CancellationToken cancellationToken)
	{
		return await Task.Run(() => new SqlCommandController(this).ExecuteReaderAsync(query, cancellationToken));
	}

	/// <summary>
	///		Obtiene el plan de ejecución de una consulta
	/// </summary>
	public async Task<DataTable?> GetExecutionPlanAsync(QueryModel query, CancellationToken cancellationToken)
	{
		return await Task.Run(() => new SqlCommandController(this).GetExecutionPlanAsync(query, cancellationToken));
	}

	/// <summary>
	///		Obtiene un proveedor de base de datos
	/// </summary>
	public LibDbProviders.Base.IDbProvider GetDbProvider(ConnectionModel connection) => ConnectionManager.GetDbProvider(connection);

	/// <summary>
	///		Obtiene el esquema de una conexión
	/// </summary>
	public async Task<LibDbProviders.Base.Schema.SchemaDbModel> GetDbSchemaAsync(ConnectionModel connection, bool includeSystemTables, CancellationToken cancellationToken)
	{
		return await ConnectionManager.GetSchemaAsync(connection, includeSystemTables, cancellationToken);
	}

	/// <summary>
	///		Obtiene la cadena de consulta de una tabla (SELECT Fields FROM Table)
	/// </summary>
	public string GetSqlQuery(ConnectionTableModel table) => ConnectionManager.GetSqlQuery(table);

	/// <summary>
	///		Obtiene el nombre completo de una tabla [Schema].[Table] con lo separadores adecuados
	/// </summary>
	public string GetSqlTableName(ConnectionTableModel table) => ConnectionManager.GetSqlTableName(table);

	/// <summary>
	///		Obtiene el nombre completo de un campo [Table].[Field] con lo separadores adecuados
	/// </summary>
	public string GetSqlFieldName(ConnectionTableFieldModel field) => ConnectionManager.GetSqlFieldName(field);

	/// <summary>
	///		Manager de log
	/// </summary>
	internal ILogger Logger { get; }

	/// <summary>
	///		Manager de conexiones
	/// </summary>
	internal Connections.ConnectionManager ConnectionManager { get; }
}
