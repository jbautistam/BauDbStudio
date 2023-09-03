using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Solution.Converters;

/// <summary>
///		Conversor de un esquema de base de datos a un <see cref="DataWarehouseModel"/>
/// </summary>
public class SchemaConverter
{
	/// <summary>
	///		Convierte un archivo de esquema de base de datos en un <see cref="DataWarehouseModel"/>
	/// </summary>
	internal DataWarehouseModel Convert(ReportingSchemaModel schema, string name, string fileName)
	{
		SchemaDbModel schemaDb = new LibDbSchema.Repository.Xml.SchemaXmlManager().Load(fileName);

			if (schemaDb.Tables.Count == 0)
				throw new Exception($"There is not tables defined at file {fileName}");
			else
			{
				DataWarehouseModel dataWarehouse = new(schema);

					// Asigna las propiedades
					dataWarehouse.Name = name;
					// Genera los orígenes de datos de las tablas
					foreach (TableDbModel table in schemaDb.Tables)
						dataWarehouse.DataSources.Add(ConvertDataSource(dataWarehouse, table, false));
					// Genera los orígenes de datos de las vistas
					foreach (ViewDbModel view in schemaDb.Views)
						dataWarehouse.DataSources.Add(ConvertDataSource(dataWarehouse, view, true));
					// Devuelve el objeto
					return dataWarehouse;
			}
	}

	/// <summary>
	///		Combina un <see cref="DataWarehouseModel"/> con un esquema de base de datos
	/// </summary>
	internal void Merge(DataWarehouseModel target, string fileName)
	{
		DataWarehouseModel source = Convert(target.Schema, target.Name, fileName);

			// Combina los orígenes de datos de las tablas
			MergeDataSources(source, target);
			// Borra los orígenes de datos que estén en el destino pero no en el origen
			DropDataSources(source, target);
	}

	/// <summary>
	///		Combina los orígenes de datos
	/// </summary>
	private void MergeDataSources(DataWarehouseModel source, DataWarehouseModel target)
	{
		foreach (BaseDataSourceModel dataSource in source.DataSources.EnumerateValues())
			if (dataSource is DataSourceTableModel sourceTable)
			{
				DataSourceTableModel? targetTable = target.GetDataTableByFullName(sourceTable.FullName);

					if (targetTable is null)
						target.DataSources.Add(sourceTable.Clone(target));
					else
					{
						// Mezcla las propiedades
						targetTable.IsView = sourceTable.IsView;
						// Mezcla las columnas
						MergeColumns(sourceTable, targetTable);
						// Elimina las columnas que estén en el destino pero ya no estén en el origen
						DropColumns(sourceTable, targetTable);
					}
			}
	}

	/// <summary>
	///		Elimina los orígenes de datos que estén en el destino pero no en el origen
	/// </summary>
	private void DropDataSources(DataWarehouseModel source, DataWarehouseModel target)
	{
		List<DataSourceTableModel> tablesToDelete = new();

			// Busca las tablas a eliminar
			foreach (BaseDataSourceModel dataSource in target.DataSources.EnumerateValues())
				if (dataSource is DataSourceTableModel targetTable && source.GetDataTableByFullName(targetTable.FullName) is null)
					tablesToDelete.Add(targetTable);
			// Elimina las tablas encontradas
			foreach (DataSourceTableModel table in tablesToDelete)
				target.DataSources.Remove(table);
	}

	/// <summary>
	///		Combina las columnas de dos orígenes de datos
	/// </summary>
	private void MergeColumns(DataSourceTableModel source, DataSourceTableModel target)
	{
		foreach (DataSourceColumnModel sourceColumn in source.Columns.EnumerateValues())
		{
			DataSourceColumnModel? targetColumn = GetColumn(target, sourceColumn.Id);

				if (targetColumn is null)
					target.Columns.Add(sourceColumn.Clone(target));
				else
				{
					// Cambia los datos que pueden haber cambiado con el esquema
					targetColumn.IsPrimaryKey = sourceColumn.IsPrimaryKey;
					targetColumn.Type = sourceColumn.Type;
				}
		}
	}

	/// <summary>
	///		Elimina las columnas que estén en el destino pero no en el origen
	/// </summary>
	private void DropColumns(DataSourceTableModel source, DataSourceTableModel target)
	{
		List<DataSourceColumnModel> columnsToDelete = new();

			// Busca las columnas en el destino a eliminar
			foreach (DataSourceColumnModel targetColumn in target.Columns.EnumerateValues())
				if (GetColumn(source, targetColumn.Id) is null)
					columnsToDelete.Add(targetColumn);
			// Borra las columnas del destino
			foreach (DataSourceColumnModel column in columnsToDelete)
				target.Columns.Remove(column);
	}

	/// <summary>
	///		Obtiene un <see cref="DataSourceColumnModel"/> de un <see cref="DataSourceTableModel"/>
	/// </summary>
	private DataSourceColumnModel? GetColumn(DataSourceTableModel dataSource, string id)
	{
		// Busca la columna en el origen de datos
		foreach (DataSourceColumnModel column in dataSource.Columns.EnumerateValues())
			if (column.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase))
				return column;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Convierte una tabla de base de datos en un esquema
	/// </summary>
	private DataSourceTableModel ConvertDataSource(DataWarehouseModel dataWarehouse, BaseTableDbModel table, bool isView)
	{
		DataSourceTableModel dataSource = new(dataWarehouse);

			// Asigna los datos
			dataSource.Schema = table.Schema ?? string.Empty;
			dataSource.Table = table.Name ?? string.Empty;
			dataSource.IsView = isView;
			// Asigna las columnas
			foreach (FieldDbModel field in table.Fields)
				dataSource.Columns.Add(Convert(dataSource, field));
			// Devuelve el origen de datos
			return dataSource;
	}

	/// <summary>
	///		Convierte la columna
	/// </summary>
	private DataSourceColumnModel Convert(DataSourceTableModel dataSource, FieldDbModel field)
	{
		DataSourceColumnModel column = new(dataSource);

			// Asigna las propiedades
			column.Id = field.Name ?? string.Empty;
			column.IsPrimaryKey = field.IsKey;
			column.Type = Convert(field.Type);
			column.Required = field.IsRequired;
			// Devuelve la columna
			return column;
	}

	/// <summary>
	///		Convierte el tipo
	/// </summary>
	private DataSourceColumnModel.FieldType Convert(FieldDbModel.Fieldtype type)
	{
		return type switch
				{
					FieldDbModel.Fieldtype.Binary => DataSourceColumnModel.FieldType.Binary,
					FieldDbModel.Fieldtype.Boolean => DataSourceColumnModel.FieldType.Boolean,
					FieldDbModel.Fieldtype.Date => DataSourceColumnModel.FieldType.Date,
					FieldDbModel.Fieldtype.Decimal => DataSourceColumnModel.FieldType.Decimal,
					FieldDbModel.Fieldtype.Integer => DataSourceColumnModel.FieldType.Integer,
					FieldDbModel.Fieldtype.String => DataSourceColumnModel.FieldType.String,
					_ => DataSourceColumnModel.FieldType.Unknown,
				};
	}
}