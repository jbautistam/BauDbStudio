using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.LibDbScripts.Parser;

namespace Bau.Libraries.DbStudio.Application.Connections
{
	/// <summary>
	///		Controlador para el proceso de scripts de SQL
	/// </summary>
	internal class ScriptSqlController
	{
		internal ScriptSqlController(ConnectionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Obtiene el datatable de una consulta
		/// </summary>
		internal async Task<DataTable> GetDataTableAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
														 int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{
			DataTable result = null;

				// Obtiene la tabla
				using (BlockLogModel block = Manager.SolutionManager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Execute query"))
				{
					if (string.IsNullOrWhiteSpace(query))
						block.Error("The query is empty");
					else
					{
						List<SqlSectionModel> scripts = new SqlParser().Tokenize(query, arguments.Constants.ToDictionary(), out string error);

							if (!string.IsNullOrWhiteSpace(error))
								block.Error(error);
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
													block.Info($"Executing: {sql}");
													// Obtiene la consulta
													if (sql.TrimIgnoreNull().StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase))
													{
														if (pageSize == 0)
															result = await provider.GetDataTableAsync(sql, null, CommandType.Text, timeout, cancellationToken);
														else
															result = await provider.GetDataTableAsync(sql, null, CommandType.Text, actualPage, pageSize, timeout, cancellationToken);
													}
													else
														result = await ExecuteScalarQueryAsync(provider, sql, timeout, cancellationToken);
												}
										}
									// Log
									block.Info("End query");
							}
					}
				}
				// Devuelve la última tabla obtenida
				return result;
		}

		/// <summary>
		///		Obtiene el datareader de una consulta
		/// </summary>
		internal async Task<DbDataReader> ExecuteReaderAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
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
		internal async Task ExecuteAsync(IDbProvider provider, string sql, Models.ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = Manager.SolutionManager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Execute script"))
			{
				if (string.IsNullOrWhiteSpace(sql))
					block.Error("The query is empty");
				else
				{
					List<SqlSectionModel> scripts = new SqlParser().Tokenize(sql, arguments.Constants.ToDictionary(), out string error);

						if (!string.IsNullOrWhiteSpace(error))
							block.Error(error);
						else
						{
							int scriptsExecuted = 0;

								// Ejecuta los scripts
								if (scripts.Count > 0)
									scriptsExecuted = await ExecuteCommandsAsync(block, provider, scripts, 
																				 ConvertParameters(provider, arguments.Parameters), 
																				 timeout, cancellationToken);
								// Log
								if (scriptsExecuted == 0)
									block.Error("The query is empty");
								else
									block.Info($"{scriptsExecuted} command/s executed");
						}
				}
			}
		}

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		private async Task<int> ExecuteCommandsAsync(BlockLogModel block, IDbProvider provider, 
													 List<SqlSectionModel> commands, ParametersDbCollection parametersDb, 
													 TimeSpan timeout, CancellationToken cancellationToken)
		{
			int scriptsExecuted = 0;

				// Ejecuta las consultas
				foreach (SqlSectionModel command in commands)
					if (!cancellationToken.IsCancellationRequested && command.Type == SqlSectionModel.SectionType.Sql)
					{
						// Log
						block.Debug($"Execute: {command.Content}");
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
		internal async Task<DataTable> GetExecutionPlanAsync(IDbProvider provider, string query, Models.ArgumentListModel arguments, 
															 TimeSpan timeout, CancellationToken cancellationToken)
		{
			DataTable result = null;

				// Obtiene la tabla
				using (BlockLogModel block = Manager.SolutionManager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Get execution plan"))
				{
					if (string.IsNullOrWhiteSpace(query))
						block.Error("The query is empty");
					else
					{
						List<SqlSectionModel> scripts = new SqlParser().Tokenize(query, arguments.Constants.ToDictionary(), out string error);

							if (!string.IsNullOrWhiteSpace(error))
								block.Error(error);
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
													block.Info($"Get execution plan: {sql}");
													// Obtiene el plan de ejecución
													// EXPLAIN [EXTENDED | CODEGEN] statement
													result = await provider.GetExecutionPlanAsync($"EXPLAIN EXTENDED {sql}", null, CommandType.Text, timeout, cancellationToken);
												}
										}
									// Log
									block.Info("End query");
							}
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
		///		Manager de conexiones
		/// </summary>
		internal ConnectionManager Manager { get; }
	}
}