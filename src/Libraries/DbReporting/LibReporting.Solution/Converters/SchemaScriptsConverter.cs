using System;
using System.Collections.Generic;

using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.Solution.Converters
{
	/// <summary>
	///		Conversor de un <see cref="DataWarehouseModel"/> en SQL
	/// </summary>
	public class SchemaScriptsConverter
	{	
		/// <summary>
		///		Convierte un <see cref="DataWarehouseModel"/> en SQL
		/// </summary>
		internal void Convert(DataWarehouseModel dataWarehouse, string scriptFileName)
		{
			// Crea las tablas
			CreateTables();
			// Inserta los orígenes de datos
			CreateDataSources(dataWarehouse);
			// Inserta las dimensiones
			CreateDimensions(dataWarehouse);
			// Inserta los informes
			CreateReports(dataWarehouse);
			// Graba el archivo
			LibHelper.Files.HelperFiles.SaveTextFile(scriptFileName, Builder.ToString());
		}

		/// <summary>
		///		Crea las tablas de esquema
		/// </summary>
		private void CreateTables()
		{
			// Tablas de orígenes de datos
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[DataSources];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[DataSources](
									[DataSourceId] [int] NOT NULL,
									[Schema] [varchar](50) NULL,
									[Table] [varchar](50) NOT NULL,
									[Sql] [varchar](max) NULL,
								 CONSTRAINT [PK_DataSources] PRIMARY KEY CLUSTERED 
								(
									[DataSourceId] ASC
								)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
								) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];");
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[DataSourceColumns];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[DataSourceColumns](
									[DataSourceColumnId] [int] NOT NULL,
									[DataSourceId] [int] NOT NULL,
									[Name] [varchar](50) NOT NULL,
									[Type] [int] NOT NULL,
									[IsPrimaryKey] [bit] NOT NULL,
									[Visible] [bit] NOT NULL,
									[Required] [bit] NOT NULL,
								 CONSTRAINT [PK_DataSourceColumns] PRIMARY KEY CLUSTERED 
								(
									[DataSourceColumnId] ASC
								)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
								) ON [PRIMARY];");
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[DataSourceParameters];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[DataSourceParameters](
									[DataSourceParameterId] [int] NOT NULL,
									[DataSourceId] [int] NOT NULL,
									[Name] [varchar](50) NOT NULL,
									[Type] [int] NOT NULL,
									[DefaultValue] [varchar](50) NULL,
								 CONSTRAINT [PK_DataSourceParameters] PRIMARY KEY CLUSTERED 
								(
									[DataSourceParameterId] ASC
								)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
								) ON [PRIMARY];");
			// Tablas de dimensiones
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[Dimensions];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[Dimensions](
										[DimensionId] [int] NOT NULL,
										[Name] [varchar](200) NOT NULL,
										[DataSourceId] [int] NOT NULL,
									 CONSTRAINT [PK_Dimensions] PRIMARY KEY CLUSTERED 
									(
										[DimensionId] ASC
									)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
									) ON [PRIMARY];");
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[DimensionRelations];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[DimensionRelations](
										[RelationId] [int] NOT NULL,
										[SourceDimensionId] [int] NOT NULL,
										[SourceColumnId] [int] NOT NULL,
										[TargetDimensionId] [int] NOT NULL,
										[TargetColumnId] [int] NOT NULL,
									 CONSTRAINT [PK_DimensionRelations] PRIMARY KEY CLUSTERED 
									(
										[RelationId] ASC
									)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
									) ON [PRIMARY];");
			// Tablas de informes
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[Reports];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[Reports](
									[ReportId] [int] NOT NULL,
									[Name] [varchar](200) NOT NULL,
									[Description] [varchar](2000) NOT NULL,
								 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
								(
									[ReportId] ASC
								)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
								) ON [PRIMARY];");
			Builder.AppendLine("DROP TABLE IF EXISTS [dbo].[ReportRelations];");
			Builder.AppendLine(@"CREATE TABLE [dbo].[ReportRelations](
									[RelationId] [int] NOT NULL,
									[ReportId] [int] NOT NULL,
									[SourceDataSourceId] [int] NOT NULL,
									[SourceColumnId] [int] NOT NULL,
									[TargetDimensionId] [int] NOT NULL,
									[TargetColumnId] [int] NOT NULL,
								 CONSTRAINT [PK_ReportRelations] PRIMARY KEY CLUSTERED 
								(
									[RelationId] ASC
								)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
								) ON [PRIMARY];");
		}

		/// <summary>
		///		Crea las líneas de origen de datos
		/// </summary>
		private void CreateDataSources(DataWarehouseModel dataWarehouse)
		{
			int dataSourceId = 1, columnId = 1, parameterId = 1;

				// Inserta los orígenes de datos
				foreach (BaseDataSourceModel dataSource in dataWarehouse.DataSources.EnumerateValues())
				{
					// Inserta los registros
					if (dataSource is DataSourceTableModel dataSourceTable)
						CreateDataSource(dataSourceId, dataSourceTable, ref columnId);
					else if (dataSource is DataSourceSqlModel dataSourceSql)
						CreateDataSource(dataSourceId, dataSourceSql, ref columnId, ref parameterId);
					// Incrementa el id
					dataSourceId++;
				}
		}

		/// <summary>
		///		Crea un origen de datos de tabla
		/// </summary>
		private void CreateDataSource(int dataSourceId, DataSourceTableModel dataSourceTable, ref int columnId)
		{
			// Inserta el origen de datos en la lista
			DataBaseIds.Add(("DataSource", dataSourceTable.FullName, dataSourceId, null));
			// Crea el registro de origen de datos tabla
			Builder.AppendLine($@"INSERT INTO DataSources (DataSourceId, [Schema], [Table], Sql)
									VALUES ({dataSourceId}, {Convert(dataSourceTable.Schema)}, {Convert(dataSourceTable.Table)}, NULL);");
			// Crea las columnas
			CreateDataSourceColumns(dataSourceId, dataSourceTable.Columns, ref columnId);
		}

		/// <summary>
		///		Crea un origen de datos de SQL
		/// </summary>
		private void CreateDataSource(int dataSourceId, DataSourceSqlModel dataSourceSql, ref int columnId, ref int parameterId)
		{
			// Inserta el origen de datos en la lista
			DataBaseIds.Add(("DataSource", dataSourceSql.Id, dataSourceId, null));
			// Crea el registro de origen de datos SQL
			Builder.AppendLine($@"INSERT INTO DataSources (DataSourceId, [Schema], [Table], Sql)
									VALUES ({dataSourceId}, NULL, {Convert(dataSourceSql.Id)}, {Convert(dataSourceSql.Sql)});");
			// Crea las columnas
			CreateDataSourceColumns(dataSourceId, dataSourceSql.Columns, ref columnId);
			// Crea los parámetros
			CreateDataSourceParameters(dataSourceId, dataSourceSql.Parameters, ref parameterId);
		}

		/// <summary>
		///		Crea los registros de columnas
		/// </summary>
		private void CreateDataSourceColumns(int dataSourceId, BaseReportingDictionaryModel<DataSourceColumnModel> columns, ref int columnId)
		{
			foreach (DataSourceColumnModel column in columns.EnumerateValues())
			{
				// Inserta el origen de datos en la lista de Ids
				DataBaseIds.Add(("Column", column.Id, columnId, column.DataSource.Id));
				// Añade el registro
				Builder.AppendLine($@"INSERT INTO DataSourceColumns (DataSourceColumnId, DataSourceId, Name, Type, IsPrimaryKey, Visible, Required)
										VALUES ({columnId++}, {dataSourceId}, {Convert(column.Id)}, {(int) column.Type}, {Convert(column.IsPrimaryKey)},
												{Convert(column.Visible)}, {Convert(column.Required)});");
			}
		}

		/// <summary>
		///		Crea los registros de parámetros
		/// </summary>
		private void CreateDataSourceParameters(int dataSourceId, List<DataSourceSqlParameterModel> parameters, ref int parameterId)
		{
			foreach (DataSourceSqlParameterModel parameter in parameters)
				Builder.AppendLine($@"INSERT INTO DataSourceParameters (DataSourceParameterId, DataSourceId, Name, Type, DefaultValue)
										VALUES ({parameterId++}, {dataSourceId}, {Convert(parameter.Name)}, 
												{(int) parameter.Type}, {Convert(parameter.DefaultValue)});");
		}

		/// <summary>
		///		Crea los registros de dimensiones
		/// </summary>
		private void CreateDimensions(DataWarehouseModel dataWarehouse)
		{
			int dimensionId = 1, relationId = 1;

				// Crea los registros
				foreach (DimensionModel dimension in dataWarehouse.Dimensions.EnumerateValues())
				{
					// Guarda el Id
					DataBaseIds.Add(("Dimension", dimension.Id, dimensionId, null));
					// Inserta el registro
					Builder.AppendLine($@"INSERT INTO Dimensions (DimensionId, Name, DataSourceId)
											VALUES ({dimensionId}, {Convert(dimension.Id)}, {GetDataBaseId("DataSource", dimension.DataSource.Id)});");
					// Incrementa el Id
					dimensionId++;
				}
				// Inserta las relaciones
				foreach (DimensionModel dimension in dataWarehouse.Dimensions.EnumerateValues())
				{
					int sourceDimensionId = GetDataBaseId("Dimension", dimension.Id);

						foreach (DimensionRelationModel relation in dimension.Relations)
						{
							int targetDimensionId = GetDataBaseId("Dimension", relation.Dimension.Id);

								// Graba las tablas foráneas
								foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
								{
									// Inserta el registro
									Builder.AppendLine($@"INSERT INTO DimensionRelations (RelationId, SourceDimensionId, SourceColumnId, TargetDimensionId, TargetColumnId)
															VALUES ({relationId}, {sourceDimensionId}, {GetDataBaseId("Column", foreignKey.ColumnId, dimension.DataSource.Id)}, 
																	{targetDimensionId}, {GetDataBaseId("Column", foreignKey.TargetColumnId, relation.Dimension.DataSource.Id)});");
									// Incrementa el Id
									relationId++;
								}
						}
				}
		}

		/// <summary>
		///		Crea los registros de informes
		/// </summary>
		private void CreateReports(DataWarehouseModel dataWarehouse)
		{
			int reportId = 1, relationId = 1;

				// Crea los registros
				foreach (ReportModel report in dataWarehouse.Reports.EnumerateValues())
				{
					// Guarda el Id
					DataBaseIds.Add(("Report", report.Id, reportId, null));
					// Inserta el registro
					Builder.AppendLine($@"INSERT INTO Reports (ReportId, Name, Description)
											VALUES ({reportId}, {Convert(report.Id)}, {Convert(report.Description)});");
					// Incrementa el Id
					reportId++;
				}
				// Inserta las relaciones
				foreach (ReportModel report in dataWarehouse.Reports.EnumerateValues())
				{
					// Obtiene el Id del informe
					reportId = GetDataBaseId("Report", report.Id);
					// Guarda las relaciones del informe
					foreach (ReportDataSourceModel reportDataSource in report.ReportDataSources)
					{
						int dataSourceId = GetDataBaseId("DataSource", reportDataSource.DataSource.Id);

							foreach (DimensionRelationModel dimensionRelation in reportDataSource.Relations)
							{
								int targetDimensionId = GetDataBaseId("Dimension", dimensionRelation.Dimension.Id);

									// Graba las tablas foráneas
									foreach (RelationForeignKey foreignKey in dimensionRelation.ForeignKeys)
									{
										// Inserta el registro
										Builder.AppendLine($@"INSERT INTO ReportRelations (RelationId, ReportId, SourceDataSourceId, SourceColumnId, TargetDimensionId, TargetColumnId)
																VALUES ({relationId}, {reportId}, {dataSourceId}, 
																		{GetDataBaseId("Column", foreignKey.ColumnId, reportDataSource.DataSource.Id)}, 
																		{targetDimensionId}, 
																		{GetDataBaseId("Column", foreignKey.TargetColumnId, dimensionRelation.Dimension.DataSource.Id)});");
										// Incrementa el Id
										relationId++;
									}
							}
					}
				}
		}

		/// <summary>
		///		Obtiene un Id de base de datos para un tipo
		/// </summary>
		private int GetDataBaseId(string type, string name)
		{
			// Busca el Id de la base de datos
			foreach ((string type, string name, int id, string parentId) dataBaseId in DataBaseIds)
				if (dataBaseId.type.Equals(type, StringComparison.CurrentCultureIgnoreCase) && dataBaseId.name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
					return dataBaseId.id;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return -1;
		}

		/// <summary>
		///		Obtiene un Id de base de datos para un tipo teniendo en cuenta el Id del elemento padre (por ejemplo, en caso de una columna, el Id de la tabla)
		/// </summary>
		private int GetDataBaseId(string type, string name, string parentId)
		{
			// Busca el Id de la base de datos
			foreach ((string type, string name, int id, string parentId) dataBaseId in DataBaseIds)
				if (dataBaseId.type.Equals(type, StringComparison.CurrentCultureIgnoreCase) && dataBaseId.name.Equals(name, StringComparison.CurrentCultureIgnoreCase) &&
						dataBaseId.parentId.Equals(parentId, StringComparison.CurrentCultureIgnoreCase))
					return dataBaseId.id;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return -1;
		}

		/// <summary>
		///		Convierte una cadena en SQL
		/// </summary>
		private string Convert(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return "''";
			else
				return $"'{value.Replace("'", "''")}'";
		}

		/// <summary>
		///		Convierte un valor lógico a SQL
		/// </summary>
		private string Convert(bool value)
		{
			if (value)
				return "1";
			else
				return "0";
		}

		/// <summary>
		///		Generador de SQL
		/// </summary>
		private System.Text.StringBuilder Builder { get; } = new System.Text.StringBuilder();

		/// <summary>
		///		Lista de Ids en la base de datos
		/// </summary>
		private List<(string type, string name, int dataBaseId, string parentId)> DataBaseIds { get; } = new List<(string type, string name, int dataBaseId, string parentId)>();
	}
}