namespace Bau.Libraries.LibPatternText.Models;

/// <summary>
///		Resultado de la aplicación de <see cref="PatternFormulaModel"/> sobre <see cref="PatternSourceModel"/>
/// </summary>
public class PatternResultModel
{
	public PatternResultModel(string text)
	{
		Text = text;
	}

	/// <summary>
	///		Texto resultado
	/// </summary>
	public string Text { get; }

	/// <summary>
	///		Errores
	/// </summary>
	public List<string> Errors { get; } = new();
}
