namespace Bau.Libraries.LibPatternText.Models;

/// <summary>
///		Formula que se aplica sobre <see cref="PatternSourceModel"/>
/// </summary>
public class PatternFormulaModel
{
	public PatternFormulaModel(string text)
	{
		Text = text;
	}

	/// <summary>
	///		Texto
	/// </summary>
	public string Text { get; }
}
