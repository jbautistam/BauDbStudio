using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Models;

namespace Bau.Libraries.DbStudio.Application;

/// <summary>
///		Manager de soluciones
/// </summary>
public class SolutionManager
{
	// Enumerados públicos
	/// <summary>
	///		Tipo de formato de los archivos de salida
	/// </summary>
	public enum FormatType
	{
		/// <summary>Archivos CSV</summary>
		Csv,
		/// <summary>Archivos parquet</summary>
		Parquet,
		/// <summary>Archivos SQL (INSERT INTO)</summary>
		Sql
	}

	public SolutionManager(ILogger logger)
	{
		Logger = logger;
		DbScriptsManager = new DbScripts.Manager.DbScriptsManager(logger);
	}

	/// <summary>
	///		Carga los datos de configuración
	/// </summary>
	public SolutionModel LoadConfiguration(string fileName) => new Repository.SolutionRepository().Load(fileName);

	/// <summary>
	///		Graba los datos de una solución
	/// </summary>
	public void SaveSolution(SolutionModel solution, string fileName)
	{
		new Repository.SolutionRepository().Save(solution, fileName);
	}

	/// <summary>
	///		Carga el esquema de una conexión
	/// </summary>
	public async Task LoadSchemaAsync(ConnectionModel connection, bool includeSystemTables, CancellationToken cancellationToken)
	{
		await DbScriptsManager.LoadSchemaAsync(connection, new DbScripts.Manager.DbScriptsManager.SchemaOptions(true, true, false, includeSystemTables), cancellationToken);
	}

	/// <summary>
	///		Ejecuta una consulta sobre una conexión
	/// </summary>
	public async Task ExecuteQueryAsync(QueryModel query, CancellationToken cancellationToken)
	{
		await DbScriptsManager.ExecuteQueryAsync(query, cancellationToken);
	}

	/// <summary>
	///		Obtiene un <see cref="DataTable"/> paginada con una consulta sobre una conexión sin interpretarla
	/// </summary>
	public async Task<DataTable?> GetDatatableAsync(QueryModel query, CancellationToken cancellationToken)
	{
		return await DbScriptsManager.GetDatatableAsync(query, cancellationToken);
	}

	/// <summary>
	///		Obtiene el datareader de una consulta
	/// </summary>
	public async Task<System.Data.Common.DbDataReader?> ExecuteReaderAsync(QueryModel query, CancellationToken cancellationToken)
	{
		return await DbScriptsManager.ExecuteReaderAsync(query, cancellationToken);
	}

	/// <summary>
	///		Obtiene el plan de ejecución de una consulta
	/// </summary>
	public async Task<DataTable?> GetExecutionPlanAsync(QueryModel query, CancellationToken cancellationToken)
	{
		return await DbScriptsManager.GetExecutionPlanAsync(query, cancellationToken);
	}

	/// <summary>
	///		Obtiene la cadena de consulta de una tabla (SELECT Fields FROM Table)
	/// </summary>
	public string GetSqlQuery(ConnectionTableModel table) => DbScriptsManager.GetSqlQuery(table);

	/// <summary>
	///		Obtiene el nombre completo de una tabla [Schema].[Table] con lo separadores adecuados
	/// </summary>
	public string GetSqlTableName(ConnectionTableModel table) => DbScriptsManager.GetSqlTableName(table);

	/// <summary>
	///		Obtiene el nombre completo de un campo [Table].[Field] con lo separadores adecuados
	/// </summary>
	public string GetSqlFieldName(ConnectionTableFieldModel field) => DbScriptsManager.GetSqlFieldName(field);

	/// <summary>
	///		Manager de log
	/// </summary>
	public ILogger Logger { get; }

	/// <summary>
	///		Manager para el tratamiento de scripts
	/// </summary>
	internal DbScripts.Manager.DbScriptsManager DbScriptsManager { get; }
}