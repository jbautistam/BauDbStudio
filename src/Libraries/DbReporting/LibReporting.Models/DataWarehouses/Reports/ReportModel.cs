using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports
{
	/// <summary>
	///		Clase con los datos de un informe
	/// </summary>
	public class ReportModel : Base.BaseReportingModel
	{
		public ReportModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		Nombre del <see cref="ReportModel"/>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Descripción del <see cref="ReportModel"/>
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		<see cref="DataWarehouseModel"/> al que se asocia este <see cref="ReportModel"/>
		/// </summary>
		public DataWarehouseModel DataWarehouse { get; }

		/// <summary>
		///		Orígenes de datos de las expresiones del informe
		/// </summary>
		public List<ReportDataSourceModel> ReportDataSources { get; } = new List<ReportDataSourceModel>();
	}
}
