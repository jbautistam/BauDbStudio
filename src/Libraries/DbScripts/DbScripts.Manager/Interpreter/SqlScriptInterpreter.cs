using Microsoft.Extensions.Logging;

using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbScripts.Interpreter;

namespace Bau.Libraries.DbScripts.Manager.Interpreter;

/// <summary>
///		Intérprete de ejecución de consultas SQL
/// </summary>
internal class SqlScriptInterpreter
{
	internal SqlScriptInterpreter(DbScriptsManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Ejecuta un script de SQL
	/// </summary>
	internal async Task ExecuteAsync(IDbProvider dbProvider, string query, ArgumentListModel arguments, TimeSpan timeout, CancellationToken cancellationToken)
	{
		SqlScriptExecutor executor = new SqlScriptExecutor(Manager, dbProvider, arguments, timeout);
		DbScriptsInterpreter interpreter = new DbScriptsInterpreter(executor, Manager.Logger);

			// Log
			Manager.Logger.LogInformation("Start script execution");
			// Ejecuta el archivo
			await interpreter.ExecuteAsync(query, null, cancellationToken);
			// Recopila los errores
			Errors.AddRange(interpreter.Errors);
			// y los pasa al log
			if (Errors.Count == 0)
				Manager.Logger.LogInformation("End script execution");
			else
			{
				string error = string.Empty;

					// Agrega los errores
					foreach (string inner in Errors)
						error += inner + Environment.NewLine;
					// log
					Manager.Logger.LogError($"Error when execute sql script: {Environment.NewLine}{error}");
			}
	}

	/// <summary>
	///		Manager
	/// </summary>
	internal DbScriptsManager Manager { get; }

	/// <summary>
	///		Errores
	/// </summary>
	internal List<string> Errors { get; } = new();
}
