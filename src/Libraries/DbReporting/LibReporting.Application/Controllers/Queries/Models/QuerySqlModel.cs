using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models
{
	/// <summary>
	///		Datos de una consulta SQL
	/// </summary>
	internal class QuerySqlModel
	{
		/// <summary>
		///		Tipo de consulta
		/// </summary>
		internal enum QueryType
		{
			/// <summary>Bloque de consultas</summary>
			Block,
			/// <summary>Cte</summary>
			Cte,
			/// <summary>Consulta</summary>
			Query,
			/// <summary>Bloque de ejecución</summary>
			Execution
		}

		internal QuerySqlModel(QueryType type)
		{
			Type = type;
		}

		/// <summary>
		///		Tipo de consulta
		/// </summary>
		internal QueryType Type { get; }

		/// <summary>
		///		Clave de la consulta
		/// </summary>
		internal string Key { get; set; }

		/// <summary>
		///		Cadena SQL
		/// </summary>
		internal string Sql { get; set; }

		/// <summary>
		///		Consultas hija
		/// </summary>
		internal List<QuerySqlModel> Queries { get; } = new();
	}
}
