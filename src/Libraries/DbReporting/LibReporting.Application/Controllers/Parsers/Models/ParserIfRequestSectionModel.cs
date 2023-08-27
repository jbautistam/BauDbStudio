using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una expresión
/// </summary>
internal class ParserIfRequestSectionModel : ParserBaseSectionModel
{
	/// <summary>
	///		Obtiene la SQL adecuada para esta sección
	/// </summary>
	internal string GetSql(ReportRequestModel request)
	{
		string sql;

			// Añade la SQL para las expresiones normales
			sql = GetSql(Expressions, request);
			// Se añade la SQL para las expresiones de totales
			if (request.IsRequestedTotals())
				sql = sql.AddWithSeparator(GetSql(WhenRequestTotals, request), "," + Environment.NewLine);
			// Añade una coma si es necesario
			if (WithComma && !string.IsNullOrWhiteSpace(sql))
				sql += ", ";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de una lista de expresiones
	/// </summary>
	//TODO: la comprobación de expresiones solicitadas debería ir a la Request
	private string GetSql(List<ParserIfRequestExpressionSectionModel> expressions, ReportRequestModel request)
	{
		string sql = string.Empty;

			// Añade las SQL de las expresiones solicitadas
			foreach (ParserIfRequestExpressionSectionModel expression in expressions)
				if (expression.IsDefault || IsRequested(expression, request))
				{
					string separator = ", ";

						// Quita el separador si la sección indica que no se deben añadir
						if (expression.WithoutComma)
							separator = string.Empty;
						// Añade la consulta SQL
						sql = sql.AddWithSeparator(expression.Sql.TrimIgnoreNull(), separator + Environment.NewLine);
				}
			// Devuelve la cadena solicitada
			return sql;
	}

	/// <summary>
	///		Comprueba si se ha solicitado alguna de las expresiones asociadas
	/// </summary>
	private bool IsRequested(ParserIfRequestExpressionSectionModel expression, ReportRequestModel request)
	{
		// Comprueba si se ha solicitado alguna de las expresiones
		foreach (string key in expression.ExpressionKeys)
			if (request.IsRequestedExpression(key))
				return true;
		// Si ha llegado hasta aquí es porque no ha solicitado nada
		return false;
	}

	/// <summary>
	///     Expresiones solicitadas
	/// </summary>
	internal List<ParserIfRequestExpressionSectionModel> Expressions { get; } = new();

    /// <summary>
    ///     Expresiones que se comprueban cuando se solicitan totales
    /// </summary>
    internal List<ParserIfRequestExpressionSectionModel> WhenRequestTotals { get; } = new();

    /// <summary>
    ///     Indica si se debe añadir una coma al generar la SQL
    /// </summary>
    internal bool WithComma { get; set; }
}