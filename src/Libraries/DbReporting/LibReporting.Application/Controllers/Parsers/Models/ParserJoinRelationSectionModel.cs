using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una relación para un <see cref="ParserJoinSectionModel"/>
/// </summary>
internal class ParserJoinRelationSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Añade los campos por los que se va a hacer el join
    /// </summary>
    internal void AddFieldsJoin(string fields)
    { 
        if (!string.IsNullOrWhiteSpace(fields))
        { 
            string[] parts = fields.Split('-');

                // Si sólo hay un campo, se utiliza el mismo nombre de campo
                if (parts.Length == 1)
                    Fields.Add((parts[0].TrimIgnoreNull(), parts[0].TrimIgnoreNull()));
                else
                    Fields.Add((parts[0].TrimIgnoreNull(), parts[1].TrimIgnoreNull()));
        }
        else
            throw new Exceptions.ReportingParserException("Undefined field in clause ON");
    }

    /// <summary>
    ///     Convierte los datos de <see cref="ClauseJoinModel"/> en este <see cref="ParserJoinRelationSectionModel"/>
    /// </summary>
	internal void Convert(ClauseJoinModel join) 
    {
        // Crea los datos de la dimensión
        Dimension = new();
        // Asigna las propiedades
        Dimension.DimensionKey = join.DimensionKey;
        Dimension.Required = join.Required;
        Dimension.RelatedDimensions.AddRange(join.RelatedRequestedDimensionKeys);
        // Asigna la tabla de dimensión
        Dimension.Table = join.TableRelated;
        // Asigna los campos relacionados
        foreach ((string fieldDimension, string fieldTable) in join.Relations)
            if (string.IsNullOrWhiteSpace(fieldTable))
                Fields.Add((fieldDimension, fieldDimension));
            else
                Fields.Add((fieldDimension, fieldTable));
    }

    /// <summary>
    ///		Dimensión sobre la que se hace el join
    /// </summary>
    internal ParserDimensionModel? Dimension { get; set; }

    /// <summary>
    ///		Indica si se debe relacionar por los campos solicitados en la dimensión
    /// </summary>
    internal bool RelatedByFieldRequest { get; set; }

    /// <summary>
    ///		Campos por los que se hace la unión
    /// </summary>
    internal List<(string fieldDimension, string fieldTable)> Fields { get; } = new();

    /// <summary>
    ///		Sql adicional para un JOIN
    /// </summary>
    internal string AdditionalJoinSql { get; set; } = string.Empty;
}
