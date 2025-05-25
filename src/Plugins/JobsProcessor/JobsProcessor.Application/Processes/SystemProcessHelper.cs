using System.Diagnostics;

namespace Bau.Libraries.JobsProcessor.Application.Processes;

/// <summary>
///		Clase de ayuda para el tratamiento de procesos (ejecutables)
/// </summary>
internal class SystemProcessHelper
{
	internal SystemProcessHelper(JobsProcessorManager manager, Models.ContextModel context, Models.CommandModel command, bool showWindow = true)
	{
		// Asigna el objeto
		Manager = manager;
		Context = context;
		Command = command;
		ShowWindow = showWindow;
		// Prepara el proceso
		ProcessInitialize();
	}

	/// <summary>
	///		Prepara el proceso
	/// </summary>
	private void ProcessInitialize()
	{
		Dictionary<string, string> environmentVariables = Command.GetEnvironmentVariables(Context);

			// Genera el proceso
			Process = new();
			// Inicializa las propiedades del proceso
			Process.StartInfo.UseShellExecute = false; // ... para poder añadir variables de entorno en su caso y poder recoger los stream de salida
			Process.StartInfo.RedirectStandardOutput = true;
			Process.StartInfo.RedirectStandardError = true;
			Process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Command.FileName);
			Process.StartInfo.FileName = Command.FileName;
			Process.StartInfo.Arguments = Command.GetArguments(Context) ?? string.Empty;
			Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			Process.StartInfo.CreateNoWindow = !ShowWindow;
			// Permite que se lancen eventos
			Process.EnableRaisingEvents = true;
			// Asigna las variables de entorno
			if (environmentVariables?.Count > 0)
				AssignEnvironmentVariables(Process.StartInfo, environmentVariables);
	}

	/// <summary>
	///		Ejecuta el proceso
	/// </summary>
	internal async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// Inicializa los valores de salida
		ExitCode = -1;
		// Ejecuta el proceso
		if (Process is not null)
			using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
			{
				using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
				{
					try
					{
						// Ajusta los eventos 
						Process.OutputDataReceived += (sender, e) =>
															{
																if (e.Data == null)
																{
																	if (!outputWaitHandle.SafeWaitHandle.IsClosed && !outputWaitHandle.SafeWaitHandle.IsInvalid)
																		outputWaitHandle.Set();
																}
																else
																	AddOutputLog(e.Data, EventArguments.JobProcessEventArgs.StatusType.Information);
															};
						Process.ErrorDataReceived += (sender, e) =>
															{
																if (e.Data == null)
																{
																	if (!errorWaitHandle.SafeWaitHandle.IsClosed && !errorWaitHandle.SafeWaitHandle.IsInvalid)
																		errorWaitHandle.Set();
																}
																else
																	AddOutputLog(e.Data, EventArguments.JobProcessEventArgs.StatusType.Error);
															};
						// Arranca el proceso
						Process.Start();
						// Comienza la lectura de los datos de salida y error
						Process.BeginOutputReadLine();
						Process.BeginErrorReadLine();
						// Ejecuta el proceso hasta que termine
						await Process.WaitForExitAsync(cancellationToken);
						// Obtiene los datos de salida
						if (!cancellationToken.IsCancellationRequested)
						{
							AddOutputLog($"ExitCode: {Process.ExitCode.ToString()}", EventArguments.JobProcessEventArgs.StatusType.Information);
							ExitCode = Process.ExitCode;
						}
						else
						{
							AddOutputLog($"Cancelled", EventArguments.JobProcessEventArgs.StatusType.Information);
							ExitCode = -1;
						}
					}
					finally
					{
						outputWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
						errorWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
					}
				}
			}
	}

	/// <summary>
	///		Asigna las variables de entorno
	/// </summary>
	private void AssignEnvironmentVariables(ProcessStartInfo startInfo, Dictionary<string, string> environmentVariables)
	{
		foreach (KeyValuePair<string, string> environmentVariable in environmentVariables)
			startInfo.EnvironmentVariables.Add(environmentVariable.Key, environmentVariable.Value);
	}

	/// <summary>
	///		Comprueba si se está procesando una aplicación
	/// </summary>
	internal bool IsExecuting()
	{
		if (Process is not null)
		{
			Process.Refresh();
			return !Process.HasExited;
		}
		else
			return false;
	}

	/// <summary>
	///		Elimina el proceso de memoria
	/// </summary>
	internal bool Kill(bool entireProcessTree, out string error)
	{ 
		// Inicializa los argumentos de salida
		error = string.Empty;
		// Elimina el proceso de memoria
		if (Process is not null)
			try
			{
				Process.Kill(entireProcessTree);
			}
			catch (Exception exception)
			{
				error = exception.Message;
			}
		// Devuelve el valor que indica si se ha eliminado el proceso
		return string.IsNullOrWhiteSpace(error);
	}

	/// <summary>
	///		Espera que el proceso termine
	/// </summary>
	internal async Task WaitForExitAsync(CancellationToken cancellationToken)
	{
		if (Process is not null)
			await Process.WaitForExitAsync(cancellationToken);
	}

	/// <summary>
	///		Obtiene la salida de error estándar
	/// </summary>
	internal string GetErrorOutput()
	{
		if (Process is not null)
			return Process.StandardError.ReadToEnd();
		else
			return string.Empty;
	}

	/// <summary>
	///		Obtiene la salida estándar
	/// </summary>
	internal string GetStandardOutput()
	{
		if (Process is not null)
			return Process.StandardOutput.ReadToEnd();
		else
			return string.Empty;
	}

	/// <summary>
	///		Añade al log las salidas de la consola
	/// </summary>
	private void AddOutputLog(string message, EventArguments.JobProcessEventArgs.StatusType type)
	{
		if (!string.IsNullOrWhiteSpace(message))
			foreach (string part in message.Split(Environment.NewLine))
				Manager.RaiseEvent(Context, Command, type, part);
	}

	/// <summary>
	///		Procesador
	/// </summary>
	internal JobsProcessorManager Manager { get; }

	/// <summary>
	///		Contexto
	/// </summary>
	internal Models.ContextModel Context { get; }
	
	/// <summary>
	///		Comando
	/// </summary>
	internal Models.CommandModel Command { get; }

	/// <summary>
	///		Indica si se debe mostrar la ventana
	/// </summary>
	internal bool ShowWindow { get; }

	/// <summary>
	///		Proceso en ejecución
	/// </summary>
	internal Process Process { get; private set; } = default!;

	/// <summary>
	///		Código de salida del proceso
	/// </summary>
	internal int ExitCode { get; private set; }
}