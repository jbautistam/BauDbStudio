using System;

using Bau.Libraries.LibDataStructures.Base;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses
{
	/// <summary>
	///		Clase con los datos de un almacén de datos
	/// </summary>
	public class DataWarehouseModel : BaseExtendedModel
	{
		public DataWarehouseModel(ReportingSchemaModel schema)
		{
			Schema = schema;
		}

		/// <summary>
		///		Esquema
		/// </summary>
		public ReportingSchemaModel Schema { get; }

		/// <summary>
		///		Dimensiones del esquema
		/// </summary>
		public BaseExtendedModelCollection<Dimensions.DimensionModel> Dimensions { get; } = new BaseExtendedModelCollection<Dimensions.DimensionModel>();

		/// <summary>
		///		Orígenes de datos
		/// </summary>
		public BaseExtendedModelCollection<DataSets.BaseDataSourceModel> DataSources { get; } = new BaseExtendedModelCollection<DataSets.BaseDataSourceModel>();

		/// <summary>
		///		Informes
		/// </summary>
		public BaseExtendedModelCollection<Reports.ReportModel> Reports { get; } = new BaseExtendedModelCollection<Reports.ReportModel>();
	}
}
