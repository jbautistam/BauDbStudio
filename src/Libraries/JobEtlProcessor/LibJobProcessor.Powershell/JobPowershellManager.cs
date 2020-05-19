using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.Powershell
{
	/// <summary>
	///		Procesador para ejecución de scripts de Powershell
	/// </summary>
	public class JobPowershellManager : Core.Interfaces.BaseJobProcessor
	{
		public JobPowershellManager(LibLogger.Core.LogManager logger) : base("PowershellManager")
		{
			Logger = logger;
		}

		/// <summary>
		///		Ejecuta un paso
		/// </summary>
		protected override async Task<bool> ProcessStepAsync(List<JobContextModel> contexts, JobStepModel step, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
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
		///		Logger
		/// </summary>
		public LibLogger.Core.LogManager Logger { get; }
	}
}