namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una sección WHERE
/// </summary>
/// <example>
/// Where 
///		-DataSource: Name
///		    --Table: TableAlias
///		-Operator: AND | OR | ...
/// </example>
internal class ParserWhereSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Bloques con los orígenes de datos
    /// </summary>
    internal List<ParserDataSourceModel> DataSources { get; } = new();

    /// <summary>
    ///     Operador inicial de la condición
    /// </summary>
    internal string Operator { get; set; } = string.Empty;
}
