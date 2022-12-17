using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks
{
	/// <summary>
	///		Claúsula para asignar una relación
	/// </summary>
	/// <example>
	/// <Join Dimension = "Users" Table = "UsersCte">
	///		<Relation FromDimension = "UserId" RelatedTo = "UserId" />
	/// </Join>
	/// </example>
	public class ClauseJoinModel
	{
		/// <summary>
		///		Tipo de join
		/// </summary>
		public enum JoinType
		{
			/// <summary>INNER JOIN</summary>
			InnerJoin,
			/// <summary>LEFT JOIN</summary>
			LeftJoin,
			/// <summary>RIGHT JOIN</summary>
			RightJoin,
			/// <summary>FULL OUTER JOIN</summary>
			FullJoin,
			/// <summary>CROSS JOIN</summary>
			CrossJoin
		}

		/// <summary>
		///		Añade claves de dimensiones asociadas (dimensiones que si se solicitan, obligan a hacer un JOIN con esta
		///		dimensión aunque no se haya solicitado explícitamente)
		/// </summary>
		public void AddRelatedRequestDimensionKeys(string dimensions)
		{
			if (!string.IsNullOrWhiteSpace(dimensions))
				foreach (string dimension in dimensions.Split(';'))
					if (!string.IsNullOrWhiteSpace(dimension))
						RelatedRequestedDimensionKeys.Add(dimension);
		}

		/// <summary>
		///		Tipo de relación
		/// </summary>
		public JoinType Type { get; set; } = JoinType.InnerJoin;

		/// <summary>
		///		Clave de la dimensión
		/// </summary>
		public string DimensionKey { get; set; }

		/// <summary>
		///		Indica si la unión es obligatoria aunque no se haya seleccionado ningún campo
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		///		Claves de las dimensiones que si se solicitan pueden provocar esta unión con esta dimensión aunque no se haya solicitado
		/// </summary>
		public List<string> RelatedRequestedDimensionKeys { get; } = new();

		/// <summary>
		///		Alias de la tabla relacionada con la dimensión
		/// </summary>
		public string TableRelated { get; set; }

		/// <summary>
		///		Relaciones
		/// </summary>
		public List<(string fieldDimension, string fieldTable)> Relations { get; } = new();
	}
}
