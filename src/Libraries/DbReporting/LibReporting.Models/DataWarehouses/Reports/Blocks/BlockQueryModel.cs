using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks
{
    /// <summary>
    ///		Bloque de consulta de un informe
    /// </summary>
    public class BlockQueyModel : BaseBlockModel
    {
        public BlockQueyModel(string key) : base(key) {}

        /// <summary>
        ///     Campos adicionales de la consulta
        /// </summary>
        public List<ClauseFieldModel> Fields { get; } = new();

        /// <summary>
        ///     Relaciones de la consulta
        /// </summary>
        public List<ClauseJoinModel> Joins { get; } = new();

        /// <summary>
        ///     Filtros adicionales
        /// </summary>
        public List<ClauseFilterModel> Filters { get; } = new();

        /// <summary>
        ///		Consulta del bloque
        /// </summary>
        public string Sql { get; set; }
    }
}
