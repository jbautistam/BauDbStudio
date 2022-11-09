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
		public async Task ExecuteQueryAsync(QueryModel query, CancellationToken cancellationToken)
		{
			await DbScriptsManager.ExecuteQueryAsync(query, cancellationToken);
		}

		/// <summary>
		///		Obtiene un <see cref="DataTable"/> paginada con una consulta sobre una conexión sin interpretarla
		/// </summary>
		public async Task<DataTable> GetDatatableAsync(QueryModel query, CancellationToken cancellationToken)
		{
			return await DbScriptsManager.GetDatatableAsync(query, cancellationToken);
		}

		/// <summary>
		///		Obtiene el datareader de una consulta
		/// </summary>
		public async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(QueryModel query, CancellationToken cancellationToken)
		{
			return await DbScriptsManager.ExecuteReaderAsync(query, cancellationToken);
		}

		/// <summary>
		///		Obtiene el plan de ejecución de una consulta
		/// </summary>
		public async Task<DataTable> GetExecutionPlanAsync(QueryModel query, CancellationToken cancellationToken)
		{
			return await DbScriptsManager.GetExecutionPlanAsync(query, cancellationToken);
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