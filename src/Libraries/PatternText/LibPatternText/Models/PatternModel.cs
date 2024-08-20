namespace Bau.Libraries.LibPatternText.Models;

/// <summary>
///		Clase con los datos de origen y fórmula de un patrón
/// </summary>
public class PatternModel
{
	/// <summary>
	///		Texto sobre el que se van a aplicar los patrones
	/// </summary>
	public string Source { get; set; } = default!;

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
	public string QuoteChar { get; set; } = "\"";

	/// <summary>
	///		Extensión de la salida
	/// </summary>
	public string ExtensionHighlight { get; set; } = default!;

	/// <summary>
	///		Fórmula a aplicar
	/// </summary>
	public string Formula { get; set; } = default!;
}
