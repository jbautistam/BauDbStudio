namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una sección WHERE
/// </summary>
/// <example>
/// -Where 
///		-Equal
///		    ... <see cref="ParserWhereConditionSectionModel"/>
///		-Additional
///		    SalesAnalysisAccumulated.Date BETWEEN @StartDate AND SalesAnalysis.Date
///		        AND SalesAnalysisAccumulated.Refund = 0
/// </example>
internal class ParserWhereSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Condiciones
    /// </summary>
    internal List<ParserWhereConditionSectionModel> Conditions { get; } = new();

    /// <summary>
    ///     Sql adicional a la condición
    /// </summary>
    internal string AdditionalSql { get; set; } = string.Empty;
}
