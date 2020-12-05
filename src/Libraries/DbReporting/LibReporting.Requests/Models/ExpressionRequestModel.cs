using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Clase con los datos de una columna solicitada para un listado
	/// </summary>
	public class ExpressionRequestModel
	{
		/// <summary>
		///		Clave del informe de origen de datos
		/// </summary>
		public string ReportDataSourceId { get; set; }

		/// <summary>
		///		Columnas solicitada
		/// </summary>
		public List<ExpressionColumnRequestModel> Columns { get; } = new List<ExpressionColumnRequestModel>();
	}
}