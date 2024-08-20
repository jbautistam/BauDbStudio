namespace Bau.Libraries.LibPatternText;

/// <summary>
///		Manager de la aplicación
/// </summary>
public class PatternManager
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

	/// <summary>
	///		Convierte una cadena de texto en una serie de cadenas correspondientes a un patrón
	/// </summary>
	public string Convert(Models.PatternModel pattern) => new Domain.Parsers.FormulaConversor(pattern).Convert();
}
