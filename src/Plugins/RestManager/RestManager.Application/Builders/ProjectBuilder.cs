using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Builders;

/// <summary>
///		Generador de <see cref="RestProjectModel"/>
/// </summary>
public class ProjectBuilder
{
	public ProjectBuilder(string name)
	{
		Project = new RestProjectModel
						{
							Name = name
						};
	}

	/// <summary>
	///		Asigna la descripción
	/// </summary>
	public ProjectBuilder WithDescription(string description)
	{
		// Asigna la descripción
		Project.Description = description;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna un parámetro
	/// </summary>
	public ProjectBuilder WithParameter(string key, string value)
	{
		// Asigna el parámetro
		Project.Parameters.Add(key, value);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna una cabecera
	/// </summary>
	public ProjectBuilder WithHeader(string key, string value)
	{
		// Asigna la cabecera
		Project.Headers.Add(key, value);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna un paso
	/// </summary>
	public RestStepBuilder WithRestStep(string name, RestStepModel.RestMethod method, string url) => new RestStepBuilder(this, name, method, url);

	/// <summary>
	///		Genera el proyecto
	/// </summary>
	public RestProjectModel Build() => Project;

	/// <summary>
	///		Sobrecarga del operador de casting
	/// </summary>
	public static implicit operator RestProjectModel(ProjectBuilder builder) => builder.Build();

	/// <summary>
	///		Proyecto
	/// </summary>
	public RestProjectModel Project { get; }
}
