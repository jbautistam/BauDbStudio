using System;

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
	}
}
