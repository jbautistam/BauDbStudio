using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Application.Exceptions;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
    /// <summary>
    ///		Generador de consultas SQL para un informe
    /// </summary>
    internal class ReportQueryAdvancedGenerator : ReportBaseQueryGenerator
	{
		internal ReportQueryAdvancedGenerator(ReportingSchemaModel schema, ReportAdvancedModel report, ReportRequestModel request) 
				: base(schema, report, request)
		{
		}

		/// <summary>
		///		Obtiene la cadena SQL de un informe para responder a una solicitud
		/// </summary>
		internal override string GetSql()
		{
			if (Report is not ReportAdvancedModel reportAdvanced)
				throw new ReportingParserException($"Unknown report  {Report.Id}. {Report.GetType().ToString()}");
			else
			{
				// Normaliza la solicitud
				NormalizeRequest(Request, reportAdvanced);
				// Devuelve la SQL generada
				return GetSql(GetQueries(reportAdvanced.Blocks));
			}
		}

		/// <summary>
		///		Normaliza la solicitud
		/// </summary>
		private void NormalizeRequest(ReportRequestModel request, ReportAdvancedModel report)
		{
			foreach (ReportAdvancedRequestDimension fixedRequest in report.RequestDimensions)
				if (fixedRequest.Required || CheckIsRequestedAnyField(request, fixedRequest))
					foreach (ReportAdvancedRequestDimensionField field in fixedRequest.Fields)
						request.Add(fixedRequest.DimensionKey, field.Field);

			// Comprueba si se ha solicitado alguno de los campos considerados como obligatorios
			bool CheckIsRequestedAnyField(ReportRequestModel reportRequest, ReportAdvancedRequestDimension fixedRequest)
			{
				DimensionRequestModel dimensionRequest = reportRequest.GetDimensionRequest(fixedRequest.DimensionKey);

					// Si se ha solicitado la dimensión
					if (dimensionRequest is not null)
						foreach (ReportAdvancedRequestDimensionField field in fixedRequest.Fields)
							if (dimensionRequest.GetRequestColumn(field.Field) is not null)
								return true;
					// Si ha llegado hasta aquí es porque no se ha solicitado
					return false;
			}
		}

		/// <summary>
		///		Obtiene la SQL asociada a una serie de consultas
		/// </summary>
		private string GetSql(List<QuerySqlModel> queries)
		{
			string sql = string.Empty;

				// Obtiene la cadena de la consulta
				foreach (QuerySqlModel query in queries)
					switch (query.Type)
					{
						case QuerySqlModel.QueryType.Block:
								sql += "WITH" + Environment.NewLine;
								sql += GetSql(query.Queries);
							break;
						case QuerySqlModel.QueryType.Cte:
								int index = queries.IndexOf(query);

									// Añade la Cte a la consulta
									sql += query.Key + " AS " + Environment.NewLine;
									sql += "(" + Environment.NewLine + query.Sql + Environment.NewLine +  ")";
									// Añade una coma si es necesario
									if (index < queries.Count - 1 && queries[index + 1].Type == QuerySqlModel.QueryType.Cte)
										sql += ",";
									// Añade un salto de una línea
									sql += Environment.NewLine;
							break;
						case QuerySqlModel.QueryType.Execution:
						case QuerySqlModel.QueryType.Query:
								sql += query.Sql;
							break;
					}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL de una serie de bloques
		/// </summary>
		private List<QuerySqlModel> GetQueries(List<BaseBlockModel> blocks)
		{
			List<QuerySqlModel> queries = new();

				// Genera el SQL de los bloques
				foreach (BaseBlockModel blockBase in blocks)
					switch (blockBase)
					{
						case BlockWithModel block:
								queries.Add(GetQuery(block));
							break;
						case BlockExecutionModel block:
								queries.Add(GetQuery(block));
							break;
						case BlockQueryModel block:
								queries.Add(GetQuery(block, queries));
							break;
						case BlockCreateCteDimensionModel block:
								queries.Add(GetQuery(block));
							break;
						case BlockCreateCteModel block:
								queries.Add(GetQuery(block));
							break;
						case BlockIfRequest block:
								queries.AddRange(GetQueries(block));
							break;
						default:
							throw new ReportingParserException($"Unknown block type {blockBase.Key}. {blockBase.GetType().ToString()}");
					}
				// Devuelve la lista de consultas
				return queries;
		}

		/// <summary>
		///		Obtiene la cadena SQL de un bloque With
		/// </summary>
		private QuerySqlModel GetQuery(BlockWithModel block)
		{
			QuerySqlModel query = new(QuerySqlModel.QueryType.Block, block.Key, string.Empty);

				// Interpreta los bloques
				query.Queries.AddRange(GetQueries(block.Blocks));
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Obtiene la SQL de un bloque de ejecución
		/// </summary>
		private QuerySqlModel GetQuery(BlockExecutionModel block)
		{
			return new(QuerySqlModel.QueryType.Execution, block.Key, block.Sql);
		}

		/// <summary>
		///		Obtiene la SQL de un bloque de consulta
		/// </summary>
		private QuerySqlModel GetQuery(BlockQueryModel block, List<QuerySqlModel> queriesBlock)
		{
			ParserSection parser = new(block.Sql);

				// Convierte las secciones
				foreach ((string marker, ParserBaseSectionModel section) in parser.Parse())
					switch (section)
					{ 
						case ParserFieldsSectionModel item:
								parser.Replace(marker, GetSqlForFields(item));
							break;
						case ParserJoinSectionModel item:
								parser.Replace(marker, GetSqlForJoin(item));
							break;
						case ParserGroupBySectionModel item:
								parser.Replace(marker, GetSqlForGroupBy(item));
							break;
						case ParserOrderBySectionModel item:
								parser.Replace(marker, GetSqlForOrderBy(item));
							break;
						case ParserPartitionBySectionModel item:
								parser.Replace(marker, GetSqlForPartitionBy(item));
							break;
						case ParserIfRequestExpressionSectionModel item:
								parser.Replace(marker, GetSqlForRequestExpression(item));
							break;
						case ParserWhereSectionModel item:
								parser.Replace(marker, GetSqlForWhere(item));
							break;
						case ParserSubquerySectionModel item:
								parser.Replace(marker, GetSqlForSubqueries(item, queriesBlock));
							break;
						case ParserPaginationSectionModel item:
								parser.Replace(marker, GetSqlForPagination());
							break;
						default:
							throw new ReportingParserException($"Unknown section: {section.GetType().ToString()}");
					}
				// Quita los marcadores vacíos
				parser.RemoveMarkers();
				// Devuelve la consulta
				return new(QuerySqlModel.QueryType.Query, block.Key, parser.Sql);
		}

		/// <summary>
		///		Crea la SQL de un bloque de CTE a partir de una dimensión
		/// </summary>
		private QuerySqlModel GetQuery(BlockCreateCteDimensionModel block)
		{
			DimensionModel dimension = Report.DataWarehouse.Dimensions[block.DimensionKey];

				if (dimension is null)
					throw new ReportingParserException($"Can't find the dimension {block.DimensionKey}");
				else
				{
					string sql = GetSqlDimension(dimension, block.Fields);
					
						// Añade los JOIN a la consulta
						foreach (ClauseJoinModel join in block.Joins)
						{
							DimensionModel? dimensionJoin = GetDimensionIfRequest(join.DimensionKey, join.Required, join.RelatedRequestedDimensionKeys, null);

								if (dimensionJoin is not null)
								{
									ParserJoinSectionModel section = new();
									string sqlJoin;

											// Convierte la sección a partir de la cláusula
											section.Convert(join, dimension.Id);
											// Obtiene la SQL de la unión
											sqlJoin = GetSqlForJoin(section);
											// Añade la SQL del JOIN a la consulta
											if (!string.IsNullOrWhiteSpace(sqlJoin))
												sql += Environment.NewLine + sqlJoin;
								}
						}
						// Añade los filtros adicionales a la consulta
						sql += Environment.NewLine + GetSqlForFilters(block.Filters);
						// Devuelve la consulta
						return new QuerySqlModel(QuerySqlModel.QueryType.Cte, block.Key, sql);
				}
		}

		/// <summary>
		///		Crea la SQL de un bloque de CTE
		/// </summary>
		private QuerySqlModel GetQuery(BlockCreateCteModel block)
		{
			List<QuerySqlModel> queries = GetQueries(block.Blocks);
			string sql = string.Empty;

				// Añade los valores de las consultas a la CTE
				foreach (QuerySqlModel query in queries)
					if (!query.IsSubquery)
						sql = sql.AddWithSeparator(query.Sql, Environment.NewLine, false);
				// Devuelve el resultado de la consulta
				if (string.IsNullOrWhiteSpace(sql))
					throw new ReportingParserException($"There is no SQL query in CTE block '{block.Key}'");
				else
					return new QuerySqlModel(QuerySqlModel.QueryType.Cte, block.Key, sql);
		}

		/// <summary>
		///		Obtiene los datos de una dimensión si se ha solicitado
		/// </summary>
		private DimensionModel? GetDimensionIfRequest(ParserDimensionModel parserDimension)
		{
			return GetDimensionIfRequest(parserDimension.DimensionKey, parserDimension.Required, parserDimension.RelatedDimensions, 
										 parserDimension.IfNotRequestDimensions);
		}

		/// <summary>
		///		Obtiene los datos de una dimensión si se ha solicitado
		/// </summary>
		private DimensionModel? GetDimensionIfRequest(string dimensionKey, bool required, List<string> relatedDimensions, List<string>? notRequestedDimensions)
		{
			DimensionModel dimensionJoin = Report.DataWarehouse.Dimensions[dimensionKey];

				if (dimensionJoin is null)
					throw new ReportingParserException($"Can't find the dimension {dimensionKey}");
				else if (required || 
						 ((Request.IsRequestedDimension(dimensionJoin.Id) || Request.IsRequestedDimension(relatedDimensions)) && 
							!Request.IsRequestedDimension(notRequestedDimensions)))
					return dimensionJoin;
				else
					return null;
		}

		/// <summary>
		///		Obtiene la cadena SQL adicional para los filtros
		/// </summary>
		private string GetSqlForFilters(List<ClauseFilterModel> filters)
		{
			string sql = string.Empty;

				// Añade los filtros
				foreach (ClauseFilterModel filter in filters)
					sql = sql.AddWithSeparator(filter.Sql + Environment.NewLine, " AND ");
				// Añade la cláusula WHERE si es necesario
				if (!string.IsNullOrWhiteSpace(sql))
					sql = Environment.NewLine + " WHERE " + sql;
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la SQL adecuada para el JOIN con una dimensión
		/// </summary>
		private string GetSqlForJoin(ParserJoinSectionModel section)
		{
			if (section.Relations.Count == 0)
				throw new ReportingParserException($"Can't find relations at join {section.Join.ToString()}");
			else
			{
				string sql = string.Empty;
				List<(string dimensionTable, string dimensionAlias, string join)> sqlJoins = new();

					// Añade los campos JOINS de las relaciones
					foreach (ParserJoinRelationSectionModel relation in section.Relations)
						if (relation.Dimension is null)
							throw new ReportingParserException($"Can't find dimension at relation join {section.Join.ToString()}");
						else
						{
							DimensionModel? dimensionJoin = GetDimensionIfRequest(relation.Dimension);
							string sqlDimension = string.Empty;

								// Si se ha solicitado algo en la dimensión
								if (dimensionJoin is not null)
								{
									// Crea las condiciones de las relaciones
									if (relation.RelatedByFieldRequest)
										foreach ((string tableDimension, string fieldDimension) in GetFieldsRequest(relation.Dimension, false))
											sqlDimension = sqlDimension.AddWithSeparator
															($"{ComposeField(section.TableJoin, fieldDimension, true)} = {ComposeField(tableDimension, fieldDimension, true)}",
															 Environment.NewLine + " AND ");
									else
										foreach ((string fieldDimension, string fieldTable) in relation.Fields)
											sqlDimension = sqlDimension.AddWithSeparator
															($"{ComposeField(section.TableJoin, fieldTable, false)} = {ComposeField(relation.Dimension.TableAlias, fieldDimension, false)}",
															 Environment.NewLine + " AND ");
									// Añade la SQL adicional si es necesario
									if (!string.IsNullOrWhiteSpace(relation.AdditionalJoinSql))
										sqlDimension = sqlDimension.AddWithSeparator(relation.AdditionalJoinSql, Environment.NewLine + " AND ");
									// Si hay algo que añadir, lo añade a la lista de relaciones
									if (!string.IsNullOrWhiteSpace(sqlDimension))
										sqlJoins.Add((relation.Dimension.Table, relation.Dimension.TableAlias, sqlDimension));
								}
							}
					// Crea la cadena del JOIN final
					if (sqlJoins.Count > 0)
					{
						string lastTable = string.Empty;

							// Genera la cadena SQL
							foreach ((string dimensionTable, string dimensionAlias, string join) in sqlJoins)
							{
								string table = string.Empty;

									// Añade la tabla
									if (string.IsNullOrWhiteSpace(lastTable) || !lastTable.Equals(dimensionAlias, StringComparison.CurrentCultureIgnoreCase))
									{
										// Asigna el nombre de tabla
										if (!dimensionTable.Equals(dimensionAlias, StringComparison.CurrentCultureIgnoreCase))
											table = $"{dimensionTable} AS {dimensionAlias}";
										else
											table = dimensionTable;
										// Guarda la última tabla añadida
										lastTable = dimensionAlias;
									}
									// Si tiene que añadir un nombre de tabla, lo añade, si no, añade una cláusula AND
									if (!string.IsNullOrWhiteSpace(table))
										sql = sql.AddWithSeparator(@$"{section.GetJoin()} {table}
																		ON ", 
																	Environment.NewLine);
									else
										sql += " AND ";
									// Añade la relación
									sql = sql.AddWithSeparator(join, Environment.NewLine);
							}
					}
					// Devuelve la cadena SQL generada
					return sql;
			}

			// Rutina para componer el nombre del campo
			string ComposeField(string table, string field, bool useIsNull)
			{
				string sql = GetFieldName(table, field);

					// Añade la función IsNull si es necesario
					if (useIsNull)
						return $"IsNull({sql}, '')";
					else
						return sql;
			}
		}

		/// <summary>
		///		Obtiene la SQL para una <see cref="ParserFieldsSectionModel"/>
		/// </summary>
		private string GetSqlForFields(ParserFieldsSectionModel section) 
		{
			string sql = GetSqlFieldsForDimensions(section.ParserDimensions);

				// Añade una coma si es obligatoria
				if (!string.IsNullOrWhiteSpace(sql) && section.WithComma)
					sql += ", ";
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
		/// </summary>
		private string GetSqlFieldsForDimensions(List<ParserDimensionModel> dimensions)
		{
			string sql = string.Empty;

				// Obtiene los campos
				foreach (ParserDimensionModel dimension in dimensions)
				{
					DimensionModel? dimensionJoin = GetDimensionIfRequest(dimension);

						if (dimensionJoin is not null)
						{
							DimensionRequestModel request = Request.GetDimensionRequest(dimensionJoin.Id);
							List<(string table, string field)> fields = GetFieldsRequest(dimension, dimension.WithPrimaryKeys);

								// Añade los campos solicitados a la SQL
								foreach ((string table, string field) in fields)
								{
									string sqlField = string.Empty;

										if (dimension.CheckIfNull)
											sqlField = $"IsNull({GetFieldName(table, field)}, {GetFieldName(dimension.AdditionalTable, field)}) AS {field}";
										else
											sqlField = GetFieldName(table, field);
										// Añade el campo a la cadena SQL
										sql = sql.AddWithSeparator(sqlField, ",");
								}
						}
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL para una condición WHERE
		/// </summary>
		/// <example>
		/// 	WHERE IsNull(PointsOfSaleCte.[PointOfSale], '') = IsNull(PointsOfSaleAccumulatedCte.[PointOfSale], '')
		/// 	AND IsNull(PointsOfSaleCte.[ErpCode], '') = IsNull(PointsOfSaleAccumulatedCte.[ErpCode], '')
		/// 	AND IsNull(PointsOfSaleCte.[PointOfSaleImageUrl], '') = IsNull(PointsOfSaleAccumulatedCte.[PointOfSaleImageUrl], '')
		/// 	AND SalesAnalysisAccumulated.Date BETWEEN @StartDate AND SalesAnalysis.Date
		/// 	AND SalesAnalysisAccumulated.Refund = 0
		/// </example>
		private string GetSqlForWhere(ParserWhereSectionModel section)
		{
			string sql = string.Empty;

				sql = "falta interpretación de la sección Where";
				/*
				JAB: comentado por cambio en el modelo
				// Obtiene las comparaciones de los campos
				foreach (ParserDimensionModel parserDimension in section.ParserDimensions)
				{
					DimensionModel? dimensionJoin = GetDimensionIfRequest(parserDimension);

						if (dimensionJoin is not null)
						{
							List<(string table, string field)> fields = GetFieldsRequest(parserDimension, parserDimension.WithPrimaryKeys);

								// Añade las cláusulas IsNull de campos sobre n tablas
								foreach ((string _, string field) in fields)
								{
									string sqlField = string.Empty;

										// Añade los nombres de campos en cada tabla
										foreach (string table in parserDimension.GetAllTables())
											sqlField = sqlField.AddWithSeparator(GetFieldName(table, field), ",");
										// Añade la comparación de si es nulo y el alias
										if (!string.IsNullOrWhiteSpace(sqlField))
											sql = sql.AddWithSeparator($"IsNull({sqlField}) AS {field}", ",");
								}
						}
				}
				// Añade la SQL adicional si es necesario
				sql = sql.AddWithSeparator(section.AdditionalSql, " AND ");
				// Añade la cláusula WHERE si es necesario
				if (!string.IsNullOrWhiteSpace(sql))
					sql = " WHERE " + sql;
				*/
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la SQL necesaria para la consulta de una expresión
		/// </summary>
		private string GetSqlForRequestExpression(ParserIfRequestExpressionSectionModel section)
		{
			string sql = string.Empty;

				// Si se ha solicitado alguna de las expresiones, se añade la SQL
				foreach (string expression in section.ExpressionKeys)
					if (Request.IsRequestedExpression(expression))
						sql = sql.AddWithSeparator(section.Sql, Environment.NewLine);
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL necesaria para un GROUP BY
		/// </summary>
		private string GetSqlForGroupBy(ParserGroupBySectionModel section)
		{
			string sql = GetSqlFieldsForDimensions(section.Dimensions);

				// Añade la SQL adicional
				sql = sql.AddWithSeparator(section.AdditionalSql, ",");
				// Obtiene la cadena de salida
				if (!string.IsNullOrWhiteSpace(sql))
					sql = $" GROUP BY {sql}";
				// Devuelve la cadena con los campos
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL necesaria para un GROUP BY
		/// </summary>
		private string GetSqlForPartitionBy(ParserPartitionBySectionModel section)
		{
			string sql = GetSqlFieldsForDimensions(section.Dimensions);

				// Añade los campos adicionales
				if (!string.IsNullOrWhiteSpace(section.Additional))
					sql = sql.AddWithSeparator(section.Additional, ",");
				// Añade la cláusula PARTITION BY si es necesario
				if (!string.IsNullOrWhiteSpace(sql))
					sql = $"PARTITION BY {sql}";
				// Añade la cláusula ORDER BY si es necesario
				if (!string.IsNullOrWhiteSpace(section.OrderBy))
					sql = $"{sql} ORDER BY {section.OrderBy}";
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la lista con los campos asociados a una consulta
		/// </summary>
		private List<(string table, string field)> GetListFields(QueryModel query, string tableAlias, bool includePrimaryKey)
		{
			List<(string table, string field)> fields = new();

				// Añade los campos de la consulta
				foreach (QueryFieldModel field in query.Fields)
					if (includePrimaryKey || !field.IsPrimaryKey)
						fields.Add((tableAlias, field.Alias));
				// Añade los campos hijo
				foreach (QueryJoinModel child in query.Joins)
					fields.AddRange(GetListFields(child.Query, tableAlias, includePrimaryKey));
				// Devuelve los campos
				return fields;
		}

		/// <summary>
		///		Obtiene los campos asociados a una consulta
		/// </summary>
		private string GetSqlFields(QueryModel query, string tableAlias, bool includePrimaryKey)
		{
			return GetSqlFields(GetListFields(query, tableAlias, includePrimaryKey));
		}

		/// <summary>
		///		Obtiene los campos asociados a una consulta
		/// </summary>
		private string GetSqlFields(List<(string table, string field)> fields)
		{
			string sql = string.Empty;

				// Crea la cadena SQL con el nombre de los campos
				foreach ((string table, string field) in fields)
					sql = sql.AddWithSeparator(GetFieldName(table, field), ",");
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cláusula ORDER BY
		/// </summary>
		private string GetSqlForOrderBy(ParserOrderBySectionModel section)
		{
			List<(string table, string field, int orderIndex, DimensionColumnRequestModel.SortOrder sortOrder)> fieldsSort = new();
			string sql = string.Empty;

				// Obtiene los campos para ORDER BY
				foreach (ParserDimensionModel parserDimension in section.Dimensions)
				{
					List<(string tableDimension, string fieldDimension)> fields = GetFieldsRequest(parserDimension, parserDimension.WithPrimaryKeys);
					DimensionRequestModel requestDimension = Request.GetDimensionRequest(parserDimension.DimensionKey);

						// Obtiene las columnas ordenables
						if (requestDimension is not null)
							foreach ((string table, string field) in fields)
							{
								DimensionModel dimension = Report.DataWarehouse.Dimensions[parserDimension.DimensionKey];

									if (dimension is not null)
									{
										DataSourceColumnModel column = dimension.GetColumn(field, true);

											if (column is not null)
											{
												DimensionColumnRequestModel requestColumn = requestDimension.GetRequestColumn(column.Id);

													// Añade los datos de ordenación
													if (requestColumn is not null && requestColumn.OrderBy != BaseColumnRequestModel.SortOrder.Undefined)
														fieldsSort.Add((table, field, requestColumn.OrderIndex, requestColumn.OrderBy));
											}
									}
							}
				}
				// Ordena por el índice
				fieldsSort.Sort((first, second) => first.orderIndex.CompareTo(second.orderIndex));
				// Obtiene la cadena SQL
				foreach ((string table, string field, int orderIndex, DimensionColumnRequestModel.SortOrder sortOrder) in fieldsSort)
					sql = sql.AddWithSeparator($"{GetFieldName(table, field)} {GetSorting(sortOrder)}", ",");
				// Si es obligatorio y está vacío, ordena por el primer campo
				if (section.Required && string.IsNullOrWhiteSpace(sql))
					sql = "1";
				// Añade el ORDER BY
				if (!string.IsNullOrWhiteSpace(sql))
					sql = $"ORDER BY {sql}";
				// Devuelve la cadena SQL
				return sql;

				// Obtiene la cadena con el tipo de ordenación
				string GetSorting(DimensionColumnRequestModel.SortOrder sortOrder)
				{
					if (sortOrder == BaseColumnRequestModel.SortOrder.Descending)
						return "DESC";
					else
						return "ASC";
				}
		}

		/// <summary>
		///		Obtiene la cadena SQL de paginación
		/// </summary>
		private string GetSqlForPagination()
		{
			if (Request.Pagination.MustPaginate)
				return $"OFFSET {(Request.Pagination.Page - 1) * Request.Pagination.RecordsPerPage} ROWS FETCH FIRST {Request.Pagination.RecordsPerPage} ROWS ONLY";
			else
				return string.Empty;
		}

		/// <summary>
		///		Obtiene los campos solicitados de una dimensión
		/// </summary>
		private List<(string tableDimension, string fieldDimension)> GetFieldsRequest(ParserDimensionModel parserDimension, bool includePrimaryKey)
		{
			List<(string tableDimension, string fieldDimension)> fields = new();
			DimensionModel? dimensionJoin = GetDimensionIfRequest(parserDimension);

				// Si se ha solicitado algo de esta dimensión, se obtienen los datos
				if (dimensionJoin is not null)
				{
					DimensionRequestModel request = Request.GetDimensionRequest(dimensionJoin.Id);

						// Añade los campos solicitados a la SQL
						foreach (string field in GetListFields(new QueryDimensionGenerator(this).GetQuery(request), includePrimaryKey))
							fields.Add((parserDimension.TableAlias, field));
				}
				// Devuelve la lista de campos
				return fields;
		}

		/// <summary>
		///		Obtiene los campos asociados a una consulta
		/// </summary>
		private List<string> GetListFields(QueryModel query, bool includePrimaryKey)
		{
			List<string> fields = new();

				// Añade los campos de la consulta
				foreach (QueryFieldModel field in query.Fields)
					if (!field.IsPrimaryKey || includePrimaryKey)
						fields.Add(field.Alias);
				// Añade los campos hijo
				foreach (QueryJoinModel child in query.Joins)
					fields.AddRange(GetListFields(child.Query, includePrimaryKey));
				// Devuelve los campos
				return fields;
		}

		/// <summary>
		///		Obtiene la SQL de consulta de una dimensión
		/// </summary>
		private string GetSqlDimension(DimensionModel dimension, List<ClauseFieldModel> fields)
		{
			QueryModel query;
			DimensionRequestModel request = Request.GetDimensionRequest(dimension.Id);

				// Obtiene la consulta de la solicitud o de la dimensión
				if (request is not null)
					query = new QueryDimensionGenerator(this).GetQuery(request);
				else
					query = new QueryDimensionGenerator(this).GetQuery(dimension);
				// Añade los campos adicionales
				foreach (ClauseFieldModel field in fields)
				{
					string table = dimension.DataSource.GetTableAlias();

						// Añade el campo adicional si no estaba ya en la consulta
						if (!query.ExistsField(table, field.Alias))
							query.Fields.Add(new QueryFieldModel(query, true, dimension.DataSource.GetTableAlias(), field.Field, field.Alias, 
																 BaseColumnRequestModel.SortOrder.Undefined,
																 ExpressionColumnRequestModel.AggregationType.NoAggregated, true));
				}
				// Devuelve la cadena SQL de esta dimensión
				return query.Build();
		}

		/// <summary>
		///		Obtiene la SQL de una serie de subconsultas
		/// </summary>
		private string GetSqlForSubqueries(ParserSubquerySectionModel section, List<QuerySqlModel> queriesBlock)
		{
			if (string.IsNullOrWhiteSpace(section.Name))
				throw new NotImplementedException("Can't find name for clause 'Subquery'");
			else
			{
				// Busca la consulta en el bloque de consultas hermanas
				foreach (QuerySqlModel query in queriesBlock)
					if (query.Key.Equals(section.Name))
					{
						// Indica que la consulta se utiliza como subconsulta
						query.IsSubquery = true;
						// Devuelve la consulta
						return query.Sql;
					}
				// Lanza una excepción si no ha encontrado ninguna consulta con el nombre de la subconsulta
				throw new NotImplementedException($"Can't find the query '{section.Name}' for clause 'Subquery'");
			}
		}

		/// <summary>
		///		Crea la sentencia SQL asociada a un bloque que comprueba si se ha solicitado una (o varias) dimensiones
		/// </summary>
		private List<QuerySqlModel> GetQueries(BlockIfRequest block)
		{
			bool mustExecute = false;

				// Comprueba si se debe ejecutar
				if (block.DimensionKeys.Count > 0)
					mustExecute = Request.IsRequestedDimension(block.DimensionKeys);
				else // ... si no hay dimensiones, se pone a true para que se comprueben las expresiones
					mustExecute = true;
				if (block.ExpressionKeys.Count > 0)
					mustExecute &= Request.IsRequestedExpression(block.ExpressionKeys);
				// Obtiene las consultas
				if (mustExecute)
					return GetQueries(block.Then);
				else
					return GetQueries(block.Else);
		}

		/// <summary>
		///		Obtiene un nombre de tabla.campo
		/// </summary>
		private string GetFieldName(string table, string field)
		{
			if (string.IsNullOrWhiteSpace(table))
				return field;
			else
				return $"{table}.{field}";
		}
	}
}