using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Builders;

/// <summary>
///		Generador de <see cref="RestStepModel"/>
/// </summary>
public class RestStepBuilder
{
	public RestStepBuilder(ProjectBuilder builder, string name, RestStepModel.RestMethod method, string url)
	{
		Builder = builder;
		Step = new RestStepModel
						{
							Name = name,
							Method = method,
							Url = url
						};
	}

	/// <summary>
	///		Asigna una descripción
	/// </summary>
	public RestStepBuilder WithDescription(string description)
	{
		// Asigna la descripción
		Step.Description = description;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna el contenido
	/// </summary>
	public RestStepBuilder WithContent(string content)
	{
		// Asigna el contenido
		Step.Content = content;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna el valor que indica si está activo
	/// </summary>
	public RestStepBuilder WithEnabled(bool enabled)
	{
		// Asigna el valor
		Step.Enabled = enabled;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna una cabecera
	/// </summary>
	public RestStepBuilder WithHeader(string key, string value)
	{
		// Asigna el contenido
		Step.Headers.Add(key, value);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Vuelve hacia atrás
	/// </summary>
	public ProjectBuilder Back()
	{
		// Añade el paso
		Builder.Project.Steps.Add(Step);
		// Devuelve el generador
		return Builder;
	}

	/// <summary>
	///		Generador de proyectos
	/// </summary>
	private ProjectBuilder Builder { get; }

	/// <summary>
	///		Paso generado
	/// </summary>
	public RestStepModel Step { get; }
}
