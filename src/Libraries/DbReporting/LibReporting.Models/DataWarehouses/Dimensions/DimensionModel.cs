using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

/// <summary>
///		Clase con los datos de una dimensión
/// </summary>
public class DimensionModel : Base.BaseReportingModel
{
	public DimensionModel(DataWarehouseModel dataWarehouse, DataSets.BaseDataSourceModel dataSource)
	{
		DataWarehouse = dataWarehouse;
		DataSource = dataSource;
	}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is DimensionModel dimension)
			return Id.CompareTo(dimension.Id);
		else
			return -1;
	}

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
				if (relation.Dimension is not null && relation.Dimension.HasColumn(dimensionId, columnId))
					return true;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return false;
	}

	/// <summary>
	///		Obtiene una columna por su Id o por su alias
	/// </summary>
	public DataSets.DataSourceColumnModel? GetColumn(string columnId, bool compareWithAlias)
	{
		DataSets.DataSourceColumnModel? column = DataSource.GetColumn(columnId, compareWithAlias);

			// Si no ha encontrado la columna, busca en las dimensiones hija
			if (column is null)
				foreach (DimensionRelationModel relation in Relations)
					if (column is null && relation.Dimension is not null)
						column = relation.Dimension.GetColumn(columnId, compareWithAlias);
			// Devuelve la columna
			return column;
	}

	/// <summary>
	///		Descripción de la <see cref="DimensionModel"/>
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	///		Datawarehouse al que se asocia la dimensión
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }

	/// <summary>
	///		Origen de datos al que se asocia esta dimensión
	/// </summary>
	public DataSets.BaseDataSourceModel DataSource { get; }

	/// <summary>
	///		Relaciones de esta dimensión con su dimensión hijo
	/// </summary>
	public List<DimensionRelationModel> Relations { get; } = new();
}
