using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Base;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports
{
	/// <summary>
	///		Clase con los datos de un informe
	/// </summary>
	public class ReportModel : BaseExtendedModel
	{
		public ReportModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

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
