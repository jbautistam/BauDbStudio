using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Solution.Repositories
{
	/// <summary>
	///		Repositorio para los informes avanzados
	/// </summary>
	internal class ReportRepository
	{
		// Constantes privadas
		private const string TagRoot = "Report";
		private const string TagDataWarehouse = "DataWarehouse";
		private const string TagName = "Name";
		private const string TagDescription = "Description";
		private const string TagParameter = "Parameter";
		private const string TagType = "Type";
		private const string TagDefault = "Default";
		private const string TagBlocks = "Blocks";
		private const string TagCte = "Cte";
		private const string TagExecute = "Execute";
		private const string TagQuery = "Query";
		private const string TagWith = "With";
		private const string TagDimension = "Dimension";
		private const string TagIfRequest = "IfRequest";
		private const string TagThen = "Then";
		private const string TagElse = "Else";
		private const string TagJoin = "Join";
		private const string TagRelation = "Relation";
		private const string TagFromDimension = "FromDimension";
		private const string TagRelatedTo = "RelatedTo";
		private const string TagTable = "Table";
		private const string TagField = "Field";
		private const string TagAlias = "Alias";
		private const string TagRequired = "Required";
		private const string TagFilter = "Filter";
		private const string TagRequests = "Requests";
		private const string TagExpression = "Expression";

		/// <summary>
		///		Carga el informe de un archivo sobre un <see cref="DataWarehouseModel"/>
		/// </summary>
		internal void Load(DataWarehouseModel dataWarehouse, string fileName)
		{
			ReportAdvancedModel report = LoadReport(dataWarehouse, fileName);

				// Si el informe cargado se corresponde con el DataWarehouse solicitado, se añade
				if (report is not null && !string.IsNullOrWhiteSpace(report.DataWarehouseKey) && 
						dataWarehouse.Name.Equals(report.DataWarehouseKey, StringComparison.CurrentCultureIgnoreCase))
					dataWarehouse.Reports.Add(report);
		}

		/// <summary>
		///		Carga el informe de un archivo sobre un <see cref="DataWarehouseModel"/>
		/// </summary>
		internal ReportAdvancedModel LoadReport(DataWarehouseModel dataWarehouse, string fileName)
		{
			ReportAdvancedModel report = new(dataWarehouse, fileName);
			MLFile fileML = Load(fileName);

				// Carga los datos del informe
				if (fileML is not null)
					foreach (MLNode rootML in fileML.Nodes)
						if (rootML.Name == TagRoot)
							foreach (MLNode nodeML in rootML.Nodes)
								switch (nodeML.Name)
								{
									case TagDataWarehouse:
											report.DataWarehouseKey = nodeML.Attributes[TagName].Value.TrimIgnoreNull();
										break;
									case TagName:
											report.Id = nodeML.Value.TrimIgnoreNull();
										break;
									case TagDescription:
											report.Description = nodeML.Value.TrimIgnoreNull();
										break;
									case TagParameter:
											report.Parameters.Add(LoadParameter(nodeML));
										break;
									case TagBlocks:
											report.Blocks.AddRange(LoadBlocks(nodeML.Nodes));
										break;
									case TagDimension:
											report.DimensionKeys.Add(nodeML.Attributes[TagName].Value.TrimIgnoreNull());
										break;
									case TagRequests:
											report.RequestDimensions.AddRange(LoadRequests(nodeML));
										break;
									case TagExpression:
											report.Expressions.AddRange(LoadExpressions(nodeML));
										break;
								}
				// Devuelve el informe
				return report;
		}

		/// <summary>
		///		Carga los datos de un parámeto de un nodo
		/// </summary>
		private ReportParameterModel LoadParameter(MLNode rootML)
		{
			return new ReportParameterModel
							{
								Key = rootML.Attributes[TagName].Value.TrimIgnoreNull(),
								Type = rootML.Attributes[TagType].Value.GetEnum(LibReporting.Models.DataWarehouses.DataSets.DataSourceColumnModel.FieldType.Unknown),
								DefaultValue = rootML.Attributes[TagDefault].Value.TrimIgnoreNull()
							};
		}

		/// <summary>
		///		Carga una serie de bloques
		/// </summary>
		private List<BaseBlockModel> LoadBlocks(MLNodesCollection nodesML)
		{
			List<BaseBlockModel> blocks = new();

				// Carga los bloques de los nodos
				foreach (MLNode nodeML in nodesML)
					switch (nodeML.Name)
					{
						case TagWith:
								blocks.Add(LoadBlockWith(nodeML));
							break;
						case TagCte:
								blocks.Add(LoadBlockCte(nodeML));
							break;
						case TagExecute:
								blocks.Add(LoadBlockExecute(nodeML));
							break;
						case TagQuery:
								blocks.Add(LoadBlockQuery(nodeML));
							break;
						case TagIfRequest:
								blocks.Add(LoadBlockIfRequest(nodeML));
							break;
					}
				// Devuelve la colección de bloques
				return blocks;
		}

		/// <summary>
		///		Carga un bloque de creación de una sentencia WITH de SQL
		/// </summary>
		private BaseBlockModel LoadBlockWith(MLNode rootML)
		{
			BlockWithModel with = new(rootML.Attributes[TagName].Value.TrimIgnoreNull());

				// Carga los nodos
				with.Blocks.AddRange(LoadBlocks(rootML.Nodes));
				// Devuelve la instrucción
				return with;
		}

		/// <summary>
		///		Carga un bloque de consulta
		/// </summary>
		private BlockQueryModel LoadBlockQuery(MLNode rootML)
		{
			BlockQueryModel query = new(rootML.Attributes[TagName].Value.TrimIgnoreNull());

				// Carga la consulta
				query.Sql = rootML.Value.TrimIgnoreNull();
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Carga un bloque de ejecución
		/// </summary>
		private BlockExecutionModel LoadBlockExecute(MLNode rootML)
		{
			return new BlockExecutionModel(rootML.Attributes[TagName].Value.TrimIgnoreNull())
							{
								Sql = rootML.Value
							};
		}

		/// <summary>
		///		Carga un bloque de Cte
		/// </summary>
		private BaseBlockModel LoadBlockCte(MLNode rootML)
		{
			if (!string.IsNullOrWhiteSpace(rootML.Attributes[TagDimension].Value.TrimIgnoreNull()))
				return LoadBlockCteDimension(rootML);
			else
			{
				BlockCreateCteModel block = new(rootML.Attributes[TagName].Value.TrimIgnoreNull());

					// Carga los datos de la consulta
					if (rootML.Nodes.Count == 0)
						block.Blocks.Add(LoadBlockQuery(rootML));
					else
						block.Blocks.AddRange(LoadBlocks(rootML.Nodes));
					// Devuelve los datos del bloque
					return block;
			}
		}

		/// <summary>
		///		Carga un bloque de creación de CTE para una dimensión
		/// </summary>
		private BaseBlockModel LoadBlockCteDimension(MLNode rootML)
		{
			BlockCreateCteDimensionModel block = new(rootML.Attributes[TagName].Value.TrimIgnoreNull())
													{ 
														DimensionKey = rootML.Attributes[TagDimension].Value.TrimIgnoreNull()
													};

				// Carga los objetos asociados
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagJoin:
								block.Joins.Add(LoadJoins(nodeML));
							break;
						case TagField:
								block.Fields.Add(LoadField(nodeML));
							break;
						case TagFilter:
								block.Filters.Add(LoadFilter(nodeML));
							break;
					}
				// Devuelve el bloque
				return block;
		}

		/// <summary>
		///		Carga un campo adicional para una consulta
		/// </summary>
		private ClauseFieldModel LoadField(MLNode rootML)
		{
			return new ClauseFieldModel
								{
									Field =	rootML.Attributes[TagName].Value.TrimIgnoreNull(),
									Alias = rootML.Attributes[TagAlias].Value.TrimIgnoreNull()
								};
		}

		/// <summary>
		///		Carga un filtro adicional para una consulta
		/// </summary>
		private ClauseFilterModel LoadFilter(MLNode rootML)
		{
			return new ClauseFilterModel
								{
									Sql = rootML.Value.TrimIgnoreNull()
								};
		}

		/// <summary>
		///		Carga los datos de una cláusula JOIN
		/// </summary>
		private ClauseJoinModel LoadJoins(MLNode rootML)
		{
			ClauseJoinModel join = new()
									{
										Type = rootML.Attributes[TagType].Value.GetEnum(ClauseJoinModel.JoinType.InnerJoin),
										DimensionKey = rootML.Attributes[TagDimension].Value.TrimIgnoreNull(),
										TableRelated = rootML.Attributes[TagTable].Value.TrimIgnoreNull(),
										Required = rootML.Attributes[TagRequired].Value.GetBool()
									};

				// Añade las dimensiones asociadas
				join.AddRelatedRequestDimensionKeys(rootML.Attributes[TagIfRequest].Value.TrimIgnoreNull());
				// Asigna las relaciones
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagRelation)
						join.Relations.Add((nodeML.Attributes[TagFromDimension].Value.TrimIgnoreNull(),
											nodeML.Attributes[TagRelatedTo].Value.TrimIgnoreNull()
											));
				// Devuelve la cláusula
				return join;
		}

		/// <summary>
		///		Carga un bloque que comprueba si se ha solicitado una (o varias) dimensiones
		/// </summary>
		private BlockIfRequest LoadBlockIfRequest(MLNode rootML)
		{
			BlockIfRequest block = new BlockIfRequest(string.Empty);

				// Añade las dimensiones y las expresiones que se comprueban
				block.AddDimensions(rootML.Attributes[TagDimension].Value.TrimIgnoreNull());
				block.AddExpressions(rootML.Attributes[TagExpression].Value.TrimIgnoreNull());
				// Carga los bloques de las cláusulas then y else (si no hay nodo Then, todo el contenido pertenece al bloque then)
				if (rootML.Nodes.Search(TagThen) is null)
					block.Then.AddRange(LoadBlocks(rootML.Nodes));
				else
					foreach (MLNode nodeML in rootML.Nodes)
						switch (nodeML.Name)
						{
							case TagThen:
									block.Then.AddRange(LoadBlocks(nodeML.Nodes));
								break;
							case TagElse:
									block.Else.AddRange(LoadBlocks(nodeML.Nodes));
								break;
						}
				// Devuelve el bloque
				return block;
		}

		/// <summary>
		///		Carga la información adicional sobre solicitudes para el informe
		/// </summary>
		private List<ReportAdvancedRequestDimension> LoadRequests(MLNode rootML)
		{
			List<ReportAdvancedRequestDimension> requests = new();

				// Carga las dimensiones solicitadas
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagDimension)
					{
						ReportAdvancedRequestDimension dimension = new()
																		{
																			DimensionKey = nodeML.Attributes[TagName].Value.TrimIgnoreNull(),
																			Required = nodeML.Attributes[TagRequired].Value.TrimIgnoreNull().GetBool()
																		};

							// Añade los conjuntos de campos
							foreach (MLNode childML in nodeML.Nodes)
								if (childML.Name == TagField && !string.IsNullOrWhiteSpace(childML.Attributes[TagName].Value.TrimIgnoreNull()))
									foreach (string field in childML.Attributes[TagName].Value.TrimIgnoreNull().Split(';'))
										if (!string.IsNullOrWhiteSpace(field))
											dimension.Fields.Add(new ReportAdvancedRequestDimensionField
																				{
																					Field = field.TrimIgnoreNull()
																				}
																);
							// Añade los datos de la dimensión
							requests.Add(dimension);
					}
				// Devuelve los datos de la solicitud
				return requests;
		}

		/// <summary>
		///		Carga la lista de expresiones
		/// </summary>
		private List<string> LoadExpressions(MLNode nodeML)
		{
			List<string> expressions = new();
			string fields = nodeML.Attributes[TagName].Value.TrimIgnoreNull();

				// Carga los campos en la lista
				if (!string.IsNullOrWhiteSpace(fields))
					foreach (string field in fields.Split(';'))
						if (!string.IsNullOrWhiteSpace(field))
							expressions.Add(field.TrimIgnoreNull());
				// Devuelve la lista de expresiones
				return expressions;
		}

		/// <summary>
		///		Carga un archivo controlando las excepciones
		/// </summary>
		private MLFile Load(string fileName)
		{
			try
			{
				return new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Error when load {fileName}. {exception.Message}");
				return null;
			}
		}
	}
}
