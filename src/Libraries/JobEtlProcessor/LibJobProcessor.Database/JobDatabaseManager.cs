using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.DbAggregator;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Database.Manager;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibJobProcessor.Database.Models;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Database
{
	/// <summary>
	///		Procesador de trabajos de base de datos
	/// </summary>
	public class JobDatabaseManager : Core.Interfaces.BaseJobProcessor
	{
		public JobDatabaseManager(LibLogger.Core.LogManager logger) : base("DbManager")
		{
			Logger = logger;
		}

		/// <summary>
		///		Ejecuta un paso
		/// </summary>
		protected override async Task<bool> ProcessStepAsync(List<JobContextModel> contexts, JobStepModel step, NormalizedDictionary<object> parameters, 
															 CancellationToken cancellationToken)
		{
			bool processed = false;
			DbAggregatorManager dataProviderManager = GetProviderManager(contexts, step);

				// Procesa el paso
				using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Debug, $"Start execute step {step.Name}"))
				{
					if (string.IsNullOrEmpty(step.ScriptFileName) || !System.IO.File.Exists(step.ScriptFileName))
						block.Error($"Cant find the file {step.ScriptFileName}");
					else
					{
						DbScriptManager scriptManager = new DbScriptManager(step, dataProviderManager, parameters, Logger);

							// Ejecuta el paso
							if (System.IO.Path.GetExtension(step.ScriptFileName).EqualsIgnoreCase(".sql"))
								processed = await scriptManager.ProcessBySqlScriptAsync(step.Target, step.ScriptFileName, cancellationToken);
							else
								processed = await scriptManager.ProcessByFileAsync(step.ScriptFileName, cancellationToken);
					}
				}
				// Devuelve el valor que indica si se ha procesado correctamente
				return processed;
		}

		/// <summary>
		///		Obtiene e inicializa el controlador de proveedores de datos
		/// </summary>
		private DbAggregatorManager GetProviderManager(List<JobContextModel> contexts, JobStepModel job)
		{
			DbAggregatorManager providersManager = new DbAggregatorManager();
			NormalizedDictionary<DataBaseConnectionModel> connections = GetConnections(contexts, job);

				// Asign las conexiones al proveedor
				foreach ((string key, DataBaseConnectionModel connection) in connections.Enumerate())
				{
					DbAggregator.Models.ConnectionModel dbConnection = new DbAggregator.Models.ConnectionModel(key, connection.Type.ToString());

						// Añade los parámetros
						dbConnection.Parameters.AddRange(connection.Parameters);
						// Añade la conexión
						providersManager.AddConnection(dbConnection);
				}
				// Devuelve el agregado
				return providersManager;
		}

		/// <summary>
		///		Obtiene las conexiones del contexto y del trabjo
		/// </summary>
		private NormalizedDictionary<DataBaseConnectionModel> GetConnections(List<JobContextModel> contexts, JobStepModel job)
		{
			NormalizedDictionary<DataBaseConnectionModel> connections = new NormalizedDictionary<DataBaseConnectionModel>();

				// Obtiene los proveedores de los contextos globales
				foreach (JobContextModel context in contexts)
					if (!string.IsNullOrWhiteSpace(context.ProcessorKey) && 
							context.ProcessorKey.Equals(Key, StringComparison.CurrentCultureIgnoreCase))
						connections.AddRange(GetConnections(job.Project, context));
				// Obtiene los proveedores del contexto del trabajo
				connections.AddRange(GetConnections(job));
				// Devuelve las conexiones
				return connections;
		}

		/// <summary>
		///		Obtiene las conexiones asociadas a un trabajo: primero las de los pasos padre porque las de los hijos son más restrictivas
		/// </summary>
		private NormalizedDictionary<DataBaseConnectionModel> GetConnections(JobStepModel job)
		{
			NormalizedDictionary<DataBaseConnectionModel> connections = new NormalizedDictionary<DataBaseConnectionModel>();

				// Añade las conexiones de los pasos padre
				if (job.Parent != null)
					connections.AddRange(GetConnections(job.Parent));
				// Añade las conexiones del paso
				connections.AddRange(GetConnections(job.Project, job.Context));
				// Devuelve la colección de conexiones
				return connections;
		}

		/// <summary>
		///		Obtiene las conexiones de un contexto
		/// </summary>
		private NormalizedDictionary<DataBaseConnectionModel> GetConnections(JobProjectModel project, JobContextModel context)
		{
			NormalizedDictionary<DataBaseConnectionModel> connections = new NormalizedDictionary<DataBaseConnectionModel>();

				// Obtiene las conexiones del árbol
				foreach (LibDataStructures.Trees.TreeNodeModel node in context.Tree.Nodes)
					if (node.Id.EqualsIgnoreCase("Connection"))
					{
						DataBaseConnectionModel connection = GetConnection(project, node);

							connections.Add(connection.GlobalId, connection);
					}
				// Devuelve la lista de conexiones
				return connections;
		}

		/// <summary>
		///		Obtiene la conexión asociada a un nodo
		/// </summary>
		private DataBaseConnectionModel GetConnection(JobProjectModel project, LibDataStructures.Trees.TreeNodeModel node)
		{
			DataBaseConnectionModel connection = new DataBaseConnectionModel();

				// Asigna las propiedades básicas a la conexión
				connection.Type = node.Attributes["Type"].GetValueString();
				connection.GlobalId = node.Attributes["Key"].GetValueString();
				connection.Name = node.Attributes["Name"].GetValueString();
				connection.Description = node.Attributes["Description"].GetValueString();
				// Asigna el resto de propiedades
				foreach (LibDataStructures.Trees.TreeNodeModel child in node.Nodes)
					if (!string.IsNullOrWhiteSpace(child.Id))
					{
						string value = child.Value?.ToString();

							// Si el parámetro identifica un nombre de archivo, le añade el directorio
							if (child.Id.EqualsIgnoreCase("FileName") && !System.IO.File.Exists(value))
								value = project.GetFullFileName(value);
							// Añade un parámetro a la conexión
							connection.Parameters.Add(child.Id, value);
					}
				// Devuelve la conexión
				return connection;
		}

		/// <summary>
		///		Logger
		/// </summary>
		public LibLogger.Core.LogManager Logger { get; }
	}
}
