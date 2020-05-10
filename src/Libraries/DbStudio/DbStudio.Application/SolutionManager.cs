using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.Application
{
	/// <summary>
	///		Manager de soluciones
	/// </summary>
	public class SolutionManager
	{
		// Constantes privadas
		private const string DefaultWorkspace = "Studio.Configuration";

		public SolutionManager(LogManager logger, string pathConfiguration)
		{
			Logger = logger;
			PathConfiguration = pathConfiguration;
			ConnectionManager = new Connections.ConnectionManager(this);
		}

		/// <summary>
		///		Carga los datos de configuración
		/// </summary>
		public SolutionModel LoadConfiguration(string workspace)
		{
			WorkSpace = workspace;
			return new Repository.SolutionRepository().Load(GetConfigurationFileName());
		}

		/// <summary>
		///		Borra el archivo de configuración de un espacio de trabajo
		/// </summary>
		public void DeleteConfiguration(string workspace)
		{
			// Borra el archivo
			WorkSpace = workspace;
			LibHelper.Files.HelperFiles.KillFile(GetConfigurationFileName());
			// Carga el espacio de trabajo predeterminado
			LoadConfiguration(DefaultWorkspace);
		}

		/// <summary>
		///		Graba los datos de una solución
		/// </summary>
		public void SaveSolution(SolutionModel solution)
		{
			new Repository.SolutionRepository().Save(solution, GetConfigurationFileName());
		}

		/// <summary>
		///		Obtiene el nombre del archivo de configuración
		/// </summary>
		private string GetConfigurationFileName()
		{
			// Obtiene el directorio de configuración si no existía
			if (string.IsNullOrWhiteSpace(PathConfiguration))
				PathConfiguration = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			// Obtiene el directorio del espacio de trabajo
			if (string.IsNullOrWhiteSpace(WorkSpace))
				WorkSpace = DefaultWorkspace;
			// Devuelve el nombre del archivo de configuración
			return System.IO.Path.Combine(PathConfiguration, WorkSpace + ".xml");
		}

		/// <summary>
		///		Carga el esquema de una conexión
		/// </summary>
		public async Task LoadSchemaAsync(ConnectionModel connection, CancellationToken cancellationToken)
		{
			await ConnectionManager.LoadSchemaAsync(connection, cancellationToken);
		}

		/// <summary>
		///		Ejecuta una consulta sobre una conexión
		/// </summary>
		public async Task ExecuteQueryAsync(ConnectionModel connection, string query, 
											Connections.Models.ArgumentListModel arguments, 
											TimeSpan timeout, CancellationToken cancellationToken)
		{
			await ConnectionManager.ExecuteQueryAsync(connection, query, arguments, timeout, cancellationToken);
		}

		/// <summary>
		///		Obtiene un <see cref="System.Data.DataTable"/> paginada con una consulta sobre una conexión
		/// </summary>
		public async Task<System.Data.DataTable> GetDatatableQueryAsync(ConnectionModel connection, string query, 
																		Connections.Models.ArgumentListModel arguments, 
																		int actualPage, int pageSize, TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await ConnectionManager.GetDatatableQueryAsync(connection, query, arguments, actualPage, pageSize, timeout, cancellationToken);
		}

		/// <summary>
		///		Obtiene el datareader de una consulta
		/// </summary>
		public async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(ConnectionModel connection, string query, Connections.Models.ArgumentListModel arguments, 
																				TimeSpan timeout, CancellationToken cancellationToken)
		{
			return await ConnectionManager.ExecuteReaderAsync(connection, query, arguments, timeout, cancellationToken);
		}

		/// <summary>
		///		Exporta un directorio de archivos al formato de notebooks de Databricks
		/// </summary>
		public void ExportToDataBricks(Models.Deployments.DeploymentModel deployment)
		{
			new Controllers.Databricks.DatabrickExporter(this).Export(deployment);
		}

		/// <summary>
		///		Manager de log
		/// </summary>
		public LogManager Logger { get; }

		/// <summary>
		///		Directorio de configuración
		/// </summary>
		public string PathConfiguration { get; private set; }

		/// <summary>
		///		Nombre del archivo del espacio de trabajo
		/// </summary>
		public string WorkSpace { get; private set; }

		/// <summary>
		///		Manager de conexiones
		/// </summary>
		internal Connections.ConnectionManager ConnectionManager { get; }
	}
}