using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de dimensiones / campos
/// </summary>
internal class ParserFieldsSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Tipo de unión
    /// </summary>
    internal ClauseJoinModel.JoinType Join { get; set; }

    /// <summary>
    ///		Indica si se debe añadir una coma
    /// </summary>
    internal bool WithComma { get; set; }

    /// <summary>
    ///		Indica si es obligatorio
    /// </summary>
    internal bool Required { get; set; }

    /// <summary>
    ///		Dimensiones interpretadas
    /// </summary>
    internal List<ParserDimensionModel> ParserDimensions { get; } = new();

    /// <summary>
    ///     Expresiones interpretadas
    /// </summary>
    internal List<ParserIfRequestExpressionSectionModel> ParserExpressions { get; } = new();
}
