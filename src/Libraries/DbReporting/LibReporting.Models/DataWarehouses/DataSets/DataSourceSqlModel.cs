using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets
{
	/// <summary>
	///		Origen de datos con una cadena SQL
	/// </summary>
	public class DataSourceSqlModel : BaseDataSourceModel
	{
		public DataSourceSqlModel(DataWarehouseModel dataWarehouse) : base(dataWarehouse) {}

		/// <summary>
		///		Comando SQL de consulta
		/// </summary>
		public string Sql { get; set; }

		/// <summary>
		///		Parámetros asociados a la consulta
		/// </summary>
		public List<DataSourceSqlParameterModel> Parameters { get; } = new List<DataSourceSqlParameterModel>();
	}
}
