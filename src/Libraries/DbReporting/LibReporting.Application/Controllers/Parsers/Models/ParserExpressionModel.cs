using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models
{
    /// <summary>
    ///		Modelo de interpretación de una expresión
    /// </summary>
    internal class ParserExpressionModel
    {
        /// <summary>
        ///		Clave de las expresiones solicitadas
        /// </summary>
        internal List<string> ExpressionKeys { get; } = new();

        /// <summary>
        ///		Consulta SQL a añadir cuando se solicita la expresión
        /// </summary>
        internal string Sql { get; set; }
    }
}
