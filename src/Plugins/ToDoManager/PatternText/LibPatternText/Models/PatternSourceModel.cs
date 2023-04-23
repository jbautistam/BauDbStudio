namespace Bau.Libraries.LibPatternText.Models;

/// <summary>
///		Fuente sobre la que se va a aplicar el patrón
/// </summary>
public class PatternSourceModel
{
	public PatternSourceModel(string source)
	{
		Source = source;
	}

	/// <summary>
	///		Texto sobre el que se van a aplicar los patrones
	/// </summary>
	public string Source { get; }

	/// <summary>
	///		Indica si el texto tiene cabeceras
	/// </summary>
	public bool WithHeader { get; set; } = true;

	/// <summary>
	///		Separador de campos
	/// </summary>
	public string Separator { get; set; } = ",";

	/// <summary>
	///		Carácter que se considera comillas (para unir varios campos)
	/// </summary>
	public string? QuoteChar { get; set; } = "\"";
}
