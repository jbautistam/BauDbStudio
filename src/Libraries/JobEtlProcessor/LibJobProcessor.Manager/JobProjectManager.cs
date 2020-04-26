using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Manager
{
	/// <summary>
	///		Manager de proyectos de trabajos
	/// </summary>
    public class JobProjectManager
	{
		public JobProjectManager(LogManager logger)
		{
			Logger = logger;
		}

		/// <summary>
		///		Añade un procesador
		/// </summary>
		public void AddProcessor(IJobProcesor processor)
		{
			Processors.Add(processor.Key, processor);
		}

		/// <summary>
		///		Graba un archivo de proyecto
		/// </summary>
		public void Save(JobProjectModel project, string fileName)
		{
			new Repository.JobProjectRepository().Save(project, fileName);
		}

		/// <summary>
		///		Carga los datos de un proyecto
		/// </summary>
		public JobProjectModel Load(string fileName)
		{
			return new Repository.JobProjectRepository().Load(fileName);
		}

		/// <summary>
		///		Procesa un proyecto cargándolo de un archivo
		/// </summary>
		public async Task<bool> ProcessAsync(string fileName, CancellationToken cancellationToken)
		{
			return await ProcessAsync(fileName, string.Empty, cancellationToken, false);
		}

		/// <summary>
		///		Procesa un proyecto cargándolo de un archivo
		/// </summary>
		public async Task<bool> ProcessAsync(string fileProject, string fileContext, CancellationToken cancellationToken, bool removeProjectContexts = true)
		{
			JobProjectModel project = new Repository.JobProjectRepository().Load(fileProject);

				// Carga los contextos
				if (!string.IsNullOrWhiteSpace(fileContext))
				{
					// Elimina los contextos si es necesario
					if (removeProjectContexts)
						project.Contexts.Clear();
					// Agrega los contextos del archivo
					project.Contexts.AddRange(new Repository.JobContextRepository().Load(fileContext));
					// Cambia el directorio de trabajo del contexto para el proyecto
					project.ContextWorkPath = System.IO.Path.GetDirectoryName(fileContext);
				}
				// Procesa el proyecto
				return await ProcessAsync(project, cancellationToken);
		}

		/// <summary>
		///		Ejecuta un proyecto
		/// </summary>
		public async Task<bool> ProcessAsync(JobProjectModel project, CancellationToken cancellationToken)
		{
			// Ejecuta el proyecto
			using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Debug, $"Start execute project '{project.Name}'"))
			{
				// Indica que por ahora no ha habido errores
				Errors.Clear();
				// Ejecuta los trabajos
				if (!Validate(project, out string error))
				{
					// Muestra el error
					block.Error(error);
					// Añade el error a la colección de errores
					Errors.Add(error);
				}
				else
					foreach (JobStepModel job in project.Jobs)
						if (cancellationToken.IsCancellationRequested)
							block.Debug($"The job '{job.Name}' is not processed because user canceled the execution");
						else if (!job.Enabled)
							block.Debug($"The job '{job.Name}' is not processed because is disabled");
						else if (HasError)
							block.Debug($"The job '{job.Name}' is not processed because there is a previous error");
						else
							await ExecuteJobAsync(project, job, cancellationToken);
			}
			// Devuelve el valor que indica si se ha procesado correctamente
			return !HasError;
		}

		/// <summary>
		///		Valida los datos del proyecto
		/// </summary>
		private bool Validate(JobProjectModel project, out string error)
		{
			List<Core.Models.Errors.ErrorModel> errors = project.Validate();

				// Inicializa los argumentos de salida
				error = string.Empty;
				// Valida el proyecto
				if (errors.Count > 0)
				{
					// Inicializa el error
					error = "Error when validate the project";
					// Añade los errores
					foreach (Core.Models.Errors.ErrorModel innerError in errors)
						error += Environment.NewLine + $"\t{innerError.ToString()}";
				}
				// Devuelve el valor que indica si la validación es correcta
				return string.IsNullOrWhiteSpace(error);
		}

		/// <summary>
		///		Ejecuta un <see cref="JobModel"/>
		/// </summary>
		private async Task ExecuteJobAsync(JobProjectModel project, JobStepModel job, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Debug, $"Start execute '{job.Name}'"))
			{
				IJobProcesor processor = GetProcessor(job);

					// Comprueba los datos antes de continuar
					if (processor == null)
						block.Error($"Can't execute '{job.Name}' because can't find a procssor for '{job.ProcessorKey}'");
					else
						try
						{
							List<JobContextModel> contexts = GetContexts(project, job);

								// Cambia el nombre de archivo
								job.ScriptFileName = project.GetFullFileName(job.ScriptFileName, contexts);
								// Procesa el archivo
								if (string.IsNullOrWhiteSpace(job.ScriptFileName) || !System.IO.File.Exists(job.ScriptFileName))
								{
									block.Error($"Cant find the file '{job.ScriptFileName}'");
									Errors.Add($"Cant find the file '{job.ScriptFileName}'");
								}
								else
								{
									bool processed = await processor.ProcessAsync(contexts, job, cancellationToken);

										// Añade los errores de procesamiento
										if (!processed)
										{
											block.Error($"Error when execute '{job.Name}'{Environment.NewLine}{GetError(processor.Errors)}");
											Errors.Add($"Error when execute '{job.Name}'");
											Errors.AddRange(processor.Errors);
										}
								}
						}
						catch (Exception exception)
						{
							block.Error($"Error when execute '{job.Name}'", exception);
						}
			}
		}

		/// <summary>
		///		Obtiene los contextos que se asocian al trabajo
		/// </summary>
		private List<JobContextModel> GetContexts(JobProjectModel project, JobStepModel job)
		{
			List<JobContextModel> contexts = new List<JobContextModel>();

				// Obtiene los contextos asociados
				foreach (JobContextModel context in project.Contexts)
					if (context.ProcessorKey.Equals(JobProjectModel.GlobalContext, StringComparison.CurrentCultureIgnoreCase) ||
							context.ProcessorKey.Equals(job.ProcessorKey, StringComparison.CurrentCultureIgnoreCase))
						contexts.Add(context);
				// Devuelve la colección de contextos
				return contexts;
		}

		/// <summary>
		///		Obtiene una cadena de error
		/// </summary>
		private string GetError(List<string> errors)
		{
			string result = string.Empty;

				// Añade los errores
				foreach (string error in errors)
					result += $"\t{error}{Environment.NewLine}";
				// Devuelve la cadena de error
				return result;
		}

		/// <summary>
		///		Obtiene el procesador asociado a un trabajo
		/// </summary>
		private IJobProcesor GetProcessor(JobStepModel job)
		{
			return Processors[job.ProcessorKey];
		}

		/// <summary>
		///		Manejador de log
		/// </summary>
		internal LogManager Logger { get; }

		/// <summary>
		///		Colección de errores
		/// </summary>
		public List<string> Errors { get; } = new List<string>();

		/// <summary>
		///		Mensaje completo de error
		/// </summary>
		public string FullError
		{
			get
			{
				return GetError(Errors);
			}
		}

		/// <summary>
		///		Indica si ha habido algún error en el procesamiento del script
		/// </summary>
		public bool HasError 
		{ 
			get { return Errors.Count > 0; }
		}

		/// <summary>
		///		Procesadores
		/// </summary>
		private NormalizedDictionary<IJobProcesor> Processors { get; } = new NormalizedDictionary<IJobProcesor>();
	}
}
