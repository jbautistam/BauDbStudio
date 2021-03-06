using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Application.Repositories
{
	/// <summary>
	///		Repositorio para <see cref="DataWarehouseModel"/>
	/// </summary>
	internal class DataWarehouseRepository
	{
		// Constantes privadas
		private const string TagRoot = "Dashboard";
		private const string TagName = "Name";
		private const string TagDescription = "Description";
		private const string TagId = "Id";
		private const string TagColumn = "Column";
		private const string TagVisible = "Visible";
		private const string TagSourceId = "SourceId";
		private const string TagDataSourceTable = "DataSourceTable";
		private const string TagDataSourceSql = "DataSourceSql";
		private const string TagSql = "Sql";
		private const string TagDimension = "Dimension";
		private const string TagReport = "Report";
		private const string TagDataSource = "DataSource";
		private const string TagPrimaryKey = "IsPrimaryKey";
		private const string TagRelation = "Relation";
		private const string TagForeignKey = "ForeignKey";
		private const string TagDimensionColumn="DimensionColumn";
		private const string TagSchema = "Schema";
		private const string TagTable = "Table";
		private const string TagType = "Type";
		private const string TagRequired = "Required";
		private const string TagParameter = "Parameter";
		private const string TagValue = "Value";

		/// <summary>
		///		Carga los datos de un <see cref="DataWarehouseModel"/>
		/// </summary>
		internal DataWarehouseModel Load(Models.ReportingSchemaModel schema, string fileName)
		{
			DataWarehouseModel dataWarehouse = new DataWarehouseModel(schema);

				// Carga los datos
				if (System.IO.File.Exists(fileName))
				{
					MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

						if (fileML != null)
							foreach (MLNode rootML in fileML.Nodes)
								if (rootML.Name == TagRoot)
								{
									// Asigna las propiedades
									dataWarehouse.Id = rootML.Attributes[TagId].Value.TrimIgnoreNull();
									dataWarehouse.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
									dataWarehouse.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
									// Carga las dimensiones y orígenes de datos
									foreach (MLNode nodeML in rootML.Nodes)
										switch (nodeML.Name)
										{
											case TagDataSourceTable:
													dataWarehouse.DataSources.Add(LoadDataSourceTable(dataWarehouse, nodeML));
												break;
											case TagDataSourceSql:
													dataWarehouse.DataSources.Add(LoadDataSourceSql(dataWarehouse, nodeML));
												break;
											case TagDimension:
													dataWarehouse.Dimensions.Add(LoadDimension(nodeML, dataWarehouse));
												break;
											case TagReport:
													dataWarehouse.Reports.Add(LoadReport(nodeML, dataWarehouse));
												break;
										}
								}
				}
				// Devuelve los datos del almacén de datos
				return dataWarehouse;
		}

		/// <summary>
		///		Obtiene un origen de datos para una tabla
		/// </summary>
		private DataSourceTableModel LoadDataSourceTable(DataWarehouseModel dataWarehouse, MLNode rootML)
		{
			DataSourceTableModel dataSource = new DataSourceTableModel(dataWarehouse);

				// Asigna el esquema y la tabla
				dataSource.Schema = rootML.Attributes[TagSchema].Value.TrimIgnoreNull();
				dataSource.Table = rootML.Attributes[TagTable].Value.TrimIgnoreNull();
				// El Id del origen de datos es el nombre completo de la tabla
				dataSource.Id = dataSource.FullName;
				// Carga las columnas
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagColumn)
						dataSource.Columns.Add(LoadColumn(dataSource, nodeML));
				// Devuelve el origen de datos
				return dataSource;
		}

		/// <summary>
		///		Obtiene un origen de datos para una consulta SQL
		/// </summary>
		private DataSourceSqlModel LoadDataSourceSql(DataWarehouseModel dataWarehouse, MLNode rootML)
		{
			DataSourceSqlModel dataSource = new DataSourceSqlModel(dataWarehouse);

				// Carga las propiedades
				dataSource.Id = rootML.Attributes[TagId].Value.TrimIgnoreNull();
				// Asigna la base de datos y la cadena SQL
				dataSource.Sql = rootML.Nodes[TagSql].Value.TrimIgnoreNull();
				// Carga las columnas
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagColumn)
						dataSource.Columns.Add(LoadColumn(dataSource, nodeML));
				// Carga los parámetros de consulta
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagParameter)
						dataSource.Parameters.Add(LoadParameter(nodeML));
				// Devuelve el origen de datos
				return dataSource;
		}

		/// <summary>
		///		Carga los datos de una columna
		/// </summary>
		private DataSourceColumnModel LoadColumn(BaseDataSourceModel dataSource, MLNode rootML)
		{
			DataSourceColumnModel column = new DataSourceColumnModel(dataSource);

				// Carga las propiedades
				column.Id = rootML.Attributes[TagSourceId].Value.TrimIgnoreNull();
				column.Type = rootML.Attributes[TagType].Value.GetEnum(DataSourceColumnModel.Fieldtype.Unknown);
				column.Required = rootML.Attributes[TagRequired].Value.GetBool();
				column.IsPrimaryKey = rootML.Attributes[TagPrimaryKey].Value.GetBool();
				column.Visible = rootML.Attributes[TagVisible].Value.GetBool(true);
				// Devuelve el objeto
				return column;
		}

		/// <summary>
		///		Carga los datos de un parámetros
		/// </summary>
		private DataSourceSqlParameterModel LoadParameter(MLNode rootML)
		{
			DataSourceSqlParameterModel parameter = new DataSourceSqlParameterModel();

				// Carga las propiedades
				parameter.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
				parameter.Type = rootML.Attributes[TagType].Value.GetEnum(DataSourceColumnModel.Fieldtype.Unknown);
				parameter.DefaultValue = rootML.Attributes[TagValue].Value.TrimIgnoreNull();
				// Devuelve el parámetro
				return parameter;
		}

		/// <summary>
		///		Carga los datos de una dimension
		/// </summary>
		private DimensionModel LoadDimension(MLNode rootML, DataWarehouseModel dataWarehouse)
		{
			DimensionModel dimension =  new DimensionModel(dataWarehouse);

				// Carga las propiedades básicas
				dimension.Id = rootML.Attributes[TagId].Value.TrimIgnoreNull();
				dimension.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
				// Carga los datos de la dimensión
				dimension.DataSource = dataWarehouse.DataSources[rootML.Attributes[TagSourceId].Value.TrimIgnoreNull()];
				// Carga las dimensiones hija
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagRelation)
						dimension.Relations.Add(LoadRelatedDimension(nodeML, dataWarehouse));
				// Devuelve la dimensión
				return dimension;
		}

		/// <summary>
		///		Carga los datos de una dimension relacionada
		/// </summary>
		private DimensionRelationModel LoadRelatedDimension(MLNode rootML, DataWarehouseModel dataWarehouse)
		{
			DimensionRelationModel relation = new DimensionRelationModel(dataWarehouse);

				// Carga los datos de la dimensión
				relation.DimensionId = rootML.Attributes[TagSourceId].Value.TrimIgnoreNull();
				// Carga las columnas
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagForeignKey)
						relation.ForeignKeys.Add(new RelationForeignKey
															{
																ColumnId = nodeML.Attributes[TagColumn].Value.TrimIgnoreNull(),
																TargetColumnId = nodeML.Attributes[TagDimensionColumn].Value.TrimIgnoreNull()
															}
												);
				// Devuelve la relación
				return relation;
		}

		/// <summary>
		///		Carga los datos de un <see cref="ReportModel"/>
		/// </summary>
		private ReportModel LoadReport(MLNode rootML, DataWarehouseModel dataWarehouse)
		{
			ReportModel report = new ReportModel(dataWarehouse);

				// Carga el informe
				report.Id = rootML.Attributes[TagId].Value.TrimIgnoreNull();
				report.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
				// Carga los datos
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagDataSource:
								report.ReportDataSources.Add(LoadReportDataSource(nodeML, report));
							break;
					}
				// Devuelve el objeto
				return report;
		}

		/// <summary>
		///		Carga un origen de datos de un informe
		/// </summary>
		private ReportDataSourceModel LoadReportDataSource(MLNode rootML, ReportModel report)
		{
			ReportDataSourceModel dataSource = new ReportDataSourceModel(report);

				// Asigna las propiedades
				dataSource.DataSource = report.DataWarehouse.DataSources[rootML.Attributes[TagSourceId].Value.TrimIgnoreNull()];
				// Carga las expresiones
				foreach (MLNode childML in rootML.Nodes)
					switch (childML.Name)
					{
						case TagRelation:
								dataSource.Relations.Add(LoadRelatedDimension(childML, report.DataWarehouse));
							break;
					}
				// Devuelve el origen de datos
				return dataSource;
		}

		/// <summary>
		///		Graba los datos de un <see cref="DataWarehouseModel"/>
		/// </summary>
		internal void Save(DataWarehouseModel dataWarehouse, string fileName)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Añade las propiedades básicas
				rootML.Attributes.Add(TagId, dataWarehouse.Id);
				rootML.Nodes.Add(TagName, dataWarehouse.Name);
				rootML.Nodes.Add(TagDescription, dataWarehouse.Description);
				// Añade los nodos de orígenes de datos
				foreach (BaseDataSourceModel baseDataSource in dataWarehouse.DataSources.EnumerateValues())
					switch (baseDataSource)
					{
						case DataSourceTableModel dataSource:
								rootML.Nodes.Add(GetNodeDataSourceTable(dataSource));
							break;
						case DataSourceSqlModel dataSource:
								rootML.Nodes.Add(GetNodeDataSourceSql(dataSource));
							break;
					}
				// Añade los nodos de dimensión
				foreach (DimensionModel dimension in dataWarehouse.Dimensions.EnumerateValues())
					rootML.Nodes.Add(GetNodeDimension(dimension));
				// Añade los informes
				foreach (ReportModel report in dataWarehouse.Reports.EnumerateValues())
					rootML.Nodes.Add(GetNodeReport(report));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Obtiene el nodo de un origen de datos
		/// </summary>
		private MLNode GetNodeDataSourceTable(DataSourceTableModel dataSource)
		{
			MLNode nodeML = new MLNode(TagDataSourceTable);

				// Asigna las propiedades
				nodeML.Attributes.Add(TagId, dataSource.FullName);
				nodeML.Attributes.Add(TagSchema, dataSource.Schema);
				nodeML.Attributes.Add(TagTable, dataSource.Table);
				// Añade las columnas visibles
				nodeML.Nodes.AddRange(GetNodesColumns(dataSource.Columns));
				// Devuelve el nodo
				return nodeML;
		}

		/// <summary>
		///		Obtiene el nodo de un origen de datos
		/// </summary>
		private MLNode GetNodeDataSourceSql(DataSourceSqlModel dataSource)
		{
			MLNode nodeML = new MLNode(TagDataSourceSql);

				// Asigna las propiedades
				nodeML.Attributes.Add(TagId, dataSource.Id);
				nodeML.Nodes.Add(TagSql, dataSource.Sql);
				// Añade las columnas y los parámetros
				nodeML.Nodes.AddRange(GetNodesColumns(dataSource.Columns));
				nodeML.Nodes.AddRange(GetNodesParameters(dataSource.Parameters));
				// Devuelve el nodo
				return nodeML;
		}

		/// <summary>
		///		Obtiene los nodos de una serie de parámetros
		/// </summary>
		private MLNodesCollection GetNodesParameters(System.Collections.Generic.List<DataSourceSqlParameterModel> parameters)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los parámetros
				foreach (DataSourceSqlParameterModel parameter in parameters)
				{
					MLNode nodeML = nodesML.Add(TagParameter);

						// Asigna los atributos
						nodeML.Attributes.Add(TagName, parameter.Name);
						nodeML.Attributes.Add(TagType, parameter.Type.ToString());
						nodeML.Attributes.Add(TagValue, parameter.DefaultValue);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los nodos de las columnas
		/// </summary>
		private MLNodesCollection GetNodesColumns(Models.Base.BaseReportingDictionaryModel<DataSourceColumnModel> columns)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los nodos
				foreach (DataSourceColumnModel column in columns.EnumerateValues())
				{
					MLNode nodeML = nodesML.Add(TagColumn);

						// Añade los datos
						nodeML.Attributes.Add(TagSourceId, column.Id);
						nodeML.Attributes.Add(TagPrimaryKey, column.IsPrimaryKey);
						nodeML.Attributes.Add(TagVisible, column.Visible);
						nodeML.Attributes.Add(TagType, column.Type.ToString());
						nodeML.Attributes.Add(TagRequired, column.Required);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene el nodo de una dimensión
		/// </summary>
		private MLNode GetNodeDimension(DimensionModel dimension)
		{
			MLNode nodeML = new MLNode(TagDimension);

				// Asigna las propiedades
				nodeML.Attributes.Add(TagId, dimension.Id);
				nodeML.Nodes.Add(TagDescription, dimension.Description);
				nodeML.Attributes.Add(TagSourceId, dimension.DataSource.Id);
				// Añade las relaciones hija
				foreach (DimensionRelationModel relation in dimension.Relations)
					if (relation.Dimension != null)
						nodeML.Nodes.Add(GetNodeRelation(relation));
				// Devuelve el nodo
				return nodeML;
		}

		/// <summary>
		///		Obtiene un nodo de relación
		/// </summary>
		private MLNode GetNodeRelation(DimensionRelationModel relation)
		{
			MLNode rootML = new MLNode(TagRelation);

				// Añade los atributos
				rootML.Attributes.Add(TagSourceId, relation.Dimension.Id);
				// Añade las claves foráneas
				foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
				{
					MLNode nodeML = rootML.Nodes.Add(TagForeignKey);

						nodeML.Attributes.Add(TagColumn, foreignKey.ColumnId);
						nodeML.Attributes.Add(TagDimensionColumn, foreignKey.TargetColumnId);
				}
				// Devuelve el nodo
				return rootML;
		}

		/// <summary>
		///		Obtiene el nodo de un <see cref="ReportModel"/>
		/// </summary>
		private MLNode GetNodeReport(ReportModel report)
		{
			MLNode rootML = new MLNode(TagReport);

				// Asigna las propiedades
				rootML.Attributes.Add(TagId, report.Id);
				rootML.Nodes.Add(TagDescription, report.Description);
				// Añade las expresiones
				foreach (ReportDataSourceModel dataSource in report.ReportDataSources)
				{
					MLNode nodeML = rootML.Nodes.Add(TagDataSource);

						// Asigna las propiedades
						nodeML.Attributes.Add(TagSourceId, dataSource.DataSource.Id);
						// Añade las relaciones
						foreach (DimensionRelationModel relation in dataSource.Relations)
							nodeML.Nodes.Add(GetNodeRelation(relation));
				}
				// Devuelve los datos del nodo
				return rootML;
		}
	}
}