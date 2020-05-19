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
	public class JobCloudManager : Core.Interfaces.BaseJobProcessor
	{
		public JobCloudManager(LibLogger.Core.LogManager logger) : base("CloudManager")
		{
			Logger = logger;
		}

		/// <summary>
		///		Ejecuta un paso
		/// </summary>
		protected override async Task<bool> ProcessStepAsync(List<JobContextModel> contexts, JobStepModel step, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			bool processed = false;
			NormalizedDictionary<Models.CloudConnection> connections = GetConnections(contexts, step);

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
		///		Obtiene las conexiones del contexto y del trabajo
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
				connection.Key = node.Attributes["Key"].GetValueString();
				connection.Type = node.Attributes["Type"].GetValueString().GetEnum(Models.CloudConnection.CloudType.Unknown);
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
		///		Logger
		/// </summary>
		public LibLogger.Core.LogManager Logger { get; }
	}
}