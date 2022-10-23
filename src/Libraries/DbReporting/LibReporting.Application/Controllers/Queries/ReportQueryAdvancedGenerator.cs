using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;
using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Application.Exceptions;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers;

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
				return GetSql(GetQueries(reportAdvanced.Blocks));
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
						case BlockQueyModel block:
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
			QuerySqlModel query = new(QuerySqlModel.QueryType.Block);

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
			return new(QuerySqlModel.QueryType.Execution)
							{
								Sql = block.Sql
							};
		}

		/// <summary>
		///		Obtiene la SQL de un bloque de consulta
		/// </summary>
		private QuerySqlModel GetQuery(BlockQueyModel block)
		{
			return new(QuerySqlModel.QueryType.Query)
							{
								Sql = GetSqlJoin(block.Sql, block.Joins)
							};
		}

		/// <summary>
		///		Crea la SQL de un bloque de CTE
		/// </summary>
		private QuerySqlModel GetQuery(BlockCreateCteModel cteBlock)
		{
			QuerySqlModel query = new QuerySqlModel(QuerySqlModel.QueryType.Cte)
											{
												Key = cteBlock.Key,
											};

				// Contenido
				if (!string.IsNullOrWhiteSpace(cteBlock.DimensionKey))
					query.Sql = GetSqlDimensionQuery(cteBlock);
				else
					query.Sql = GetSqlJoin(cteBlock.Query.Sql, cteBlock.Query.Joins);
				// Añade los filtros adicionales a la consulta
				query.Sql += GetSqlForFilters(cteBlock.Query.Filters);
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Obtiene la consulta de una dimensión
		/// </summary>
		private string GetSqlDimensionQuery(BlockCreateCteModel block)
		{
			DimensionModel dimension = Report.DataWarehouse.Dimensions[block.DimensionKey];

				if (dimension is null)
					throw new ReportingParserException($"Can't find the dimension {block.DimensionKey}");
				else
				{
					string sql = GetSqlDimension(dimension, block.Query.Fields);
					
						// Añade los JOIN a la consulta
						foreach (ClauseJoinModel join in block.Query.Joins)
						{
							DimensionModel dimensionJoin = GetDimensionIfRequest(join.DimensionKey, join.Required, join.RelatedRequestedDimensionKeys, null);

								if (dimensionJoin is not null)
								{
									ParserSectionModel section = new ParserSectionModel("##_##", ParserSectionModel.SectionType.Join);
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
						// Devuelve la cadena SQL
						return sql;
				}
		}

		/// <summary>
		///		Obtiene los datos de una dimensión si se ha solicitado
		/// </summary>
		private DimensionModel GetDimensionIfRequest(ParserDimensionModel parserDimension)
		{
			return GetDimensionIfRequest(parserDimension.DimensionKey, parserDimension.Required, parserDimension.RelatedDimensions, 
										 parserDimension.IfNotRequestDimensions);
		}

		/// <summary>
		///		Obtiene los datos de una dimensión si se ha solicitado
		/// </summary>
		private DimensionModel GetDimensionIfRequest(string dimensionKey, bool required, List<string> relatedDimensions, List<string> notRequestedDimensions)
		{
			DimensionModel dimensionJoin = Report.DataWarehouse.Dimensions[dimensionKey];

				if (dimensionJoin is null)
					throw new ReportingParserException($"Can't find the dimension {dimensionKey}");
				else if (required || 
						 ((Request.IsRequested(dimensionJoin.Id) || Request.IsRequested(relatedDimensions)) && 
							!Request.IsRequested(notRequestedDimensions)))
					return dimensionJoin;
				else
					return null;
		}

		/// <summary>
		///		Obtiene la cadena SQL de un JOIN
		/// </summary>
		private string GetSqlJoin(string sql, List<ClauseJoinModel> joins)
		{
			ParserSection parser = new(sql);

				// Convierte las secciones
				foreach (ParserSectionModel section in parser.Parse())
					switch (section.Type)
					{
						case ParserSectionModel.SectionType.Fields:
								parser.Replace(section, GetSqlForFields(section));
							break;
						case ParserSectionModel.SectionType.FieldsIfNull:
								parser.Replace(section, GetSqlForFieldsIfNull(section));
							break;
						case ParserSectionModel.SectionType.Join:
								parser.Replace(section, GetSqlForJoin(section));
							break;
						case ParserSectionModel.SectionType.GroupBy:
								parser.Replace(section, GetSqlForGroupBy(section));
							break;
						case ParserSectionModel.SectionType.PartitionBy:
								parser.Replace(section, GetSqlForPartitionBy(section));
							break;
						case ParserSectionModel.SectionType.CheckNull:
								parser.Replace(section, GetSqlForCheckNull(section));
							break;
						default:
							throw new Exceptions.ReportingParserException($"Unknown section: {section.Type.ToString()}");
					}
				// Quita los marcadores vacíos
				parser.RemoveMarkers();
				// Devuelve la cadena SQL
				return parser.Sql;
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
		private string GetSqlForJoin(ParserSectionModel section)
		{
			if (section.ParserDimensions.Count == 0)
				throw new Exceptions.ReportingParserException($"Can't find dimensions at join {section.Join.ToString()}");
			else
			{
				string sql = string.Empty;
				string sqlRelations = string.Empty;

					// Añade el join adecuado
					foreach (ParserDimensionModel parserDimension in section.ParserDimensions)
						if (string.IsNullOrWhiteSpace(parserDimension.DimensionKey))
						{
							if (!string.IsNullOrWhiteSpace(parserDimension.AdditionalJoinSql))
								sqlRelations = sqlRelations.AddWithSeparator(parserDimension.AdditionalJoinSql, Environment.NewLine + " AND ");
							else
								foreach ((string fieldDimension, string fieldTable) in parserDimension.Fields)
									sqlRelations = sqlRelations.AddWithSeparator
														($"{ComposeField(parserDimension.TableRelated, fieldTable, false)} = {ComposeField(parserDimension.TableDimensionAlias, fieldDimension, false)}",
														Environment.NewLine + " AND ");
						}
						else
						{
							DimensionModel dimensionJoin = GetDimensionIfRequest(parserDimension);

								// si se ha solicitado algo en la dimensión
								if (dimensionJoin is not null)
								{
									// Crea las condiciones
									if (parserDimension.RelatedByFieldRequest)
										foreach ((string tableDimension, string fieldDimension) in GetFieldsRequest(parserDimension, false))
											sqlRelations = sqlRelations.AddWithSeparator
															($"{ComposeField(parserDimension.TableRelated, fieldDimension, true)} = {ComposeField(tableDimension, fieldDimension, true)}",
															 Environment.NewLine + " AND ");
									else
										foreach ((string fieldDimension, string fieldTable) in parserDimension.Fields)
											sqlRelations = sqlRelations.AddWithSeparator
															($"{ComposeField(parserDimension.TableRelated, fieldTable, false)} = {ComposeField(parserDimension.TableDimensionAlias, fieldDimension, false)}",
															 Environment.NewLine + " AND ");
								}
						}
					// Si realmente hay alguna relación
					if (!string.IsNullOrWhiteSpace(sqlRelations))
					{
						// Añade la cláusula JOIN con la tabla
						sql += Environment.NewLine + section.GetJoin() + section.ParserDimensions[0].Table;
						if (section.ParserDimensions[0].MustUseTableDimensionAlias)
							sql += " AS " + section.ParserDimensions[0].TableDimensionAlias;
						// Añade las condiciones al JOIN
						sql += Environment.NewLine + " ON " + sqlRelations;
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
		///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
		/// </summary>
		private string GetSqlForFields(ParserSectionModel section)
		{
			string sql = string.Empty;

				// Obtiene los campos
				foreach (ParserDimensionModel parserDimension in section.ParserDimensions)
				{
					DimensionModel dimensionJoin = GetDimensionIfRequest(parserDimension);

						if (dimensionJoin is not null)
						{
							DimensionRequestModel request = Request.GetDimensionRequest(dimensionJoin.Id);

								// Añade los campos solicitados a la SQL
								sql = sql.AddWithSeparator(GetFields(new QueryDimensionGenerator(this).GetQuery(request), parserDimension.Table), ",");
						}
				}
				// Añade la coma si es necesario
				if (section.WithComma && !string.IsNullOrWhiteSpace(sql))
					sql += ", " + Environment.NewLine;
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL para los campos solicitados de las dimensiones cuando debe comprobar si son nulos en varias tablas relacionadas
		///	(útil para los FULL OUTER JOIN)
		/// </summary>
		private string GetSqlForFieldsIfNull(ParserSectionModel section)
		{
			string sql = string.Empty;

				// Obtiene los campos
				foreach (ParserDimensionModel parserDimension in section.ParserDimensions)
				{
					DimensionModel dimensionJoin = GetDimensionIfRequest(parserDimension);

						if (dimensionJoin is not null)
						{
							List<(string table, string field)> fields = GetFieldsRequest(parserDimension, false);

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
				// Añade la coma si es necesario
				if (section.WithComma && !string.IsNullOrWhiteSpace(sql))
					sql += ", " + Environment.NewLine;
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL para comprobar si una serie de campos es nulo
		/// </summary>
		private string GetSqlForCheckNull(ParserSectionModel section)
		{
			return "Working at CheckNull";
		}

		/// <summary>
		///		Obtiene la cadena SQL necesaria para un GROUP BY
		/// </summary>
		private string GetSqlForGroupBy(ParserSectionModel section)
		{
			return GetSqlForClause(section, "GROUP BY");
		}

		/// <summary>
		///		Obtiene la cadena SQL necesaria para un GROUP BY
		/// </summary>
		private string GetSqlForPartitionBy(ParserSectionModel section)
		{
			return GetSqlForClause(section, "PARTITION BY");
		}

		/// <summary>
		///		Obtiene la cadena SQL necesaria para un GROUP BY
		/// </summary>
		private string GetSqlForClause(ParserSectionModel section, string clause)
		{
			string fields = GetSqlForFields(section);

				// Añade una coma si es necesario
				if (!string.IsNullOrWhiteSpace(fields) && section.Required)
					fields += ", ";
				// Añade la cláusula GROUP BY si es necesario
				if (!string.IsNullOrWhiteSpace(fields) || section.Required)
					fields = $"{clause} {fields}" + Environment.NewLine;
				// Devuelve la cadena con los campos
				return fields;
		}

		/// <summary>
		///		Obtiene los campos asociados a una consulta
		/// </summary>
		private string GetFields(QueryModel query, string tableAlias)
		{
			string fields = string.Empty;

				// Añade los campos de la consulta
				foreach (QueryFieldModel field in query.Fields)
					if (!field.IsPrimaryKey)
						fields = fields.AddWithSeparator(GetFieldName(tableAlias, field.Alias), ",");
				// Añade los campos hijo
				foreach (QueryJoinModel child in query.Joins)
					fields = fields.AddWithSeparator(GetFields(child.Query, tableAlias), ",");
				// Devuelve los campos
				return fields;
		}

		/// <summary>
		///		Obtiene los campos solicitados de una dimensión
		/// </summary>
		private List<(string tableDimension, string fieldDimension)> GetFieldsRequest(ParserDimensionModel parserDimension, bool includePrimaryKey)
		{
			List<(string tableDimension, string fieldDimension)> fields = new();
			DimensionModel dimensionJoin = GetDimensionIfRequest(parserDimension);

				// Si se ha solicitado algo de esta dimensión, se obtienen los datos
				if (dimensionJoin is not null)
				{
					DimensionRequestModel request = Request.GetDimensionRequest(dimensionJoin.Id);

						// Añade los campos solicitados a la SQL
						foreach (string field in GetListFields(new QueryDimensionGenerator(this).GetQuery(request), includePrimaryKey))
							fields.Add((parserDimension.TableDimensionAlias, field));
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
		///		Crea la sentencia SQL asociada a un bloque que comprueba si se ha solicitado una (o varias) dimensiones
		/// </summary>
		private List<QuerySqlModel> GetQueries(BlockIfRequest block)
		{
			if (Request.IsRequested(block.DimensionKeys))
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