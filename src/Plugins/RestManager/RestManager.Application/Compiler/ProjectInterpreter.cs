using Bau.Libraries.RestManager.Application.Models;
using Bau.Libraries.LibRestClient.Messages;

namespace Bau.Libraries.RestManager.Application.Compiler;

/// <summary>
///		Intérprete de <see cref="RestProjectModel"/>
/// </summary>
internal class ProjectInterpreter
{
	internal ProjectInterpreter(RestProjectManager manager, RestProjectModel project, LibRestClient.RestClientManager restManager)
	{
		Manager = manager;
		Project = project;
		RestManager = restManager;
	}

	/// <summary>
	///		Ejecuta un proyecto
	/// </summary>
	internal async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		await ExecuteAsync(Project.Steps, cancellationToken);
	}

	/// <summary>
	///		Ejecuta una serie de pasos
	/// </summary>
	internal async Task ExecuteAsync(List<BaseStepModel> steps, CancellationToken cancellationToken)
	{
		// Lanza un evento de inicio
		Manager.RaiseInfo("Start rest execution");
		// Inicializa el contexto
		Contexts.Clear();
		foreach (ParameterModel parameter in Project.Parameters)
			Contexts.Add(parameter.Key, parameter.Value);
		// Ejecuta las sentencias
		await ExecuteSentencesAsync(steps, cancellationToken);
		// Lanza un evento de fin
		Manager.RaiseLog(EventArguments.LogEventArgs.Status.Success, "End rest execution");
	}

	/// <summary>
	///		Ejecuta una serie de sentencias
	/// </summary>
	private async Task ExecuteSentencesAsync(List<BaseStepModel> steps, CancellationToken cancellationToken)
	{
		foreach (BaseStepModel step in steps)
			if (step.Enabled && !HasError && !cancellationToken.IsCancellationRequested)
				switch (step)
				{
					case RestStepModel sentence:
							await ExecuteRestStepAsync(sentence, cancellationToken);
						break;
					default:
							SetError($"Step type unknown: {step.GetType().ToString()}");
						break;
				}
	}

	/// <summary>
	///		Ejecuta un paso Rest
	/// </summary>
	private async Task ExecuteRestStepAsync(RestStepModel step, CancellationToken cancellationToken)
	{
		RequestMessage request = new Conversors.MessageConversor().CreateRequest(step, Contexts);
		ConnectionModel? connection = step.Project.Connections.Search(step.ConnectionId);

			// Ejecuta la solicitud
			if (connection is null)
				SetError($"Can't find the connection for request {step.Name} - {step.Method}");
			else
			{
				// Añade las cabeceras de la conexión a la solicitud
				foreach (ParameterModel header in connection.Headers)
					if (!request.Headers.ContainsKey(header.Key))
						request.Headers.Add(header.Key, header.Value);
				// Log
				Manager.RaiseInfo("Start request");
				Manager.RaiseInfo(request.GetDebugString());
				// Envía la solicitud
				try
				{
					ResponseMessage response = await RestManager.SendAsync(connection.Url, request, cancellationToken);

						// Escribe la cadena de depuración
						Manager.RaiseInfo("End request");
						Manager.RaiseInfo(response.GetDebugString());
				}
				catch (Exception exception)
				{
					SetError($"Error when execute request. {exception.Message}");
				}
			}
	}

	/// <summary>
	///		Asigna un error
	/// </summary>
	private void SetError(string message)
	{
		// Indica que ha habido algún error
		HasError = true;
		// Lanza el mensaje de error
		Manager.RaiseError(message);
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	public RestProjectManager Manager { get; }

	/// <summary>
	///		Proyecto en ejecución
	/// </summary>
	public RestProjectModel Project { get; }

	/// <summary>
	///		Contextos
	/// </summary>
	private ContextStackModel Contexts { get; } = new();

	/// <summary>
	///		Indica si hay algún error
	/// </summary>
	public bool HasError { get; private set; }

	/// <summary>
	///		Manager de llamadas REST
	/// </summary>
	private LibRestClient.RestClientManager RestManager { get; }
}
