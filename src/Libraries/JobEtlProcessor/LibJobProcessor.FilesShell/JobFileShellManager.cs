using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.FilesShell
{
	/// <summary>
	///		Procesador de trabajos de sistema operativo: copia de archivos, borrado, ejecución de procesos...
	/// </summary>
	public class JobFileShellManager : Core.Interfaces.BaseJobProcessor
	{
		public JobFileShellManager(LibLogger.Core.LogManager logger) : base("ShellManager")
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
						List<Models.Sentences.BaseSentence> program = new Repository.JobFilesShellRepository().Load(step.ScriptFileName);

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