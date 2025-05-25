namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Clase con los datos de un proyecto
/// </summary>
public class ProjectModel
{
	/// <summary>
	///		Contextos de ejecución
	/// </summary>
	public List<ContextModel> Contexts { get; } = [];

	/// <summary>
	///		Comandos
	/// </summary>
	public List<CommandModel> Commands { get; } = [];
}
