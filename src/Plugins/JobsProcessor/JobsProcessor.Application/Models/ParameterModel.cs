namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Clase con los datos de un parámetro
/// </summary>
public class ParameterModel
{
	/// <summary>
	///		Nombre del argumento
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Valor del argumento
	/// </summary>
	public string Value { get; set; } = default!;
}
