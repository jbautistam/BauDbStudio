using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers;

/// <summary>
///		Intérprete de secciones
/// </summary>
internal class ParserSection
{
	// Constantes privadas
	private const string HeaderName = "Name";
	private const string HeaderDimension = "Dimension";
	private const string HeaderOrderBy = "OrderBy";
	private const string HeaderExpression = "Expression";
	private const string HeaderSql = "Sql";
	private const string HeaderFields = "Fields";
	private const string HeaderInnerJoin = "InnerJoin";
	private const string HeaderLeftJoin = "LeftJoin";
	private const string HeaderRightJoin = "RightJoin";
	private const string HeaderFullJoin = "FullJoin";
	private const string HeaderCrossJoin = "CrossJoin";
	private const string HeaderWhere = "Where";
	private const string HeaderSubquery = "Subquery";
	private const string HeaderGroupBy = "GroupBy";
	private const string HeaderPagination = "Pagination";
	private const string HeaderIfRequest = "IfRequest";
	private const string HeaderPartitionBy = "PartitionBy";
	private const string HeaderWithComma = "WithComma";
	private const string HeaderRelation = "Relation";
	private const string HeaderTable = "Table";
	private const string HeaderAlias = "Alias";
	private const string HeaderOnRequestFields = "OnRequestFields";
	private const string HeaderOn = "On";
	private const string HeaderAdditionalTable = "AdditionalTable";
	private const string HeaderWithPrimaryKeys = "WithPrimaryKeys";
	private const string HeaderWithRequestedFields = "WithRequestedFields";
	private const string HeaderRequired = "Required";
	private const string HeaderCheckIfNull = "CheckIfNull";
	private const string HeaderEqual = "Equal";
	private const string HeaderTemplate = "Template";

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
					if (block.HasHeader(HeaderFields))
						sections.Add(ParseFields(block));
					else if (block.HasHeader(HeaderInnerJoin))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.InnerJoin));
					else if (block.HasHeader(HeaderLeftJoin))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.LeftJoin));
					else if (block.HasHeader(HeaderRightJoin))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.RightJoin));
					else if (block.HasHeader(HeaderFullJoin))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.FullJoin));
					else if (block.HasHeader(HeaderCrossJoin))
						sections.Add(ParseJoin(block, ClauseJoinModel.JoinType.CrossJoin));
					else if (block.HasHeader(HeaderWhere))
						sections.Add(ParseWhere(block));
					else if (block.HasHeader(HeaderSubquery))
						sections.Add(ParseSubquery(block));
					else if (block.HasHeader(HeaderGroupBy))
						sections.Add(ParseGroupBy(block));
					else if (block.HasHeader(HeaderOrderBy))
						sections.Add(ParseOrderBy(block));
					else if (block.HasHeader(HeaderPagination))
						sections.Add(new ParserPaginationSectionModel());
					else if (block.HasHeader(HeaderIfRequest))
						sections.Add(ParseIfRequestExpression(block));
					else if (block.HasHeader(HeaderPartitionBy))
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
			fields.WithComma = block.ExistsChildHeader(HeaderWithComma);
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
				if (child.HasHeader(HeaderDimension))
					join.Relations.Add(ParseJoinRelation(block));
				else if (child.HasHeader(HeaderRelation))
					join.Relations.Add(ParseJoinRelation(child));
				else if (child.HasHeader(HeaderTable))
					join.Table = child.Content;
				else if (child.HasHeader(HeaderAlias))
					join.TableAlias = child.Content;
				else if (child.HasHeader(HeaderSql) && join.Relations.Count > 0)
					join.Relations[0].AdditionalJoinSql = child.GetChildsContent();
				else if (child.HasHeader(HeaderOnRequestFields) && join.Relations.Count > 0)
					join.Relations[0].RelatedByFieldRequest = true;
				else if (child.HasHeader(HeaderOn) && join.Relations.Count > 0)
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
				if (child.HasHeader(HeaderDimension))
					relation.Dimension = ParseDimension(child);
				else if (child.HasHeader(HeaderOnRequestFields))
					relation.RelatedByFieldRequest = true;
				else if (child.HasHeader(HeaderOn))
					ParseJoinRelationFields(child, relation);
				else if (child.HasHeader(HeaderSql))
					relation.AdditionalJoinSql = child.GetChildsContent();
			// Devuelve la relación
			return relation;
	}

	/// <summary>
	///		Interpreta los campos de una relación para un JOIN
	/// </summary>
	private void ParseJoinRelationFields(BlockInfo child, ParserJoinRelationSectionModel relation)
	{
		if (child.Content.Equals(HeaderOnRequestFields, StringComparison.CurrentCultureIgnoreCase))
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
				if (child.HasHeader(HeaderDimension))
					dimensions.Add(ParseDimension(child));
			// Devuelve las dimensiones
			return dimensions;
	}

	/// <summary>
	///		Interpreta una dimensión
	/// </summary>
	private ParserDimensionModel ParseDimension(BlockInfo block)
	{ 
		ParserDimensionModel dimension = new()
											{
												DimensionKey = block.Content ?? string.Empty
											};

			// Obtiene los datos
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderName))
					dimension.DimensionKey = child.Content;
				else if (child.HasHeader(HeaderTable))
					dimension.Table = child.Content;
				else if (child.HasHeader(HeaderAdditionalTable))
					dimension.AdditionalTable = child.Content;
				else if (child.HasHeader(HeaderAlias))
					dimension.TableAlias = child.Content;
				else if (child.HasHeader(HeaderWithPrimaryKeys))
					dimension.WithPrimaryKeys = true;
				else if (child.HasHeader(HeaderWithRequestedFields))
					dimension.WithRequestedFields = true;
				else if (child.HasHeader(HeaderRequired))
					dimension.Required = true;
				else if (child.HasHeader(HeaderIfRequest))
					dimension.AddRelatedDimensions(child.Content);
				else if (child.HasHeader(HeaderCheckIfNull))
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
				if (child.HasHeader(HeaderEqual))
					section.Conditions.Add(ParserCondition(child));
				else if (child.HasHeader(HeaderSql))
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
				if (child.HasHeader(HeaderDimension))
					condition.Dimension = ParseDimension(child);
				else if (child.HasHeader(HeaderTemplate))
					condition.Template = child.GetChildsContent();
				else if (child.HasHeader(HeaderTable))
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
				if (child.HasHeader(HeaderDimension))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderSql))
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
				if (child.HasHeader(HeaderDimension))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderSql))
					section.AdditionalSql = child.GetChildsContent();
				else if (child.HasHeader(HeaderRequired))
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
				if (child.HasHeader(HeaderExpression))
					section.AddExpressions(child.Content);
				else if (child.HasHeader(HeaderSql))
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
				if (child.HasHeader(HeaderDimension))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderSql))
					section.Additional = child.GetChildsContent();
				else if (child.HasHeader(HeaderOrderBy))
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
