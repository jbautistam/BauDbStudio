using System;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses
{
	/// <summary>
	///		Clase con los datos de un almacén de datos
	/// </summary>
	public class DataWarehouseModel : Base.BaseReportingModel
	{
		public DataWarehouseModel(ReportingSchemaModel schema)
		{
			Schema = schema;
		}

		/// <summary>
		///		Compara el valor de dos elementos para ordenarlo
		/// </summary>
		public override int CompareTo(Base.BaseReportingModel item)
		{
			if (item is DataWarehouseModel dataWarehouse)
				return Name.CompareTo(dataWarehouse.Name);
			else
				return -1;
		}

		/// <summary>
		///		Esquema
		/// </summary>
		public ReportingSchemaModel Schema { get; }

		/// <summary>
		///		Nombre del elemento
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Descripción del elemento
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		Dimensiones del esquema
		/// </summary>
		public Base.BaseReportingDictionaryModel<Dimensions.DimensionModel> Dimensions { get; } = new Base.BaseReportingDictionaryModel<Dimensions.DimensionModel>();

		/// <summary>
		///		Orígenes de datos
		/// </summary>
		public Base.BaseReportingDictionaryModel<DataSets.BaseDataSourceModel> DataSources { get; } = new Base.BaseReportingDictionaryModel<DataSets.BaseDataSourceModel>();

		/// <summary>
		///		Informes
		/// </summary>
		public Base.BaseReportingDictionaryModel<Reports.ReportModel> Reports { get; } = new Base.BaseReportingDictionaryModel<Reports.ReportModel>();
	}
}
