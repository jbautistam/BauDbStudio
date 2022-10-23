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
		///		Compara el valor de dos elementos para ordenarlo
		/// </summary>
		public override int CompareTo(Base.BaseReportingModel item)
		{
			if (item is DataSourceSqlModel dataSource)
				return Id.CompareTo(dataSource.Id);
			else
				return -1;
		}

		/// <summary>
		///		Obtiene el nombre de tabla
		/// </summary>
		public override string GetTableAlias()
		{
			return Id;
		}

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
