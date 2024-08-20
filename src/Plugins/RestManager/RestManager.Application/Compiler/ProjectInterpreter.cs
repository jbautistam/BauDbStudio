using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Compiler;

/// <summary>
///		Intérprete de <see cref="RestProjectModel"/>
/// </summary>
internal class ProjectInterpreter
{
	internal ProjectInterpreter(RestProjectModel project)
	{
		Project = project;
	}

	/// <summary>
	///		Ejecuta un proyecto
	/// </summary>
	internal async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// Inicializa el contexto
		Contexts.Clear();
		foreach (ParameterModel parameter in Project.Parameters)
			Contexts.Add(parameter.Key, parameter.Value);
		// Ejecuta las sentencias
		await ExecuteSentencesAsync(Project.Steps, cancellationToken);
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
		RequestMessage request = new(step);

			// Asigna las propiedades
			request.Method = step.Method;
			request.Url = Parse(step.Url) ?? throw new ArgumentException("Can't find Url");
			request.Content = Parse(step.Content);
			request.Headers.AddRange(Parse(step.Headers));
			// Ejecuta la solicitud
			await ExecuteAsync(request, cancellationToken);
	}

	/// <summary>
	///		Interpreta las cabeceras
	/// </summary>
	private ParametersCollectionModel Parse(ParametersCollectionModel headers)
	{
		ParametersCollectionModel parsed = new();

			// Interpreta los datos
			foreach (ParameterModel parameter in headers)
				parsed.Add(parameter.Key, Parse(parameter.Value));
			// Devuelve los datos interpretador
			return parsed;
	}

	/// <summary>
	///		Interpreta un valor
	/// </summary>
	private string? Parse(string? value)
	{
		// Normaliza el valor
		value = Contexts.Parse(value);
		// Devuelve el valor interpretado
		return value;
	}

	/// <summary>
	///		Ejecuta una solicitud
	/// </summary>
	private async Task ExecuteAsync(RequestMessage request, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Escribe la cadena de depuración
		Console.WriteLine(request.Debug(0));
		// Salto
		Console.WriteLine(new string('-', 20));
	}

	/// <summary>
	///		Proyecto en ejecución
	/// </summary>
	public RestProjectModel Project { get; }

	/// <summary>
	///		Contextos
	/// </summary>
	private ContextStackModel Contexts { get; } = new();
}
