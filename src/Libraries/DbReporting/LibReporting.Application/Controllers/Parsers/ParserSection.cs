using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers
{
	/// <summary>
	///		Intérprete de secciones
	/// </summary>
	internal class ParserSection
	{
		internal ParserSection(string sql)
		{	
			Sql = sql;
		}

		/// <summary>
		///		Interpreta las secciones del SQL
		/// </summary>
		internal List<ParserSectionModel> Parse()
		{
			List<ParserSectionModel> sections = new();
			List<string> placeholders = Sql.Extract("{{", "}}", false);

				// Interpreta las secciones
				foreach (string placeholder in placeholders)
					if (!string.IsNullOrWhiteSpace(placeholder))
					{
						string alias = $"##{placeholders.IndexOf(placeholder).ToString()}##";

							// Convierte la sección
							sections.Add(ConvertSection(alias, placeholder.TrimIgnoreNull()));
							// Sustituye el placeholder de la cadena SQL
							Sql = Sql.Replace(placeholder, alias);
					}
				// Devuelve las secciones generadas
				return sections;
		}

		/// <summary>
		///		Convierte una sección
		/// </summary>
		private ParserSectionModel ConvertSection(string alias, string placeholder)
		{
			if (string.IsNullOrWhiteSpace(placeholder))
				throw new Exceptions.ReportingParserException("Placeholder empty at section {{}}");
			else if (placeholder.StartsWith("Fields ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForFields(alias, placeholder.Substring("Fields ".Length), ParserSectionModel.SectionType.Fields);
			else if (placeholder.StartsWith("GroupBy ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForFields(alias, placeholder.Substring("GroupBy ".Length), ParserSectionModel.SectionType.GroupBy);
			else if (placeholder.StartsWith("PartitionBy ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForFields(alias, placeholder.Substring("PartitionBy ".Length), ParserSectionModel.SectionType.PartitionBy);
			else if (placeholder.StartsWith("InnerJoin ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForJoin(alias, placeholder.Substring("InnerJoin ".Length), ParserSectionModel.SectionType.Join, 
									ClauseJoinModel.JoinType.InnerJoin);
			else if (placeholder.StartsWith("LeftJoin ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForJoin(alias, placeholder.Substring("LeftJoin ".Length), ParserSectionModel.SectionType.Join, 
									ClauseJoinModel.JoinType.LeftJoin);
			else if (placeholder.StartsWith("RightJoin ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForJoin(alias, placeholder.Substring("RightJoin ".Length), ParserSectionModel.SectionType.Join, 
									ClauseJoinModel.JoinType.RightJoin);
			else if (placeholder.StartsWith("FullJoin ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForJoin(alias, placeholder.Substring("FullJoin ".Length), ParserSectionModel.SectionType.Join, 
									ClauseJoinModel.JoinType.FullJoin);
			else if (placeholder.StartsWith("CrossJoin ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForJoin(alias, placeholder.Substring("CrossJoin ".Length), ParserSectionModel.SectionType.Join, 
									ClauseJoinModel.JoinType.CrossJoin);
			else if (placeholder.StartsWith("CheckNull ", StringComparison.CurrentCultureIgnoreCase))
				return ParserForCheckNull(alias, placeholder.Substring("CheckNull ".Length), ParserSectionModel.SectionType.CheckNull);
			else if (placeholder.StartsWith("FieldsIfNull ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForFields(alias, placeholder.Substring("FieldsIfNull ".Length), ParserSectionModel.SectionType.FieldsIfNull);
			else if (placeholder.StartsWith("IfRequest ", StringComparison.CurrentCultureIgnoreCase))
				return ParseForRequestExpression(alias, placeholder.Substring("IfRequest ".Length), ParserSectionModel.SectionType.IfRequestExpression);
			else
				throw new Exceptions.ReportingParserException($"Placeholder unknown: {placeholder}");
		}

		/// <summary>
		///		Interpreta una sección de campos
		/// </summary>
		/// <remarks>
		/// {{Fields
		///		{Dimension:Users Table:UsersCte}
		///		{Dimension:UsersPointOfSale Table:UsersPointsOfSaleCte}
		///		{Dimension:PointsOfSale Table:PointsOfSaleCte}
		///		{Dimension:Products Table:ProductsCte}
		///		WithComma
		///	}}
		/// </remarks>
		private ParserSectionModel ParseForFields(string placeholder, string marker, ParserSectionModel.SectionType type)
		{
			ParserSectionModel section = new(placeholder, type);

				// Interpreta la sección
				marker = Parse(section, marker).TrimIgnoreNull();
				// Obtiene el resto de parámetros
				if (!string.IsNullOrWhiteSpace(marker))
					foreach (string part in marker.Split(' '))
						if (!string.IsNullOrWhiteSpace(part))
						{
							if (part.TrimIgnoreNull().Equals("WithComma", StringComparison.CurrentCultureIgnoreCase))
								section.WithComma = true;
							else if (part.TrimIgnoreNull().Equals("Required", StringComparison.CurrentCultureIgnoreCase))
								section.Required = true;
						}
				// Devuelve la sección generada
				return section;
		}
		
		/// <summary>
		///		Interpreta una sección de campos
		/// </summary>
		/// <remarks>
		/// {{InnerJoin { Dimension:UsersPointOfSale Table:UsersPointOfSaleCte RelatedTo:SalesAnalysis AliasRelated:SalesAnalysisTwo On:UserPointOfSaleId-UserPointOfSaleId} }}
		/// </remarks>
		private ParserSectionModel ParseForJoin(string placeholder, string marker, ParserSectionModel.SectionType type, ClauseJoinModel.JoinType joinType)
		{
			ParserSectionModel section = new(placeholder, type);

				// Asigna el tipo de JOIN
				section.Join = joinType;
				// Interpreta la sección
				marker = Parse(section, marker).TrimIgnoreNull();
				// Devuelve la sección generada
				return section;
		}

		/// <summary>
		///		Interpreta una sección para comprobar campos nulos
		/// </summary>
		/// <remarks>
		/// {{CheckNull { Dimension:UsersPointOfSale Table:SalesAnalysisBaseCte RelatedTo:DaysCte On:RequestFields} 
		///				{ Dimension:Users Table:SalesAnalysisBaseCte RelatedTo:DaysCte On:RequestFields}
		///				{ Dimension:PointsOfSale Table:SalesAnalysisBaseCte RelatedTo:DaysCte On:RequestFields} 
		///				{ Dimension:Products Table:SalesAnalysisBaseCte RelatedTo:DaysCte On:RequestFields}
		/// }}
		/// </remarks>
		private ParserSectionModel ParserForCheckNull(string placeholder, string marker, ParserSectionModel.SectionType type)
		{
			ParserSectionModel section = new(placeholder, type);

				// Interpreta la sección
				marker = Parse(section, marker).TrimIgnoreNull();
				// Devuelve la sección generada
				return section;
		}

		/// <summary>
		///		Interpreta una cadena
		/// </summary>
		/// <remarks>
		///		{Dimension:Users Table:UsersCte}
		///		{Dimension:UsersPointOfSale Table:UsersPointsOfSaleCte}
		///		{Dimension:PointsOfSale Table:PointsOfSaleCte}
		///		{Dimension:Products Table:ProductsCte}
		///		WithComma
		/// </remarks>
		private string Parse(ParserSectionModel section, string marker)
		{
			List<string> placeholders = marker.Extract("{", "}", false);

				// Obtiene las dimensions
				foreach (string placeholder in placeholders)
					if (!string.IsNullOrWhiteSpace(placeholder))
					{
						ParserDimensionModel dimension = new();
						string[] parts = placeholder.Split(' ');

							// Obtiene las diferentes partes de la dimensión
							foreach (string part in parts)
								if (!string.IsNullOrWhiteSpace(part))
								{
									string[] sections = part.Split(':');

										if (sections.Length == 2)
										{
											// Quita los espacios
											sections[0] = sections[0].TrimIgnoreNull();
											sections[1] = sections[1].TrimIgnoreNull();
											// Comprueba los datos
											if (sections[0].Equals("Dimension", StringComparison.CurrentCultureIgnoreCase))
												dimension.DimensionKey = sections[1].TrimIgnoreNull();
											else if (sections[0].Equals("Table", StringComparison.CurrentCultureIgnoreCase))
												dimension.Table = sections[1].TrimIgnoreNull();
											else if (sections[0].Equals("AdditionalTable", StringComparison.CurrentCultureIgnoreCase))
												dimension.AdditionalTables.AddRange(SplitStrings(sections[1].TrimIgnoreNull()));
											else if (sections[0].Equals("Required", StringComparison.CurrentCultureIgnoreCase))
												dimension.Required = sections[1].TrimIgnoreNull().GetBool();
											else if (sections[0].Equals("WithPrimaryKey", StringComparison.CurrentCultureIgnoreCase))
												dimension.WithPrimaryKeys = sections[1].TrimIgnoreNull().GetBool();
											else if (sections[0].Equals("RelatedTo", StringComparison.CurrentCultureIgnoreCase))
												dimension.TableRelated = sections[1].TrimIgnoreNull();
											else if (sections[0].Equals("AliasRelated", StringComparison.CurrentCultureIgnoreCase))
												dimension.TableDimensionAlias = sections[1].TrimIgnoreNull();
											else if (sections[0].Equals("On", StringComparison.CurrentCultureIgnoreCase))
											{
												if (sections[1].Equals("RequestFields", StringComparison.CurrentCultureIgnoreCase))
													dimension.RelatedByFieldRequest = true;
												else
													dimension.Fields.AddRange(GetFields(sections[1].TrimIgnoreNull()));
											}
											else if (sections[0].Equals("IfRequest", StringComparison.CurrentCultureIgnoreCase))
												dimension.RelatedDimensions.AddRange(SplitStrings(sections[1].TrimIgnoreNull()));
											else if (sections[0].Equals("IfNotRequest", StringComparison.CurrentCultureIgnoreCase))
												dimension.IfNotRequestDimensions.AddRange(SplitStrings(sections[1].TrimIgnoreNull()));
											else if (sections[0].Equals("Additional", StringComparison.CurrentCultureIgnoreCase))
												dimension.AdditionalJoinSql = ConcatParts(parts, "Additional:");
										}
								}
							// Añade el placeholder
							section.ParserDimensions.Add(dimension);
							// Quita el placeholder de la cadena
							marker = marker.Replace("{" + placeholder + "}", string.Empty);
					}
				// Devuelve la cadena resultante
				return marker;
		}

		/// <summary>
		///		Obtiene una lista de cadenas a partir de una cadena separada por guiones
		/// </summary>
		/// <example>
		///		IfRequest:Users-PointsOfSale
		/// </example>
		private List<string> SplitStrings(string value)
		{
			List<string> related = new();

				// Separa las dimensiones
				if (!string.IsNullOrWhiteSpace(value))
					foreach (string part in value.Split('-'))
						if (!string.IsNullOrWhiteSpace(part))
							related.Add(part.TrimIgnoreNull());
				// Devuelve la lista de dimensiones relacionadas
				return related;
		}

		/// <summary>
		///		Concatena una serie de cadenas a partir de una
		/// </summary>
		private string ConcatParts(string[] parts, string startAt)
		{
			string result = string.Empty;
			bool found = false;

				// Añade las partes de la cadena (a partir de la cadena inicial)
				foreach (string part in parts)
					if (!string.IsNullOrWhiteSpace(part))
					{
						if (found)
							result += part.TrimIgnoreNull() + " ";
						else if (part.TrimIgnoreNull().Equals(startAt.TrimIgnoreNull(), StringComparison.CurrentCultureIgnoreCase))
							found = true;
					}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Obtiene una lista de campos para el JOIN
		/// </summary>
		/// <example>
		///		On:UserPointOfSaleId-UserPointOfSaleId
		/// </example>
		private List<(string fieldDimension, string fieldTable)> GetFields(string value)
		{
			List<(string fieldDimension, string fieldTable)> fields = new();

				// Separa los campos
				if (!string.IsNullOrWhiteSpace(value))
				{
					string[] parts = value.Split('-');

						switch (parts.Length)
						{
							case 1:
									fields.Add((parts[0].TrimIgnoreNull(), parts[0].TrimIgnoreNull()));
								break;
							case 2:
									fields.Add((parts[0].TrimIgnoreNull(), parts[1].TrimIgnoreNull()));
								break;
						}
				}
				// Devuelve la lista de campos
				return fields;
		}

		/// <summary>
		///		Interpreta una comprobación de expresión
		/// </summary>
		/// <example>
		///		{{ifRequest {Expression:SalesAchievementPercentage
		///							-- % Consecución Ventas	Sum(Importe II) / Sum(ObjetivoVentas)
		///							100.0 * SalesFirst.SalesTaxesIncluded / NullIf(SalesFirst.SalesTarget, 0) AS SalesAchievementPercentage,
		///					}
		///		}}
		private ParserSectionModel ParseForRequestExpression(string placeholder, string marker, ParserSectionModel.SectionType type)
		{
			ParserSectionModel section = new(placeholder, type);
			List<string> parts = marker.Extract("{", "}", false);

				// Interpreta la sección
				foreach (string part in parts)
					if (!string.IsNullOrWhiteSpace(part))
					{
						string[] expressions = part.Split('\n');

							// Si no se ha obtenido ningún valor partiendo por \n, parte por \r
							if (expressions is null || expressions.Length == 0)
								expressions = part.Split('\r');
							// Interpreta las expresiones resultantes
							if (expressions.Length > 0 && expressions[0].StartsWith("Expression:", StringComparison.CurrentCultureIgnoreCase))
							{
								ParserExpressionModel expression = new();
								string expressionKeys = expressions[0].From("Expression:".Length);

									if (string.IsNullOrWhiteSpace(expressionKeys))
										throw new Exceptions.ReportingParserException($"Can't find the expression key: {part}");
									else
									{
										// Asigna las claves de las expresiones
										foreach (string expressionKey in expressionKeys.Split(';'))
											if (!string.IsNullOrWhiteSpace(expressionKey))
												expression.ExpressionKeys.Add(expressionKey.TrimIgnoreNull());
										// Añade la SQL
										for (int index = 1; index < expressions.Length; index++)
											expression.Sql += expressions[index].TrimIgnoreNull() + Environment.NewLine;
										// Añade la expresión a la sección
										section.ParserExpressions.Add(expression);
									}
							}
							else
								throw new Exceptions.ReportingParserException($"Can't parse the expression: {placeholder}");
					}
				// Devuelve la sección generada
				return section;
		}

		/// <summary>
		///		Reemplaza una sección por una cadena SQL
		/// </summary>
		internal void Replace(ParserSectionModel section, string sql)
		{
			Sql = Sql.Replace(section.Placeholder, sql);
		}

		/// <summary>
		///		Quita los marcadores
		/// </summary>
		internal void RemoveMarkers()
		{
			while (!string.IsNullOrWhiteSpace(Sql) && (Sql.IndexOf("{{") >= 0 || Sql.IndexOf("}}") >= 0))
			{
				Sql = Sql.Replace("{{", string.Empty);
				Sql = Sql.Replace("}}", string.Empty);
			}
		}

		/// <summary>
		///		Sql
		/// </summary>
		internal string Sql { get; private set; }
	}
}
