using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Powershell
{
	/// <summary>
	///		Procesador para ejecución de scripts de Powershell
	/// </summary>
	public class JobPowershellManager : Core.Interfaces.IJobProcesor
	{
		public JobPowershellManager(LibLogger.Core.LogManager logger)
		{
			Logger = logger;
		}

		/// <summary>
		///		Procesa un trabajo
		/// </summary>
		public async Task<bool> ProcessAsync(List<JobContextModel> contexts, JobStepModel step, CancellationToken cancellationToken)
		{
			NormalizedDictionary<object> parameters = GetParameters(contexts, step);
			bool processed = false;

				// Procesa el paso
				try
				{
					processed = await ProcessStepAsync(step, parameters, cancellationToken);
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
		private async Task<bool> ProcessStepAsync(JobStepModel step, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			bool processed = false;

				// Procesa el paso
				using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Debug, $"Start execute step {step.Name}"))
				{
					if (string.IsNullOrEmpty(step.ScriptFileName) || !System.IO.File.Exists(step.ScriptFileName))
						block.Error($"Cant find the file {step.ScriptFileName}");
					else
					{
						List<Models.BaseSentence> program = new Repository.JobPowershellRepository().Load(step.ScriptFileName);

							// Ejecuta el paso
							processed = await new Manager.ScriptInterpreter(this, step).ProcessAsync(block, program, parameters, cancellationToken);
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
		///		Clave del proveedor
		/// </summary>
		public string Key { get; } = "PowershellManager";

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