using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Colunma solicitada de una dimensión
	/// </summary>
	public class DimensionRequestModel
	{
		/// <summary>
		///		Obtiene la solicitud de columna
		/// </summary>
		public DimensionColumnRequestModel GetRequestColumn(string columnId)
		{
			// Obtiene la columna asociada
			foreach (DimensionColumnRequestModel column in Columns)
				if (column.ColumnId.Equals(columnId, StringComparison.CurrentCultureIgnoreCase))
					return column;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Código de dimensión
		/// </summary>
		public string DimensionId { get; set; }

		/// <summary>
		///		Columnas
		/// </summary>
		public List<DimensionColumnRequestModel> Columns { get; } = new List<DimensionColumnRequestModel>();

		/// <summary>
		///		Solicitudes de dimensiones hija
		/// </summary>
		public List<DimensionRequestModel> Childs { get; } = new List<DimensionRequestModel>();
	}
}
