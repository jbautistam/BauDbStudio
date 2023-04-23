namespace Bau.Libraries.LibPatternText
{
	/// <summary>
	///		Manager de la aplicación
	/// </summary>
	public class PatternManager
	{
		/// <summary>
		///		Convierte una cadena de texto en una serie de cadenas correspondientes a un patrón
		/// </summary>
		public string Convert(Models.PatternModel pattern)
		{
			return new Domain.Parsers.FormulaConversor(pattern).Convert();
		}
	}
}
