using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Managers;

/// <summary>
///		Manager de procesamiento Rest
/// </summary>
public class RestProjectManager
{
	/// <summary>
	///		Carga el proyecto
	/// </summary>
	public ProjectModel Load(string fileName) => new Repositories.ProjectRepository().Load(fileName);

	/// <summary>
	///		Graba el proyecto
	/// </summary>
	public void Save(ProjectModel project, string fileName)
	{
		new Repositories.ProjectRepository().Save(project, fileName);
	}

	/// <summary>
	///		Ejecuta un proyecto
	/// </summary>
	public async Task ExecuteAsync(ProjectModel project, CancellationToken cancellationToken)
	{
		await new Compiler.ProjectInterpreter(project).ExecuteAsync(cancellationToken);
	}
}
