using System;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Clase con los datos de una columna solicitada para un listado
	/// </summary>
	public class ExpressionRequestModel : BaseColumnRequestModel
	{
		/// <summary>
		///		Modo de agregación por esta columna
		/// </summary>
		public enum AggregationType
		{
			/// <summary>Sin agregación</summary>
			NoAggregated,
			/// <summary>Suma</summary>
			Sum,
			/// <summary>Valor máximo</summary>
			Max,
			/// <summary>Valor mínimo</summary>
			Min,
			/// <summary>Media</summary>
			Average,
			/// <summary>Desviación estándar</summary>
			StandardDeviation
		}

		/// <summary>
		///		Clave del informe de origen de datos
		/// </summary>
		public string ReportDataSourceId { get; set; }

		/// <summary>
		///		Código de columna solicitada
		/// </summary>
		public string ColumnId { get; set; }

		/// <summary>
		///		Modo de agregación
		/// </summary>
		public AggregationType AggregatedBy { get; set; }
	}
}