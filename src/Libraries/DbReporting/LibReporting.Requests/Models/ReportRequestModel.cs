using System;

using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Clase con los datos de solicitud de informe
	/// </summary>
	public class ReportRequestModel
	{
		/// <summary>
		///		Código de informe solicitado
		/// </summary>
		public string ReportId { get; set; }

		/// <summary>
		///		Dimensiones solicitadas
		/// </summary>
		public List<DimensionRequestModel> Dimensions { get; } = new List<DimensionRequestModel>();

		/// <summary>
		///		Expresiones solicitadas
		/// </summary>
		public List<ExpressionRequestModel> Expressions { get; } = new List<ExpressionRequestModel>();
	}
}
