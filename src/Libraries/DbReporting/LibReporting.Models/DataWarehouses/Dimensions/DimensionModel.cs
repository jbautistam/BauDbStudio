using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions
{
	/// <summary>
	///		Clase con los datos de una dimensión
	/// </summary>
	public class DimensionModel : Base.BaseReportingModel
	{
		public DimensionModel(DataWarehouseModel dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		/// <summary>
		///		Nombre de la <see cref="DimensionModel"/>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Descripción de la <see cref="DimensionModel"/>
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		Comprueba si una columna está en esta dimensión o en alguna de sus hijas
		/// </summary>
		public bool HasColumn(string dimensionId, string columnId)
		{
			// Si la columna está en esta dimensión ...
			if (Id.Equals(dimensionId, StringComparison.CurrentCultureIgnoreCase) && DataSource.HasColumn(columnId))
				return true;
			else // ... si la columna está en alguna dimensión de la jerarquía
				foreach (DimensionRelationModel relation in Relations)
					if (relation.Dimension.HasColumn(dimensionId, columnId))
						return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Datawarehouse al que se asocia la dimensión
		/// </summary>
		public DataWarehouseModel DataWarehouse { get; }

		/// <summary>
		///		Origen de datos al que se asocia esta dimensión
		/// </summary>
		public DataSets.BaseDataSourceModel DataSource { get; set; }

		/// <summary>
		///		Relaciones de esta dimensión con su dimensión hijo
		/// </summary>
		public System.Collections.Generic.List<DimensionRelationModel> Relations { get; } = new System.Collections.Generic.List<DimensionRelationModel>();
	}
}
