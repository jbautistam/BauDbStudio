using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Cloud
{
	/// <summary>
	///		Procesador de trabajos de base de datos
	/// </summary>
	public class JobCloudManager : Core.Interfaces.IJobProcesor
	{
		public JobCloudManager(LibLogger.Core.LogManager logger)
		{
			Logger = logger;
		}

		/// <summary>
		///		Procesa un trabajo
		/// </summary>
		public async Task<bool> ProcessAsync(List<JobContextModel> contexts, JobStepModel step, CancellationToken cancellationToken)
		{
			NormalizedDictionary<Models.CloudConnection> connections = GetConnections(contexts, step);
			NormalizedDictionary<object> parameters = GetParameters(contexts, step);
			bool processed = false;

				// Procesa el paso
				try
				{
					processed = await ProcessStepAsync(step, connections, parameters, cancellationToken);
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
		private async Task<bool> ProcessStepAsync(JobStepModel step, NormalizedDictionary<Models.CloudConnection> connections, 
												  NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			bool processed = false;

				// Procesa el paso
				using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Debug, $"Start execute step {step.Name}"))
				{
					if (string.IsNullOrEmpty(step.ScriptFileName) || !System.IO.File.Exists(step.ScriptFileName))
						block.Error($"Cant find the file {step.ScriptFileName}");
					else
					{
						List<Models.Sentences.BaseSentence> program = new Repository.JobCloudRepository().Load(step.ScriptFileName);

							// Ejecuta el paso
							processed = await new Manager.ScriptInterpreter(this, step, connections).ProcessAsync(block, program, parameters, cancellationToken);
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
		///		Obtiene las conexiones del contexto y del trabjo
		/// </summary>
		private NormalizedDictionary<Models.CloudConnection> GetConnections(List<JobContextModel> contexts, JobStepModel job)
		{
			NormalizedDictionary<Models.CloudConnection> connections = new NormalizedDictionary<Models.CloudConnection>();

				// Obtiene los proveedores de los contextos globales
				foreach (JobContextModel context in contexts)
					if (!string.IsNullOrWhiteSpace(context.ProcessorKey) && 
							context.ProcessorKey.Equals(Key, StringComparison.CurrentCultureIgnoreCase))
						connections.AddRange(GetConnections(context));
				// Obtiene los proveedores del contexto del trabajo
				connections.AddRange(GetConnections(job));
				// Devuelve las conexiones
				return connections;
		}

		/// <summary>
		///		Obtiene las conexiones asociadas a un trabajo: primero las de los pasos padre porque las de los hijos son más restrictivas
		/// </summary>
		private NormalizedDictionary<Models.CloudConnection> GetConnections(JobStepModel job)
		{
			NormalizedDictionary<Models.CloudConnection> connections = new NormalizedDictionary<Models.CloudConnection>();

				// Añade las conexiones de los pasos padre
				if (job.Parent != null)
					connections.AddRange(GetConnections(job.Parent));
				// Añade las conexiones del paso
				connections.AddRange(GetConnections(job.Context));
				// Devuelve la colección de conexiones
				return connections;
		}

		/// <summary>
		///		Obtiene las conexiones de un contexto
		/// </summary>
		private NormalizedDictionary<Models.CloudConnection> GetConnections(JobContextModel context)
		{
			NormalizedDictionary<Models.CloudConnection> connections = new NormalizedDictionary<Models.CloudConnection>();

				// Obtiene las conexiones del árbol
				foreach (LibDataStructures.Trees.TreeNodeModel node in context.Tree.Nodes)
					if (node.Id.EqualsIgnoreCase("Cloud"))
					{
						Models.CloudConnection configuration = GetConnection(node);

							connections.Add(configuration.Key, configuration);
					}
				// Devuelve la lista de conexiones
				return connections;
		}

		/// <summary>
		///		Obtiene la conexión asociada a un nodo
		/// </summary>
		private Models.CloudConnection GetConnection(LibDataStructures.Trees.TreeNodeModel node)
		{
			Models.CloudConnection connection = new Models.CloudConnection();

				// Asigna las propiedades básicas a la conexión
				connection.Key = GetAttributeValue(node, "Key");
				connection.Type = GetAttributeValue(node, "Type").GetEnum(Models.CloudConnection.CloudType.Unknown);
				// Asigna el resto de propiedades
				foreach (LibDataStructures.Trees.TreeNodeModel child in node.Nodes)
					if (!string.IsNullOrWhiteSpace(child.Id))
					{
						if (child.Id.EqualsIgnoreCase("ConnectionString"))
							connection.StorageConnectionString = child.Value?.ToString().TrimIgnoreNull();
						else
							connection.Parameters.Add(child.Id, child.Value?.ToString());
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
					return value.ToString().Trim();
		}

		/// <summary>
		///		Clave del proveedor
		/// </summary>
		public string Key { get; } = "CloudManager";

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