using System;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Colunma solicitada de una dimensión
	/// </summary>
	public class DimensionRequestModel : BaseColumnRequestModel
	{
		/// <summary>
		///		Código de dimensión
		/// </summary>
		public string DimensionId { get; set; }

		/// <summary>
		///		Código de columna
		/// </summary>
		public string ColumnId { get; set; }
	}
}
