using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un JOIN
/// </summary>
internal class ParserJoinSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Obtiene la cadena adecuada para un tipo de JOIN
    /// </summary>
    internal string GetJoin()
    {
        switch (Join)
        {
            case ClauseJoinModel.JoinType.InnerJoin:
                return " INNER JOIN ";
            case ClauseJoinModel.JoinType.CrossJoin:
                return " CROSS JOIN ";
            case ClauseJoinModel.JoinType.FullJoin:
                return " FULL OUTER JOIN ";
            case ClauseJoinModel.JoinType.LeftJoin:
                return " LEFT JOIN ";
            case ClauseJoinModel.JoinType.RightJoin:
                return " RIGHT JOIN ";
            default:
                throw new Exceptions.ReportingParserException($"Join type unknown: {Join.ToString()}");
        }
    }

    /// <summary>
    ///     Convierte los datos de <see cref="ClauseJoinModel"/> en este <see cref="ParserJoinSectionModel"/>
    /// </summary>
	internal void Convert(ClauseJoinModel join, string tableDimension) 
    {
        // Asigna los valores básicos
        Join = join.Type;
        Table = tableDimension;
        // Asigna las relaciones
        Relations.Add(CreateRelation(join));
    }

    /// <summary>
    ///     Crea una relación a partir de la cláusula
    /// </summary>
    private ParserJoinRelationSectionModel CreateRelation(ClauseJoinModel join)
    {
        ParserJoinRelationSectionModel relation = new();

            // Interpreta la relación
            relation.Convert(join);
            // Devuelve la relación
            return relation;
    }

	/// <summary>
	///		Tipo de unión
	/// </summary>
	internal ClauseJoinModel.JoinType Join { get; set; }

    /// <summary>
    ///     Nombre de tabla
    /// </summary>
    internal string Table { get; set; } = default!;

    /// <summary>
    ///     Alias de la tabla
    /// </summary>
    internal string TableAlias { get; set; } = default!;

    /// <summary>
    ///     Tabla con la que se hace el join
    /// </summary>
    internal string TableJoin
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(TableAlias))
                return TableAlias;
            else if (!string.IsNullOrWhiteSpace(Table))
                return Table;
            else
                return string.Empty;
        }
    }

    /// <summary>
    ///		Indica si es obligatorio
    /// </summary>
    internal bool Required { get; set; }

    /// <summary>
    ///     Relaciones asociadas al JOIN
    /// </summary>
    internal List<ParserJoinRelationSectionModel> Relations { get; } = new();
}
