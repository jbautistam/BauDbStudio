using System;
using System.Collections.Generic;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models
{
    /// <summary>
    ///		Sección devuelta por el intérprete de secciones
    /// </summary>
    internal class ParserSectionModel
    {
        /// <summary>
        ///		Tipo de sección
        /// </summary>
        internal enum SectionType
        {
            /// <summary>Campos</summary>
            Fields,
            /// <summary>Join</summary>
            Join,
            /// <summary>Cláusula GROUP BY</summary>
            GroupBy,
            /// <summary>Cláusula PARTITION BY</summary>
            PartitionBy,
            /// <summary>Comprobación de campos NULL</summary>
            CheckNull,
            /// <summary>Obtiene los campos de dos tablas: IfNull(x, y). Úil en los FULL OUTER JOIN</summary>
            FieldsIfNull,
            /// <summary>SQL asociada a si se ha solicitado una expresión</summary>
            IfRequestExpression
        }

        internal ParserSectionModel(string placeholder, SectionType type)
        {
            Placeholder = placeholder;
            Type = type;
        }

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
        ///		Convierte un <see cref="ClauseJoinModel"/> en un <see cref="ParserSectionModel"/>
        /// </summary>
        internal void Convert(ClauseJoinModel join, string tableDimension)
        {
            // Asigna los valores básicos
            Join = join.Type;
            // Asigna las dimensiones
            ParserDimensions.Add(ConvertToDimension(join, tableDimension));
        }

        /// <summary>
        ///		Convierte un <see cref="ClauseJoinModel"/> en un <see cref="ParserDimensionModel"/>
        /// </summary>
        private ParserDimensionModel ConvertToDimension(ClauseJoinModel join, string tableDimension)
        {
            ParserDimensionModel parserDimension = new();

            // Asigna las propiedades
            parserDimension.DimensionKey = join.DimensionKey;
            parserDimension.Required = join.Required;
            parserDimension.RelatedDimensions.AddRange(join.RelatedRequestedDimensionKeys);
            // Asigna las tablas de dimensiones y relacionada
            //? Las pone al revés por la forma en que se hace el JOIN, venimos de una definición de dimensión, por tanto la tabla de dimensión es la base
            parserDimension.Table = join.TableRelated;
            parserDimension.TableRelated = tableDimension;
            // Asigna los campos relacionados
            foreach ((string fieldDimension, string fieldTable) in join.Relations)
                if (string.IsNullOrWhiteSpace(fieldTable))
                    parserDimension.Fields.Add((fieldDimension, fieldDimension));
                else
                    parserDimension.Fields.Add((fieldDimension, fieldTable));
            // Devuelve la dimensión
            return parserDimension;
        }

        /// <summary>
        ///		Placeholder leido
        /// </summary>
        internal string Placeholder { get; }

        /// <summary>
        ///		Tipo de sección
        /// </summary>
        internal SectionType Type { get; }

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
        internal List<ParserExpressionModel> ParserExpressions { get; } = new();
    }
}
