using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports
{
	/// <summary>
	///		Clase base con los datos de un informe
	/// </summary>
	public abstract class ReportBaseModel : Base.BaseReportingModel
	{
		protected ReportBaseModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		Compara el valor de dos elementos para ordenarlo
		/// </summary>
		public override int CompareTo(Base.BaseReportingModel item)
		{
			if (item is ReportBaseModel report)
				return Id.CompareTo(report.Id);
			else
				return -1;
		}

		/// <summary>
		///		Descripción del <see cref="ReportModel"/>
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		<see cref="DataWarehouseModel"/> al que se asocia este <see cref="ReportModel"/>
		/// </summary>
		public DataWarehouseModel DataWarehouse { get; }
	}
}
