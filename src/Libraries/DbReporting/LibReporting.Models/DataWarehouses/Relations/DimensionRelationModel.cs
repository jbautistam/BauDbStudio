using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Relations
{
	/// <summary>
	///		Claves de una relación con una dimensión
	/// </summary>
	public class DimensionRelationModel
	{
		public DimensionRelationModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		Datawarehouse al que pertenece la dimensión
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
			get { return DataWarehouse.Dimensions[DimensionId]; }
		}

		/// <summary>
		///		Campos de la relación
		/// </summary>
		public List<RelationForeignKey> ForeignKeys { get; } = new List<RelationForeignKey>();
	}
}
