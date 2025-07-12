using Bau.Libraries.RestManager.Application.Models;
using Bau.Libraries.LibRestClient.Messages;

namespace Bau.Libraries.RestManager.Application.Compiler;

/// <summary>
///		Intérprete de <see cref="RestProjectModel"/>
/// </summary>
internal class ProjectInterpreter
{
	internal ProjectInterpreter(RestProjectManager manager, RestProjectModel project)
	{
		Manager = manager;
		Project = project;
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
		// Inicializa el contexto
		Contexts.Clear();
		foreach (ParameterModel parameter in Project.Parameters)
			Contexts.Add(parameter.Key, parameter.Value);
		// Ejecuta las sentencias
		await ExecuteSentencesAsync(steps, cancellationToken);
	}

	/// <summary>
	///		Ejecuta una serie de sentencias
	/// </summary>
	private async Task ExecuteSentencesAsync(List<BaseStepModel> steps, CancellationToken cancellationToken)
	{
		foreach (BaseStepModel step in steps)
			if (step.Enabled && !cancellationToken.IsCancellationRequested)
				switch (step)
				{
					case RestStepModel sentence:
							await ExecuteRestStepAsync(sentence, cancellationToken);
						break;
					default:
						throw new NotImplementedException($"Step type unknown: {step.GetType().ToString()}");
				}
	}

	/// <summary>
	///		Ejecuta un paso Rest
	/// </summary>
	private async Task ExecuteRestStepAsync(RestStepModel step, CancellationToken cancellationToken)
	{
		RequestMessage request = new Conversors.MessageConversor().CreateRequest(step, Contexts);

			// Ejecuta la solicitud
			await ExecuteAsync(request, cancellationToken);
	}

	/// <summary>
	///		Ejecuta una solicitud
	/// </summary>
	private async Task<bool> ExecuteAsync(RequestMessage request, CancellationToken cancellationToken)
	{
		bool executed = false;

			// Ejecuta la solicitud
			try
			{
				// Log
				Manager.RaiseInfo("Start request");
				Manager.RaiseInfo(request.GetDebugString());
				// Evita las advertencias
				await Task.Delay(1, cancellationToken);
				// Escribe la cadena de depuración
				Manager.RaiseInfo("End request");
				// Indica que se ha ejecutado correctamente
				executed = true;
			}
			catch (Exception exception)
			{
				Manager.RaiseError($"Error when execute request. {exception.Message}");
			}
			// Devuelve el valor que indica si se ha ejecutado correctamente
			return executed;
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
}
