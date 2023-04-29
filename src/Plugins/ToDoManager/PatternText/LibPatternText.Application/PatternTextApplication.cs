namespace Bau.Libraries.LibPatternText.Application;

/// <summary>
///		Clase de aplicación para la librería de patrones de texto
/// </summary>
public class PatternTextApplication
{
	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	public Models.PatternModel Load(string fileName) => new Repositories.PatternTextRepository().Load(fileName);

	/// <summary>
	///		Graba los datos de un patrón en un archivo
	/// </summary>
	public void Save(string fileName, Models.PatternModel pattern)
	{
		new Repositories.PatternTextRepository().Save(fileName, pattern);
	}
}
