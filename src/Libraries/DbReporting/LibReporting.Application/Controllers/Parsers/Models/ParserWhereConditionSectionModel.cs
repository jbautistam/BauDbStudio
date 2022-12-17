namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una condición para un WHERE
/// </summary>
/// <example>
/// -Equal
///     -Dimension: UsersPointOfSale
///         -Table: UsersPointOfSaleCte
///     -Table: UsersPointOfSaleAccumulatedCte
///     -Template
///         IsNull(Dimension.Field, '') = IsNull(Table.Field, '')
/// </example>
internal class ParserWhereConditionSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Dimensión
    /// </summary>
    internal ParserDimensionModel? Dimension { get; set; }

    /// <summary>
    ///     Tabla principal sobre la que se aplica la condición
    /// </summary>
    internal string Table { get; set; } = default!;

    /// <summary>
    ///     Plantilla de aplicación del WHERE
    /// </summary>
    internal string Template { get; set; } = default!;
}
