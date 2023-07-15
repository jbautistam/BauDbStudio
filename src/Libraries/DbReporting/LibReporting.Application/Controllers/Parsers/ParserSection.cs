using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers;

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
	internal List<(string marker, ParserBaseSectionModel section)> Parse()
	{
		List<(string marker, ParserBaseSectionModel section)> sections = new();
		List<string> placeholders = Sql.Extract("{{", "}}", false);

			// Interpreta las secciones
			foreach (string placeholder in placeholders)
				if (!string.IsNullOrWhiteSpace(placeholder))
				{
					string alias = $"##{placeholders.IndexOf(placeholder).ToString()}##";
					List<ParserBaseSectionModel> sectionsBase = Parse(placeholder.TrimIgnoreNull());

						// Añade las secciones con su alias
						foreach (ParserBaseSectionModel sectionBase in sectionsBase)
							sections.Add((alias, sectionBase));
						// Sustituye el placeholder de la cadena SQL
						Sql = Sql.Replace(placeholder, alias);
				}
			// Devuelve las secciones generadas
			return sections;
	}

	/// <summary>
	///		Interpreta las líneas
	/// </summary>
	private List<ParserBaseSectionModel> Parse(string content)
	{ 
		List<ParserBaseSectionModel> sections = new();
		BlockInfoCollection blocks = new();

			// Interpreta los bloques
			blocks.Parse(content);
			// Si hay algo que validar
			if (blocks.Blocks.Count == 0)
				throw new Exceptions.ReportingParserException("Can't parse section");
			else
			{
				// Depuración
				System.Diagnostics.Debug.WriteLine(blocks.GetDebugString());
				// Interpreta las líneas correspondientes
				foreach (BlockInfo block in blocks.Blocks)
					if (block.HasHeader("Fields"))
						sections.Add(ParseFields(block));
					else if (block.HasHeader("InnerJoin"))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.InnerJoin));
					else if (block.HasHeader("LeftJoin"))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.LeftJoin));
					else if (block.HasHeader("RightJoin"))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.RightJoin));
					else if (block.HasHeader("FullJoin"))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.FullJoin));
					else if (block.HasHeader("CrossJoin"))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.CrossJoin));
					else if (block.HasHeader("Where"))
						sections.Add(ParseWhere(block));
					else if (block.HasHeader("Subquery"))
						sections.Add(ParseSubquery(block));
					else if (block.HasHeader("GroupBy"))
						sections.Add(ParseGroupBy(block));
					else if (block.HasHeader("OrderBy"))
						sections.Add(ParseOrderBy(block));
					else if (block.HasHeader("Pagination"))
						sections.Add(new ParserPaginationSectionModel());
					else if (block.HasHeader("IfRequest"))
						sections.Add(ParseIfRequestExpression(block));
					else if (block.HasHeader("PartitionBy"))
						sections.Add(ParsePartition(block));
					else
						throw new Exceptions.ReportingParserException($"Section type unknown when parse: {blocks.Blocks[0].Line}");
			}
			// Devuelve la lista de secciones
			return sections;
	}

	/// <summary>
	///		Interpreta una cláusula <see cref="ParserFieldsSectionModel"/>
	/// </summary>
	private ParserFieldsSectionModel ParseFields(BlockInfo block)
	{ 
		ParserFieldsSectionModel fields = new();

			// Carga las dimensiones
			fields.ParserDimensions.AddRange(ParseDimensions(block));
			// Interpreta las propiedades
			fields.WithComma = block.ExistsChildHeader("WithComma");
			// Devuelve los datos del campo
			return fields;
	}

	/// <summary>
	///		Interpreta una cláusula <see cref="ParserJoinSectionModel"/>
	/// </summary>
	private ParserJoinSectionModel ParseJoin(BlockInfo block, ClauseJoinModel.JoinType type)
	{ 
		ParserJoinSectionModel join = new();

			// Asigna las propiedades
			join.Join = type;
			// Añade los parámetros
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					join.Relations.Add(ParseJoinRelation(block));
				else if (child.HasHeader("Relation"))
					join.Relations.Add(ParseJoinRelation(child));
				else if (child.HasHeader("Table"))
					join.Table = child.Content;
				else if (child.HasHeader("Alias"))
					join.TableAlias = child.Content;
				else if (child.HasHeader("AdditionalSql") && join.Relations.Count > 0)
					join.Relations[0].AdditionalJoinSql = child.GetChildsContent();
				else if (child.HasHeader("OnRequestFields") && join.Relations.Count > 0)
					join.Relations[0].RelatedByFieldRequest = true;
				else if (child.HasHeader("On") && join.Relations.Count > 0)
					ParseJoinRelationFields(child, join.Relations[0]);
			// Devuelve la cláusula
			return join;
	}

	/// <summary>
	///		Interpreta una relación para un JOIN
	/// </summary>
	private ParserJoinRelationSectionModel ParseJoinRelation(BlockInfo block)
	{
		ParserJoinRelationSectionModel relation = new();

			// Crea los datos de la relación
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					relation.Dimension = ParseDimension(child);
				else if (child.HasHeader("OnRequestFields"))
					relation.RelatedByFieldRequest = true;
				else if (child.HasHeader("On"))
					ParseJoinRelationFields(child, relation);
				else if (child.HasHeader("AdditionalSql"))
					relation.AdditionalJoinSql = child.GetChildsContent();
			// Devuelve la relación
			return relation;
	}

	/// <summary>
	///		Interpreta los campos de una relación para un JOIN
	/// </summary>
	private void ParseJoinRelationFields(BlockInfo child, ParserJoinRelationSectionModel relation)
	{
		if (child.Content.Equals("RequestFields", StringComparison.CurrentCultureIgnoreCase))
			relation.RelatedByFieldRequest = true;
		else
			relation.AddFieldsJoin(child.Content);
	}

	/// <summary>
	///		Interpreta las dimensiones de un bloque
	/// </summary>
	private List<ParserDimensionModel> ParseDimensions(BlockInfo block)
	{ 
		List<ParserDimensionModel> dimensions = new();

			// Interpreta las dimensiones
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					dimensions.Add(ParseDimension(child));
			// Devuelve las dimensiones
			return dimensions;
	}

	/// <summary>
	///		Interpreta una dimensión
	/// </summary>
	private ParserDimensionModel ParseDimension(BlockInfo block)
	{ 
		ParserDimensionModel dimension = new();

			// Obtiene los datos de la dimensión
			dimension.DimensionKey = block.Content ?? string.Empty;
			// Obtiene los datos
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Name"))
					dimension.DimensionKey = child.Content;
				else if (child.HasHeader("Table"))
					dimension.Table = child.Content;
				else if (child.HasHeader("AdditionalTable"))
					dimension.AdditionalTable = child.Content;
				else if (child.HasHeader("Alias"))
					dimension.TableAlias = child.Content;
				else if (child.HasHeader("PrimaryKeys"))
					dimension.WithPrimaryKeys = true;
				else if (child.HasHeader("Required"))
					dimension.Required = true;
				else if (child.HasHeader("IfRequest"))
					dimension.AddRelatedDimensions(child.Content);
				else if (child.HasHeader("CheckIfNull"))
					dimension.CheckIfNull = true;
			// Devuelve los datos de la dimensión
			return dimension;
	}

	/// <summary>
	///		Interpreta una subconsulta
	/// </summary>
	private ParserSubquerySectionModel ParseSubquery(BlockInfo block)
	{ 
		ParserSubquerySectionModel section = new();

			// Asigna las propiedades
			section.Name = block.Content;
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una cláusula WHERE
	/// </summary>
	private ParserWhereSectionModel ParseWhere(BlockInfo block)
	{ 
		ParserWhereSectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Equal"))
					section.Conditions.Add(ParserCondition(child));
				else if (child.HasHeader("Additional"))
					section.AdditionalSql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una condición de una cláusula
	/// </summary>
	private ParserWhereConditionSectionModel ParserCondition(BlockInfo block)
	{ 
		ParserWhereConditionSectionModel condition = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					condition.Dimension = ParseDimension(child);
				else if (child.HasHeader("Template"))
					condition.Template = child.GetChildsContent();
				else if (child.HasHeader("Table"))
					condition.Table = child.Content;
			// Devuelve la condición
			return condition;
	}

	/// <summary>
	///		Interpreta una una cláusula GROUP BY
	/// </summary>
	private ParserGroupBySectionModel ParseGroupBy(BlockInfo block)
	{ 
		ParserGroupBySectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader("Additional"))
					section.AdditionalSql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una una cláusula ORDER BY
	/// </summary>
	private ParserOrderBySectionModel ParseOrderBy(BlockInfo block)
	{ 
		ParserOrderBySectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader("Additional"))
					section.AdditionalSql = child.GetChildsContent();
				else if (child.HasHeader("Required"))
					section.Required = true;
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una una cláusula IfRequest
	/// </summary>
	private ParserIfRequestExpressionSectionModel ParseIfRequestExpression(BlockInfo block)
	{ 
		ParserIfRequestExpressionSectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Expression"))
					section.AddExpressions(child.Content);
				else if (child.HasHeader("Sql"))
					section.Sql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una una cláusula PARTITION BY
	/// </summary>
	private ParserPartitionBySectionModel ParsePartition(BlockInfo block)
	{ 
		ParserPartitionBySectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader("Dimension"))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader("Additional"))
					section.Additional = child.GetChildsContent();
				else if (child.HasHeader("OrderBy"))
					section.OrderBy = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Reemplaza una sección por una cadena SQL
	/// </summary>
	internal void Replace(string marker, string sql)
	{
		Sql = Sql.Replace(marker, sql);
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
