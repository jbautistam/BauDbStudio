using System;

using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.LibReporting.Models
{
	/// <summary>
	///		Clase con los datos del esquema de BI
	/// </summary>
	public class ReportingSchemaModel
	{
		/// <summary>
		///		Limpia el esquema
		/// </summary>
		public void Clear()
		{
			DataWarehouses.Clear();
		}

		/// <summary>
		///		Capa de almacenes de datos
		/// </summary>
		public Base.BaseReportingDictionaryModel<DataWarehouses.DataWarehouseModel> DataWarehouses { get; } = new Base.BaseReportingDictionaryModel<DataWarehouses.DataWarehouseModel>();
	}
}
