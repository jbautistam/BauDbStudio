using System;

using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Solution.Converters
{
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
			DataWarehouseModel dataWarehouse = new DataWarehouseModel(schema);

				// Asigna las propiedades
				dataWarehouse.Name = name;
				// Genera los orígenes de datos de las tablas
				foreach (TableDbModel table in schemaDb.Tables)
					dataWarehouse.DataSources.Add(ConvertDataSource(dataWarehouse, table));
				// Genera los orígenes de datos de las vistas
				foreach (ViewDbModel view in schemaDb.Views)
					dataWarehouse.DataSources.Add(ConvertDataSource(dataWarehouse, view));
				// Devuelve el objeto
				return dataWarehouse;
		}

		/// <summary>
		///		Convierte una tabla de base de datos en un esquema
		/// </summary>
		private DataSourceTableModel ConvertDataSource(DataWarehouseModel dataWarehouse, BaseTableDbModel table)
		{
			DataSourceTableModel dataSource = new DataSourceTableModel(dataWarehouse);

				// Asigna los datos
				dataSource.Schema = table.Schema;
				dataSource.Table = table.Name;
				// Asigna las columnas
				foreach (FieldDbModel field in table.Fields)
				{
					DataSourceColumnModel column = new DataSourceColumnModel(dataSource);

						// Asigna las propiedades
						column.Id = field.Name;
						column.IsPrimaryKey = field.IsKey;
						column.Type = Convert(field.Type);
						column.Required = field.IsRequired;
						// Añade la columna
						dataSource.Columns.Add(column);
				}
				// Devuelve el origen de datos
				return dataSource;
		}

		/// <summary>
		///		Convierte el tipo
		/// </summary>
		private DataSourceColumnModel.FieldType Convert(FieldDbModel.Fieldtype type)
		{
			switch (type)
			{
				case FieldDbModel.Fieldtype.Binary:
					return DataSourceColumnModel.FieldType.Binary;
				case FieldDbModel.Fieldtype.Boolean:
					return DataSourceColumnModel.FieldType.Boolean;
				case FieldDbModel.Fieldtype.Date:
					return DataSourceColumnModel.FieldType.Date;
				case FieldDbModel.Fieldtype.Decimal:
					return DataSourceColumnModel.FieldType.Decimal;
				case FieldDbModel.Fieldtype.Integer:
					return DataSourceColumnModel.FieldType.Integer;
				case FieldDbModel.Fieldtype.String:
					return DataSourceColumnModel.FieldType.String;
				default:
					return DataSourceColumnModel.FieldType.Unknown;
			}
		}
	}
}