using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks
{
    /// <summary>
    ///		Bloque de creación de una CTE
    /// </summary>
    public class BlockCreateCteModel : BaseBlockModel
    {
        public BlockCreateCteModel(string key) : base(key) {}

        /// <summary>
        ///     Clave de la dimensión
        /// </summary>
        public string DimensionKey { get; set; }

        /// <summary>
        ///		Consulta
        /// </summary>
        public BlockQueyModel Query { get; set; }
    }
}
