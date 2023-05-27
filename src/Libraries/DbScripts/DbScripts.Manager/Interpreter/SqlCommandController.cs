using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Models;
using Bau.Libraries.LibDbScripts.Parser;

namespace Bau.Libraries.DbScripts.Manager.Interpreter;

/// <summary>
///		Controlador para el proceso de scripts de SQL
/// </summary>
internal class SqlCommandController
{
	internal SqlCommandController(DbScriptsManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Obtiene el datatable de una consulta
	/// </summary>
	internal async Task<DataTable?> GetDataTableAsync(Models.QueryModel query, CancellationToken cancellationToken)
	{
		if (query.ParseQuery)
			return await GetDataTableAsync(Manager.GetDbProvider(query.Connection), query.Sql, query.Arguments, query.Pagination.Page, query.Pagination.PageSize,
										   query.Timeout, cancellationToken);
		else
			return await GetDatatableParsedAsync(Manager.GetDbProvider(query.Connection), query.Sql, query.Arguments, query.Pagination.Page, query.Pagination.PageSize,
												 query.Timeout, cancellationToken);
	}

	/// <summary>
	///		Obtiene el datatable de una consulta
	/// </summary>
	private async Task<DataTable?> GetDataTableAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
													 int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
	{
		DataTable? result = null;

			// Normaliza los argumentos
			if (arguments == null)
				arguments = new Models.ArgumentListModel();
			// Log
			Manager.Logger.LogInformation("Execute query");
			// Obtiene la tabla
			if (string.IsNullOrWhiteSpace(query))
				Manager.Logger.LogError("The query is empty");
			else
			{
				List<SqlSectionModel> scripts = new SqlParser().Tokenize(query, arguments.Constants.ToDictionary(), out string error);

					if (!string.IsNullOrWhiteSpace(error))
						Manager.Logger.LogError(error);
					else
					{
						ParametersDbCollection parametersDb = ConvertParameters(provider, arguments.Parameters);

							// Obtiene el datatable
							foreach (SqlSectionModel script in scripts)
								if (script.Type == SqlSectionModel.SectionType.Sql)
								{
									string sql = provider.SqlHelper.ConvertSqlNoParameters(script.Content, parametersDb).TrimIgnoreNull();

										if (!string.IsNullOrWhiteSpace(sql))
										{
											// Log
											Manager.Logger.LogDebug($"Executing: {sql}");
											// Obtiene la consulta
											if (sql.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase) ||
												sql.StartsWith("WITH ", StringComparison.CurrentCultureIgnoreCase) ||
												sql.StartsWith("EXECUTE ", StringComparison.CurrentCultureIgnoreCase) ||
												sql.StartsWith("EXEC ", StringComparison.CurrentCultureIgnoreCase))
											{
												if (pageSize == 0)
													result = await provider.GetDataTableAsync(sql, parametersDb, CommandType.Text, timeout, cancellationToken);
												else
													result = await provider.GetDataTableAsync(sql, parametersDb, CommandType.Text, actualPage, pageSize, timeout, cancellationToken);
											}
											else
												result = await ExecuteScalarQueryAsync(provider, sql, timeout, cancellationToken);
										}
								}
							// Log
							Manager.Logger.LogInformation("End query");
					}
			}
			// Devuelve la última tabla obtenida
			return result;
	}

	/// <summary>
	///		Obtiene el datatable de una consulta sin interpretarla
	/// </summary>
	private async Task<DataTable> GetDatatableParsedAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
														  int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
	{
		ParametersDbCollection parametersDb = ConvertParameters(provider, (arguments ?? new()).Parameters);

			// Obtiene la tabla
			if (pageSize == 0)
				return await provider.GetDataTableAsync(query, parametersDb, CommandType.Text, timeout, cancellationToken);
			else
				return await provider.GetDataTableAsync(query, parametersDb, CommandType.Text, actualPage, pageSize, timeout, cancellationToken);
	}

	/// <summary>
	///		Obtiene el datareader de una consulta
	/// </summary>
	internal async Task<DbDataReader?> ExecuteReaderAsync(Models.QueryModel query, CancellationToken cancellationToken)
	{
		if (query.ParseQuery)
			return await ExecuteReaderAsync(Manager.GetDbProvider(query.Connection), query.Sql, query.Arguments, query.Timeout, cancellationToken);
		else
			return await ExecuteParsedReaderAsync(Manager.GetDbProvider(query.Connection), query.Sql, query.Arguments,
												  query.Timeout, cancellationToken);
	}

	/// <summary>
	///		Obtiene el datareader de una consulta
	/// </summary>
	private async Task<DbDataReader?> ExecuteReaderAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
														TimeSpan timeout, CancellationToken cancellationToken)
	{
		List<SqlSectionModel> scripts = new SqlParser().Tokenize(query, arguments.Constants.ToDictionary(), out string error);

			if (!string.IsNullOrWhiteSpace(error))
				throw new Exception(error);
			else
			{
				ParametersDbCollection parametersDb = ConvertParameters(provider, arguments.Parameters);

					// Obtiene el datareader
					foreach (SqlSectionModel script in scripts)
						if (script.Type == SqlSectionModel.SectionType.Sql)
						{
							string sql = provider.SqlHelper.ConvertSqlNoParameters(script.Content, parametersDb).TrimIgnoreNull();

								if (!string.IsNullOrWhiteSpace(sql))
									return await provider.ExecuteReaderAsync(sql, null, CommandType.Text, timeout, cancellationToken);
						}
					// Si ha llegado hasta aquí es porque no ha leído nada
					return null;
			}
	}

	/// <summary>
	///		Obtiene el datareader de una consulta ya interpretada
	/// </summary>
	private async Task<DbDataReader> ExecuteParsedReaderAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
															  TimeSpan timeout, CancellationToken cancellationToken)
	{
		return await provider.ExecuteReaderAsync(query, ConvertParameters(provider, arguments.Parameters), CommandType.Text, timeout, cancellationToken);
	}

	/// <summary>
	///		Ejecuta una consulta escalar
	/// </summary>
	private async Task<DataTable> ExecuteScalarQueryAsync(IDbProvider provider, string query, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long rows = await provider.ExecuteAsync(query, null, CommandType.Text, timeout, cancellationToken);
		DataTable table = new DataTable();
		DataRow row = table.NewRow();

			// Añade la columna
			table.Columns.Add("Rows", typeof(long));
			// Añade el valor con el número de filas
			row[0] = rows;
			// Añade la fila a la tabla
			table.Rows.Add(row);
			// Devuelve la tabla resultante
			return table;
	}
	
	/// <summary>
	///		Ejecuta los comandos de una cadena SQL
	/// </summary>
	internal async Task ExecuteAsync(Models.QueryModel query, CancellationToken cancellationToken)
	{
		if (query.ParseQuery)
			await ExecuteAsync(Manager.GetDbProvider(query.Connection), query.Sql, query.Arguments, query.Timeout, cancellationToken);
		else
			throw new NotImplementedException("Can't execute query not parsed");
	}

	/// <summary>
	///		Ejecuta los comandos de una cadena SQL
	/// </summary>
	private async Task ExecuteAsync(IDbProvider provider, string sql, Models.ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
	{
		// Log
		Manager.Logger.LogInformation("Execute script");
		// Ejecuta la consulta
		if (string.IsNullOrWhiteSpace(sql))
			Manager.Logger.LogError("The query is empty");
		else
		{
			List<SqlSectionModel> scripts = new SqlParser().Tokenize(sql, arguments.Constants.ToDictionary(), out string error);

				if (!string.IsNullOrWhiteSpace(error))
					Manager.Logger.LogError(error);
				else
				{
					int scriptsExecuted = 0;

						// Ejecuta los scripts
						if (scripts.Count > 0)
							scriptsExecuted = await ExecuteCommandsAsync(provider, scripts, 
																		 ConvertParameters(provider, arguments.Parameters), 
																		 timeout, cancellationToken);
						// Log
						if (scriptsExecuted == 0)
							Manager.Logger.LogError("The query is empty");
						else
							Manager.Logger.LogInformation($"{scriptsExecuted} command/s executed");
				}
		}
	}

	/// <summary>
	///		Ejecuta una serie de comandos
	/// </summary>
	private async Task<int> ExecuteCommandsAsync(IDbProvider provider, 
												 List<SqlSectionModel> commands, ParametersDbCollection parametersDb, 
												 TimeSpan timeout, CancellationToken cancellationToken)
	{
		int scriptsExecuted = 0;

			// Ejecuta las consultas
			foreach (SqlSectionModel command in commands)
				if (!cancellationToken.IsCancellationRequested && command.Type == SqlSectionModel.SectionType.Sql)
				{
					// Log
					Manager.Logger.LogDebug($"Execute: {command.Content}");
					// Ejecuta la cadena SQL
					await provider.ExecuteAsync(provider.SqlHelper.ConvertSqlNoParameters(command.Content, parametersDb), 
												null, CommandType.Text, timeout, cancellationToken);
					// Indica que se ha ejecutado una sentencia
					scriptsExecuted++;
				}
			// Devuelve el número de comandos ejecutados
			return scriptsExecuted;
	}

	/// <summary>
	///		Obtiene el plan de ejecución de una consulta
	/// </summary>
	internal async Task<DataTable?> GetExecutionPlanAsync(Models.QueryModel query, CancellationToken cancellationToken)
	{
		DataTable? result = null;

			// Log
			Manager.Logger.LogInformation("Get execution plan");
			// Obtiene la tabla
			if (string.IsNullOrWhiteSpace(query.Sql))
				Manager.Logger.LogError("The query is empty");
			else
			{
				IDbProvider provider = Manager.GetDbProvider(query.Connection);
				List<SqlSectionModel> scripts = new SqlParser().Tokenize(query.Sql, query.Arguments.Constants.ToDictionary(), out string error);

					if (!string.IsNullOrWhiteSpace(error))
						Manager.Logger.LogError(error);
					else
					{
						ParametersDbCollection parametersDb = ConvertParameters(provider, query.Arguments.Parameters);

							// Obtiene el datatable
							foreach (SqlSectionModel script in scripts)
								if (script.Type == SqlSectionModel.SectionType.Sql)
								{
									string sql = provider.SqlHelper.ConvertSqlNoParameters(script.Content, parametersDb).TrimIgnoreNull();

										if (!string.IsNullOrWhiteSpace(sql))
										{
											// Log
											Manager.Logger.LogInformation($"Get execution plan: {sql}");
											// Obtiene el plan de ejecución
											result = await provider.GetExecutionPlanAsync(sql, null, CommandType.Text, query.Timeout, cancellationToken);
										}
								}
							// Log
							Manager.Logger.LogInformation("End query");
					}
			}
			// Devuelve la última tabla obtenida
			return result;
	}

	/// <summary>
	///		Crea la lista de parámetros a pasar a la consulta
	/// </summary>
	private ParametersDbCollection ConvertParameters(IDbProvider provider, NormalizedDictionary<object> parameters)
	{
		ParametersDbCollection parametersDb = new ParametersDbCollection();

			// Convierte los parámetros
			foreach ((string key, object value) in parameters.Enumerate())
				parametersDb.Add($"{provider.SqlHelper.ParameterPrefix}{key}", value);
			// Devuelve la colección de parámetros para la base de datos
			return parametersDb;
	}

	/// <summary>
	///		Manager
	/// </summary>
	internal DbScriptsManager Manager { get; }
}