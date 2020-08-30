using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.DbScripts.Manager.Connections.Models;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbScripts.Manager.Connections.Interpreter
{
	/// <summary>
	///		Procesador de consultas SQL
	/// </summary>
	internal class ScriptSqlProcessor : LibDbScripts.Interpreter.Interfaces.IDbScriptExecutor
	{
		// Variables privadas
		private IDbProvider _dbProvider;
		private ArgumentListModel _arguments;
		private TimeSpan _timeout;

		public ScriptSqlProcessor(ScriptSqlInterpreter interpreter, IDbProvider dbProvider, ArgumentListModel arguments, TimeSpan timeout)
		{
			Interpreter = interpreter;
			_dbProvider = dbProvider;
			_arguments = arguments;
			_timeout = timeout;
		}

		/// <summary>
		///		Ejecuta una sentencia SQL
		/// </summary>
		public async Task<(bool executed, List<string> errors)> ExecuteAsync(string sql, Dictionary<string, object> parameters, CancellationToken cancellationToken)
		{
			// Ejecuta la sentencia
			if (!string.IsNullOrWhiteSpace(sql))
				await ExecuteAsync(_dbProvider, sql, GetArguments(parameters), _timeout, cancellationToken);
			// Indica que se ha ejecutado
			return (Errors.Count == 0, Errors);
		}

		/// <summary>
		///		Ejecuta los comandos de una cadena SQL
		/// </summary>
		//TODO: Esta rutina es igual a la que existe en ScriptSqlController
		private async Task ExecuteAsync(IDbProvider provider, string sql, ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = Interpreter.ConnectionManager.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, "Execute script"))
			{
				if (string.IsNullOrWhiteSpace(sql))
					Errors.Add("The query is empty");
				else
				{
					List<SqlSectionModel> scripts = new SqlParser().Tokenize(sql, arguments.Constants.ToDictionary(), out string error);

						if (!string.IsNullOrWhiteSpace(error))
							Errors.Add(error);
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
									Errors.Add("The query is empty");
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
													null, System.Data.CommandType.Text, timeout, cancellationToken);
						// Indica que se ha ejecutado una sentencia
						scriptsExecuted++;
					}
				// Devuelve el número de comandos ejecutados
				return scriptsExecuted;
		}

		/// <summary>
		///		Crea una lista de argumentos con los argumentos predeterminados y los parámetros pasados a la consulta
		/// </summary>
		private ArgumentListModel GetArguments(Dictionary<string, object> parameters)
		{
			ArgumentListModel arguments = new ArgumentListModel();

				// Agrega las constantes iniciales
				foreach ((string key, object value) in _arguments.Constants.Enumerate())
					arguments.Constants.Add(key, value);
				// Agrega los parámetros iniciales
				foreach ((string key, object value) in _arguments.Parameters.Enumerate())
					arguments.Parameters.Add(key, value);
				// y agrega los parámetros obtenidos en la interpretación
				foreach (KeyValuePair<string, object> parameter in parameters)
					arguments.Parameters.Add(parameter.Key, parameter.Value);
				// Devuelve la lista de argumentos
				return arguments;
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
		///		Escribe un mensaje en la consola
		/// </summary>
		public void ConsoleWriteLine(string message)
		{
			Interpreter.ConnectionManager.Manager.Logger.Default.LogItems.Console(message);
		}

		/// <summary>
		///		Procesador
		/// </summary>
		internal ScriptSqlInterpreter Interpreter { get; }

		/// <summary>
		///		Errores
		/// </summary>
		private List<string> Errors { get; } = new List<string>();
	}
}
