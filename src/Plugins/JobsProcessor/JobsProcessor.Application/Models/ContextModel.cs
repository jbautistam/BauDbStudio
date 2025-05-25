namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Clase con los datos de contexto
/// </summary>
public class ContextModel
{
	/// <summary>
	///		Id del contexto
	/// </summary>
	public string Id { get; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Parámetros del contexto
	/// </summary>
	public List<ParameterModel> Parameters { get; } = [];
}
