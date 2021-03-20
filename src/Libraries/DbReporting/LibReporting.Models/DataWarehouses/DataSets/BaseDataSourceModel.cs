using System;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets
{
	/// <summary>
	///		Clase base para los orígenes de datos
	/// </summary>
	public abstract class BaseDataSourceModel : Base.BaseReportingModel
	{
		protected BaseDataSourceModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		Comprueba si este origen de datos tiene alguna columna con el Id solicitado
		/// </summary>
		public bool HasColumn(string columnId)
		{
			return Columns[columnId] != null;
		}

		/// <summary>
		///		Almacén de datos
		/// </summary>
		public DataWarehouseModel DataWarehouse { get; }

		/// <summary>
		///		Columnas visibles de la tabla
		/// </summary>
		public Base.BaseReportingDictionaryModel<DataSourceColumnModel> Columns { get; } = new Base.BaseReportingDictionaryModel<DataSourceColumnModel>();
	}
}
