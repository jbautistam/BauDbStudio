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
		///		Columnas solicitadas
		/// </summary>
		public List<BaseColumnRequestModel> Columns { get; } = new List<BaseColumnRequestModel>();
	}
}
