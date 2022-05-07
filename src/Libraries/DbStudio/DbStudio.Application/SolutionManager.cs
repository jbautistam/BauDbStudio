using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Models;

namespace Bau.Libraries.DbStudio.Application
{
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
			Parquet
		}

		public SolutionManager(LogManager logger)
		{
			Logger = logger;
			DbScriptsManager = new DbScripts.Manager.DbScriptsManager(logger);
		}

		/// <summary>
		///		Carga los datos de configuración
		/// </summary>
		public SolutionModel LoadConfiguration(string fileName)
		{
			return new Repository.SolutionRepository().Load(fileName);
		}

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
		public async Task LoadSchemaAsync(ConnectionModel connection, CancellationToken cancellationToken)
		{
			await DbScriptsManager.LoadSchemaAsync(connection, cancellationToken);
		}

		/// <summary>
		///		Ejecuta una consulta sobre una conexión
		/// </summary>
		public async Task ExecuteQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
											TimeSpan timeout, CancellationToken cancellationToken)
		{
			await DbScriptsManager.ExecuteQueryAsync(connection, query, arguments, timeout, cancellationToken);
		}

		/// <summary>
		///		Ejecuta una consulta interpretada sobre una conexión
		/// </summary>
		public async Task ExecuteInterpretedQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, CancellationToken cancellationToken)
		{
			await DbScriptsManager.ExecuteInterpretedQueryAsync(connection, query, arguments, cancellationToken);
		}

		/// <summary>
		///		Obtiene un <see cref="DataTable"/> paginada con una consulta sobre una conexión
		/// </summary>
		public async Task<DataTable> GetDatatableQueryAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
															int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await DbScriptsManager.GetDatatableQueryAsync(connection, query, arguments, actualPage, pageSize, timeout, cancellationToken);
		}

		/// <summary>
		///		Obtiene el datareader de una consulta
		/// </summary>
		public async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
																			  TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await DbScriptsManager.ExecuteReaderAsync(connection, query, arguments, timeout, cancellationToken);
		}

		/// <summary>
		///		Obtiene el plan de ejecución de una consulta
		/// </summary>
		public async Task<DataTable> GetExecutionPlanAsync(ConnectionModel connection, string query, ArgumentListModel arguments, 
														   TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await DbScriptsManager.GetExecutionPlanAsync(connection, query, arguments, timeout, cancellationToken);
		}

		/// <summary>
		///		Manager de log
		/// </summary>
		public LogManager Logger { get; }

		/// <summary>
		///		Manager para el tratamiento de scripts
		/// </summary>
		internal DbScripts.Manager.DbScriptsManager DbScriptsManager { get; }
	}
}