using System;

using Bau.Libraries.LibDataStructures.Base;

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
		public BaseExtendedModelCollection<DataWarehouses.DataWarehouseModel> DataWarehouses { get; } = new BaseExtendedModelCollection<DataWarehouses.DataWarehouseModel>();
	}
}
