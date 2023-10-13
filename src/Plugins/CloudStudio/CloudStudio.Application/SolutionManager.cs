using Microsoft.Extensions.Logging;

using Bau.Libraries.CloudStudio.Models;

namespace Bau.Libraries.CloudStudio.Application;

/// <summary>
///		Manager de soluciones
/// </summary>
public class SolutionManager
{
	public SolutionManager(ILogger logger)
	{
		Logger = logger;
	}

	/// <summary>
	///		Carga los datos de configuración
	/// </summary>
	public SolutionModel LoadConfiguration(string fileName) => new Repository.SolutionRepository().Load(fileName);

	/// <summary>
	///		Graba los datos de una solución
	/// </summary>
	public void SaveSolution(SolutionModel solution, string fileName)
	{
		new Repository.SolutionRepository().Save(solution, fileName);
	}

	/// <summary>
	///		Manager de log
	/// </summary>
	public ILogger Logger { get; }
}