using Bau.Libraries.JobsProcessor.Application.EventArguments;
using Bau.Libraries.JobsProcessor.Application.Models;

namespace Bau.Libraries.JobsProcessor.Application;

/// <summary>
///		Manager del procesador de trabajos
/// </summary>
public class JobsProcessorManager
{
	// Eventos públicos
	public event EventHandler<JobProcessEventArgs>? JobProcessing;

	/// <summary>
	///		Carga los datos de ejecución de un comando
	/// </summary>
	public ProjectModel Load(string fileName) => new Repository.ConsoleRepository().Load(fileName);

	/// <summary>
	///		Valida un proyecto
	/// </summary>
	public List<string> Validate(ProjectModel project)
	{
		List<string> errors = new();

			// Valida el proyecto
			if (project == null)
				errors.Add("Error when parse file");
			else if (project.Commands.Count == 0)
				errors.Add("There is no any command at file");
			else
				foreach (CommandModel command in project.Commands)
					if (string.IsNullOrWhiteSpace(command.FileName))
						errors.Add($"The executable file name for command {project.Commands.IndexOf(command).ToString()} is empty");
					else if (!File.Exists(command.FileName))
						errors.Add($"Can't find the executable file: {command.FileName}");
			// Devuelve la lista de errores
			return errors;
	}

	/// <summary>
	///		Ejecuta un proyecto
	/// </summary>
	public async Task ExecuteAsync(ProjectModel project, CancellationToken cancellationToken)
	{
		// Log
		AddLog(JobProcessEventArgs.StatusType.StartProject, $"Start project execution");
		// Ejecuta los comandos
		try
		{
			if (project.Contexts.Count == 0)
				await ExecuteCommandsAsync(project.Commands, new ContextModel(), cancellationToken);
			else
				foreach (ContextModel context in project.Contexts)
				{
					int contextIndex = project.Contexts.IndexOf(context) + 1;

						// Log
						AddLog(context, null, JobProcessEventArgs.StatusType.StartContext, $"Start execution context {contextIndex}", contextIndex, project.Contexts.Count);
						// Ejecuta el comando
						await ExecuteCommandsAsync(project.Commands, context, cancellationToken);
						// Log
						AddLog(context, null, JobProcessEventArgs.StatusType.EndContext, $"End execution context {contextIndex}", contextIndex, project.Contexts.Count);
				}
		}
		catch (Exception exception)
		{
			AddLog(JobProcessEventArgs.StatusType.Error, $"Execution error: {exception.Message}");
		}
		// Log
		if (cancellationToken.IsCancellationRequested)
			AddLog(null, null, JobProcessEventArgs.StatusType.Error, $"Cancel project execution");
		else
			AddLog(null, null, JobProcessEventArgs.StatusType.EndProject, $"End project execution");
	}

	/// <summary>
	///		Ejecuta una serie de comandos teniendo en cuenta el contexto
	/// </summary>
	private async Task ExecuteCommandsAsync(List<CommandModel> commands, ContextModel context, CancellationToken cancellationToken)
	{
		foreach (CommandModel command in commands)
			if (!cancellationToken.IsCancellationRequested)
			{
				// Log
				AddLog(context, command, JobProcessEventArgs.StatusType.StartCommand, 
					   $"Start execution {Path.GetFileName(command.FileName)}", commands.IndexOf(command) + 1, commands.Count);
				// Ejecuta el proceso
				if (!await ExecuteProcessAsync(command, context, cancellationToken))
				{
					// Log
					AddLog(JobProcessEventArgs.StatusType.Error, $"Error when execute command: '{command.FileName}'");
					// Detiene el proceso si es necesario
					if (cancellationToken.IsCancellationRequested)
						throw new Exception($"Stop the process (command '{command.FileName}'. Cancel request");
					else if (command.StopWhenError)
						throw new Exception($"Stop the process (command '{command.FileName}'. StopWhenError = true");
				}
				// Log
				AddLog(context, command, JobProcessEventArgs.StatusType.EndCommand, 
					   $"End execution {Path.GetFileName(command.FileName)}", commands.IndexOf(command) + 1, commands.Count);
			}
			else
				AddLog(context, command, JobProcessEventArgs.StatusType.Error, "Canceled command");
	}

	/// <summary>
	///		Ejecuta el proceso asociado a un comando
	/// </summary>
	private async Task<bool> ExecuteProcessAsync(CommandModel command, ContextModel context, CancellationToken cancellationToken)
	{
		Processes.SystemProcessHelper processor = new(this, context, command, false);
		bool processed;

			// Ejecuta la aplicación
			try
			{
				// Ejecuta el comando
				await processor.ExecuteAsync(cancellationToken);
				// Devuelve el valor que indica si ha habido algún error
				processed = processor.ExitCode >= 0;
			}
			catch (Exception exception)
			{
				AddLog(JobProcessEventArgs.StatusType.Error, $"Error when execute {Path.GetFileName(command.FileName)}. {exception.Message}");
				processed = false;
			}
			// Elimina el proceso
			if (cancellationToken.IsCancellationRequested)
				try
				{
					processor.Kill(true, out string error);
					if (!string.IsNullOrWhiteSpace(error))
						AddLog(JobProcessEventArgs.StatusType.Error, $"Error when kill process {error}");
				}
				catch (Exception exception)
				{
					AddLog(JobProcessEventArgs.StatusType.Error, $"Error when kill process {Path.GetFileName(command.FileName)}. {exception.Message}");
				}
			// Devuelve el valor que indica si se ha ejecutado correctamente
			return processed;
	}

	/// <summary>
	///		Lanza un evento de proceso
	/// </summary>
	internal void RaiseEvent(ContextModel context, CommandModel command, JobProcessEventArgs.StatusType status, string message)
	{
		AddLog(context, command, status, message);
	}

	/// <summary>
	///		Envía un evento de log de ejecución
	/// </summary>
	private void AddLog(JobProcessEventArgs.StatusType status, string? message = null, int? actual = null, int? total = null)
	{
		AddLog(null, null, status, message, actual, total);
	}

	/// <summary>
	///		Envía un evento de log de ejecución
	/// </summary>
	private void AddLog(ContextModel? context, CommandModel? command, JobProcessEventArgs.StatusType status, string? message = null,
						int? actual = null, int? total = null)
	{
		JobProcessing?.Invoke(this, new JobProcessEventArgs(context, command, status, message, actual, total));
	}
}
