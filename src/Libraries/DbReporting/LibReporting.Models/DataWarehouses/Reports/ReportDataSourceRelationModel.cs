using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports
{
	/// <summary>
	///		Relación de un origen de datos con una dimensión
	/// </summary>
	public class ReportDataSourceRelationModel
	{
		public ReportDataSourceRelationModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		DataWarehouse al que pertenece la dimensión
		/// </summary>
		public DataWarehouseModel DataWarehouse { get; }

		/// <summary>
		///		Código de la dimensión relacionada
		/// </summary>
		public string DimensionId { get; set; }

		/// <summary>
		///		Dimensión con la que se establece esta relación
		/// </summary>
		public Dimensions.DimensionModel Dimension 
		{ 
			get { return DataWarehouse.Dimensions.Search(DimensionId); }
		}

		/// <summary>
		///		Columnas de la relación
		/// </summary>
		public List<Relations.RelationForeignKey> ForeignKeys { get; } = new List<Relations.RelationForeignKey>();
	}
}
