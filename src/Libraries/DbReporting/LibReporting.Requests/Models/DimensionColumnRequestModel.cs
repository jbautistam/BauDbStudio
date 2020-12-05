using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Colunma solicitada de una dimensión
	/// </summary>
	public class DimensionColumnRequestModel : BaseColumnRequestModel
	{
		/// <summary>
		///		Código de dimensión
		/// </summary>
		public string DimensionId { get; set; }

		/// <summary>
		///		Código de columna
		/// </summary>
		public string ColumnId { get; set; }

		/// <summary>
		///		Solicitudes de dimensiones hija
		/// </summary>
		public List<DimensionColumnRequestModel> Childs { get; } = new List<DimensionColumnRequestModel>();
	}
}
