namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una expresión
/// </summary>
internal class ParserIfRequestExpressionSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Añade una expresión
    /// </summary>
    internal void AddExpressions(string expression)
    { 
        if (!string.IsNullOrWhiteSpace(expression))
        { 
            string [] parts = expression.Split('-');

                foreach (string part in parts)
                    if (!string.IsNullOrWhiteSpace(part))
                        ExpressionKeys.Add(part.Trim());
        }
    }

	/// <summary>
	///		Clave de las expresiones solicitadas
	/// </summary>
	internal List<string> ExpressionKeys { get; } = new();

    /// <summary>
    ///		Consulta SQL a añadir cuando se solicita la expresión
    /// </summary>
    internal string Sql { get; set; } = default!;
}
