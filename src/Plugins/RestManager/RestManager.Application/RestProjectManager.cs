using Bau.Libraries.LibRestClient;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application;

/// <summary>
///		Manager de procesamiento Rest
/// </summary>
public class RestProjectManager
{
	// Evento de log
	public event EventHandler<EventArguments.LogEventArgs>? Log;

	/// <summary>
	///		Carga el proyecto
	/// </summary>
	public RestProjectModel Load(string fileName) => new Repositories.ProjectRepository().Load(fileName);

	/// <summary>
	///		Graba el proyecto
	/// </summary>
	public void Save(RestProjectModel project, string fileName)
	{
		new Repositories.ProjectRepository().Save(project, fileName);
	}

	/// <summary>
	///		Ejecuta un proyecto
	/// </summary>
	public async Task ExecuteAsync(RestProjectModel project, CancellationToken cancellationToken)
	{
		await new Compiler.ProjectInterpreter(this, project, PrepareRestClient(project)).ExecuteAsync(cancellationToken);
	}

	/// <summary>
	///		Ejecuta un paso de un proyecto
	/// </summary>
	public async Task ExecuteAsync(RestProjectModel project, RestStepModel step, CancellationToken cancellationToken)
	{
		await new Compiler.ProjectInterpreter(this, project, PrepareRestClient(project)).ExecuteAsync([ step ], cancellationToken);
	}

	/// <summary>
	///		Prepara el cliente de llamadas REST
	/// </summary>
	private RestClientManager PrepareRestClient(RestProjectModel project)
	{
		RestClientManager manager = new();

			// Añade las conexiones
			foreach (ConnectionModel connection in project.Connections)
				manager.AddConnection(connection.Url, connection.Timeout);
			// Devuelve el manager
			return manager;
	}

	/// <summary>
	///		Lanza un evento informativo
	/// </summary>
	internal void RaiseInfo(string message) => RaiseLog(EventArguments.LogEventArgs.Status.Info, message);

	/// <summary>
	///		Lanza un evento de error
	/// </summary>
	internal void RaiseError(string message) => RaiseLog(EventArguments.LogEventArgs.Status.Error, message);

	/// <summary>
	///		Lanza un evento de log
	/// </summary>
	internal void RaiseLog(EventArguments.LogEventArgs.Status status, string message)
	{
		Log?.Invoke(this, new EventArguments.LogEventArgs(status, message));
	}
}