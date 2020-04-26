using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.DbAggregator;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Manager;
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
	public class JobDatabaseManager : Core.Interfaces.IJobProcesor
	{
		public JobDatabaseManager(LibLogger.Core.LogManager logger)
		{
			Logger = logger;
		}

		/// <summary>
		///		Procesa un trabajo
		/// </summary>
		public async Task<bool> ProcessAsync(List<JobContextModel> contexts, JobStepModel step, CancellationToken cancellationToken)
		{
			NormalizedDictionary<object> parameters = GetParameters(contexts, step);
			DbAggregatorManager dataProviderManager = GetProviderManager(contexts, step);
			bool processed = false;

				// Procesa el paso
				try
				{
					processed = await ProcessStepAsync(dataProviderManager, step, parameters, cancellationToken);
				}
				catch (Exception exception)
				{
					Logger.Default.LogItems.Error($"Error when execute step '{step.Name}'", exception);
				}
				// Devuelve el valor que indica si se ha procesado correctamente
				return processed;
		}

		/// <summary>
		///		Ejecuta un paso
		/// </summary>
		private async Task<bool> ProcessStepAsync(DbAggregatorManager dataProviderManager, JobStepModel step, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			bool processed = false;

				// Procesa el paso
				using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Debug, $"Start execute step {step.Name}"))
				{
					if (string.IsNullOrEmpty(step.ScriptFileName) || !System.IO.File.Exists(step.ScriptFileName))
						block.Error($"Cant find the file {step.ScriptFileName}");
					else
					{
						DbScriptManager scriptManager = new DbScriptManager(dataProviderManager, parameters, GetProjectPaths(step), Logger);

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
		///		Obtiene los parámetros del paso
		/// </summary>
		private NormalizedDictionary<object> GetParameters(List<JobContextModel> contexts, JobStepModel step)
		{
			NormalizedDictionary<object> parameters = new NormalizedDictionary<object>();

				// Añade los parámetros de los contextos
				foreach (JobContextModel context in contexts)
					parameters.AddRange(GetParameters(context.Parameters));
				// Añade al diccionario los parámetros del script (desde el padre hacia abajo: los más internos son los que prevalecen
				// porque son más específicos que los externos)
				parameters.AddRange(GetParameters(step));
				// Devuelve el diccionario de parámetros
				return parameters;
		}

		/// <summary>
		///		Obtiene los parámetros de un paso: primero obtiene los parámetros de sus ancestros porque se supone que cuánto más
		///	abajo, más específico
		/// </summary>
		private NormalizedDictionary<object> GetParameters(JobStepModel step)
		{
			NormalizedDictionary<object> parameters = new NormalizedDictionary<object>();

				// Obtiene la colección de parámetros del padre
				if (step.Parent != null)
					parameters.AddRange(GetParameters(step.Parent));
				// y le añade los parámetros de este paso
				parameters.AddRange(GetParameters(step.Context.Parameters));
				// Devuelve la colección de parámetros
				return parameters;
		}

		/// <summary>
		///		Obtiene los parámetros de una lista de parámetros de trabajo
		/// </summary>
		private NormalizedDictionary<object> GetParameters(NormalizedDictionary<JobParameterModel> parameters)
		{
			NormalizedDictionary<object> result = new NormalizedDictionary<object>();

				// Convierte los parámetros
				foreach ((string key, JobParameterModel parameter) in parameters.Enumerate())
					result.Add(key, parameter.Value);
				// Devuelve el diccionario de parámetros
				return result;
		}

		/// <summary>
		///		Obtiene los directorios de proyecto
		/// </summary>
		private NormalizedDictionary<string> GetProjectPaths(JobStepModel step)
		{
			NormalizedDictionary<string> paths = new NormalizedDictionary<string>();

				// Añade los directorios de proyecto
				foreach (JobContextModel context in step.Project.Contexts)
					paths.AddRange(context.Paths);
				// Sustituye los directorios "Fijos"
				paths.Add("ProjectWorkPath", step.Project.ProjectWorkPath);
				paths.Add("ContextWorkPath", step.Project.ContextWorkPath);
				// Devuelve el diccionario de directorios
				return paths;
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
				connection.Type = GetAttributeValue(node, "Type");
				connection.GlobalId = GetAttributeValue(node, "Key");
				connection.Name = GetAttributeValue(node, "Name");
				connection.Description = GetAttributeValue(node, "Description");
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
		///		Obtiene el valor de un atributo en forma de cadena
		/// </summary>
		private string GetAttributeValue(LibDataStructures.Trees.TreeNodeModel node, string key)
		{
			object value = node.Attributes[key].Value;

				if (value == null)
					return string.Empty;
				else
					return value.ToString();
		}

		/// <summary>
		///		Clave del proveedor
		/// </summary>
		public string Key { get; } = "DbManager";

		/// <summary>
		///		Logger
		/// </summary>
		public LibLogger.Core.LogManager Logger { get; }

		/// <summary>
		///		Errores
		/// </summary>
		public List<string> Errors { get; } = new List<string>();
	}
}
