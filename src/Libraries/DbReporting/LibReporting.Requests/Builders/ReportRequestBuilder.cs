using System;

using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Requests.Builders
{
	/// <summary>
	///		Builder de <see cref="ReportRequestModel"/>
	/// </summary>
	public class ReportRequestBuilder
	{
		public ReportRequestBuilder(string reportId)
		{
			ReportRequest = new ReportRequestModel
										{
											ReportId = reportId
										};
		}

		/// <summary>
		///		Añade una <see cref="DimensionRequestModel"/> a la solicitud
		/// </summary>
		public ColumnRequestBuilder WithDimension(string dimensionId, string columnId)
		{
			return new ColumnRequestBuilder(this, new DimensionRequestModel
																{
																	DimensionId = dimensionId,
																	ColumnId = columnId
																}
										   );
		}

		/// <summary>
		///		Añade una <see cref="ExpressionRequestModel"/> a la solicitud
		/// </summary>
		public ColumnRequestBuilder WithExpression(string reportDataSourceId, string columnId)
		{
			return new ColumnRequestBuilder(this, new ExpressionRequestModel
															{
																ReportDataSourceId = reportDataSourceId,
																ColumnId = columnId,
															}
										    );
		}

		/// <summary>
		///		Genera el resultado
		/// </summary>
		public ReportRequestModel Build()
		{
			return ReportRequest;
		}

		/// <summary>
		///		Solicitud de informe
		/// </summary>
		internal ReportRequestModel ReportRequest { get; }
	}
}