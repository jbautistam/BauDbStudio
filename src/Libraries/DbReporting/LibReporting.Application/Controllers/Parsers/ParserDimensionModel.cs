using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers
{
	/// <summary>
	///		Modelo de interpretación de una dimensión
	/// </summary>
	internal class ParserDimensionModel
	{
		// Variables privadas
		private string _tableDimensionAlias;


		/// <summary>
		///		Obtiene todas las tablas definidas: la tabla normal + tablas adicionales
		/// </summary>
		internal List<string> GetAllTables()
		{
			List<string> tables = new()
									{
										Table
									};

				// Añade las tablas adicionales
				tables.AddRange(AdditionalTables);
				// Devuelve la colección de tablas
				return tables;
		}

		/// <summary>
		///		Clave de la dimensión
		/// </summary>
		internal string DimensionKey { get; set; }

		/// <summary>
		///		Tabla de la dimensión
		/// </summary>
		internal string Table { get; set; }

		/// <summary>
		///		Tablas adicionales de la dimensión
		/// </summary>
		internal List<string> AdditionalTables { get; } = new List<string>();

		/// <summary>
		///		Indica si se debe utilizar el alias de la tabla de dimnesión
		/// </summary>
		internal bool MustUseTableDimensionAlias
		{
			get { return !string.IsNullOrWhiteSpace(_tableDimensionAlias); }
		}

		/// <summary>
		///		Alias de la tabla de dimensión (para los casos en que es un JOIN contra la misma tabla)
		/// </summary>
		internal string TableDimensionAlias 
		{ 
			get
			{
				if (MustUseTableDimensionAlias)
					return _tableDimensionAlias;
				else
					return Table;
			}
			set { _tableDimensionAlias = value; }
		}

		/// <summary>
		///		Tabla relacionada
		/// </summary>
		internal string TableRelated { get; set; }

		/// <summary>
		///		Sql adicional para un JOIN
		/// </summary>
		internal string AdditionalJoinSql { get; set; }

		/// <summary>
		///		Indica si es obligatorio aunque no se haya solicitado
		/// </summary>
		internal bool Required { get; set; }

		/// <summary>
		///		Indica si se debe relacionar por los campos solicitados en la dimensión
		/// </summary>
		internal bool RelatedByFieldRequest { get; set; }

		/// <summary>
		///		Dimensiones relacionadas (obliga a hacer un join por esta dimensión aunque no se haya solicitado directamente)
		/// </summary>
		internal List<string> RelatedDimensions { get; } = new();

		/// <summary>
		///		Dimensiones que se deben comprobar que no se han solicitado
		/// </summary>
		internal List<string> IfNotRequestDimensions { get; } = new();

		/// <summary>
		///		Campos por los que se hace la unión
		/// </summary>
		internal List<(string fieldDimension, string fieldTable)> Fields { get; } = new();
	}
}
