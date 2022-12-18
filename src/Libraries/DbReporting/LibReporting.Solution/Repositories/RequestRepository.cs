using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Solution.Repositories;

/// <summary>
///		Repositorio para <see cref="ReportRequestModel"/>
/// </summary>
internal class RequestRepository
{
	// Constantes privadas
	private const string TagRoot = "ReportRequest";
	private const string TagId = "Id";
	private const string TagParameter = "Parameter";
	private const string TagKey = "Key";
	private const string TagType = "Type";
	private const string TagValue = "Value";
	private const string TagTypeDate = "Date";
	private const string TagTypeNumeric = "Numeric";
	private const string TagTypeBoolean = "Boolean";
	private const string TagTypeString = "String";
	private const string TagExpression = "Expression";
	private const string TagColumn = "Column";
	private const string TagAggregatedBy = "AggregatedBy";
	private const string TagVisible = "Visible";
	private const string TagOrderIndex = "OrderIndex";
	private const string TagOrderBy = "OrderBy";
	private const string TagWhere = "Where";
	private const string TagHaving = "Having";
	private const string TagCondition = "Condition";
	private const string TagDimension = "Dimension";

	/// <summary>
	///		Carga la solicitud de un archivo
	/// </summary>
	internal ReportRequestModel Load(string fileName)
	{
		ReportRequestModel request = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos del archivo
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
					{
						// Asigna las propiedades
						request.ReportId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
						// Carga los parámetros
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagParameter:
										LoadParameter(nodeML, request.Parameters);
									break;
								case TagDimension:
										request.Dimensions.Add(LoadDimension(nodeML));
									break;
								case TagExpression:
										request.Expressions.Add(LoadExpressions(nodeML));
									break;
							}
					}
			// Devuelve los datos de la solicitud
			return request;
	}

	/// <summary>
	///		Carga los datos de una dimensión
	/// </summary>
	private DimensionRequestModel LoadDimension(MLNode rootML)
	{
		DimensionRequestModel dimension = new();

			// Añade los atributos de la dimensión
			dimension.DimensionId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			// Carga las columnas hija
			dimension.Columns.AddRange(LoadDimensionColumns(rootML));
			// Carga las dimensions hija
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagDimension)
					dimension.Childs.Add(LoadDimension(nodeML));
			// Devuelve los datos
			return dimension;
	}

	/// <summary>
	///		Carga una lista de <see cref="DimensionColumnRequestModel"/>
	/// </summary>
	private List<DimensionColumnRequestModel> LoadDimensionColumns(MLNode rootML)
	{
		List<DimensionColumnRequestModel> columns = new();

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
				{
					DimensionColumnRequestModel column = new();

						// Asigna las propiedades
						column.ColumnId = nodeML.Attributes[TagId].Value.TrimIgnoreNull();
						// Asigna las propiedades básicas
						LoadBaseColumnData(column, nodeML);
						// Carga las columnas básicas
						column.Childs.AddRange(LoadDimensionColumns(nodeML));
						// Añade la columna
						columns.Add(column);
				}
			// Devuelve la lista
			return columns;
	}

	/// <summary>
	///		Carga los datos de una expresión
	/// </summary>
	private ExpressionRequestModel LoadExpressions(MLNode rootML)
	{
		ExpressionRequestModel expression = new();

			// Asigna los datos de la expresión
			expression.ReportDataSourceId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			// Añade las columnas
			expression.Columns.AddRange(LoadExpressionColumns(rootML));
			// Devuelve los datos
			return expression;
	}

	/// <summary>
	///		Carga las columnas de una expresión
	/// </summary>
	private List<ExpressionColumnRequestModel> LoadExpressionColumns(MLNode rootML)
	{
		List<ExpressionColumnRequestModel> columns = new();

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
				{
					ExpressionColumnRequestModel column = new();

						// Carga las propiedades
						column.ColumnId = nodeML.Attributes[TagId].Value.TrimIgnoreNull();
						column.AggregatedBy = nodeML.Attributes[TagAggregatedBy].Value.TrimIgnoreNull().GetEnum(ExpressionColumnRequestModel.AggregationType.Sum);
						// Carga los datos básicos
						LoadBaseColumnData(column, nodeML);
						// Añade la columna a la colección
						columns.Add(column);
				}
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		Carga los datos base de una columna
	/// </summary>
	private void LoadBaseColumnData(BaseColumnRequestModel column, MLNode rootML)
	{
		// Carga los datos de la columna
		column.Visible = rootML.Attributes[TagVisible].Value.GetBool();
		column.OrderIndex = rootML.Attributes[TagOrderIndex].Value.GetInt(0);
		column.OrderBy = rootML.Attributes[TagOrderBy].Value.GetEnum(BaseColumnRequestModel.SortOrder.Undefined);
		// Carga los filtros
		column.FiltersWhere.AddRange(LoadFilters(TagWhere, rootML));
		column.FiltersHaving.AddRange(LoadFilters(TagHaving, rootML));
	}

	/// <summary>
	///		Carga los filtros asociados a una columna
	/// </summary>
	private List<FilterRequestModel> LoadFilters(string tag, MLNode rootML)
	{
		List<FilterRequestModel> filters = new();

			// Añade los datos de los filtros
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == tag)
				{
					FilterRequestModel filter = new();

						// Asigna los atributos
						filter.Condition = nodeML.Attributes[TagCondition].Value.GetEnum(FilterRequestModel.ConditionType.Undefined);
						// Asigna los valores
						foreach (MLNode childML in nodeML.Nodes)
							if (childML.Name == TagParameter)
								filter.Values.Add(ConvertParameter(childML.Attributes[TagType].Value.TrimIgnoreNull(),
																   childML.Attributes[TagValue].Value.TrimIgnoreNull()));																  
						// Añade el filtro a la colección
						filters.Add(filter);
				}
			// Devuelve la colección de filtros
			return filters;
	}

	/// <summary>
	///		Carga los datos de un parámetro
	/// </summary>
	private void LoadParameter(MLNode rootML, Dictionary<string, object> parameters)
	{
		parameters.Add(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
					   ConvertParameter(rootML.Attributes[TagType].Value.TrimIgnoreNull(),
										rootML.Attributes[TagValue].Value.TrimIgnoreNull())
					  );
	}

	/// <summary>
	///		Convierte el parámetro
	/// </summary>
	private object ConvertParameter(string type, string value)
	{
		if (type.Equals(TagTypeDate, StringComparison.CurrentCultureIgnoreCase))
			return value.GetDateTime();
		else if (type.Equals(TagTypeNumeric, StringComparison.CurrentCultureIgnoreCase))
			return value.GetDouble();
		if (type.Equals(TagTypeBoolean, StringComparison.CurrentCultureIgnoreCase))
			return value.GetBool();
		else
			return value;
	}

	/// <summary>
	///		Graba los datos de una solicitud
	/// </summary>
	internal void Save(ReportRequestModel request, string fileName)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los atributos
			rootML.Attributes.Add(TagId, request.ReportId);
			// Añade los datos de la solicitud
			rootML.Nodes.AddRange(GetNodesParameters(request.Parameters));
			rootML.Nodes.AddRange(GetNodesDimensions(request.Dimensions));
			rootML.Nodes.AddRange(GetNodesExpressions(request.Expressions));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene los nodos XML de parámetros
	/// </summary>
	private MLNodesCollection GetNodesParameters(Dictionary<string, object> parameters)
	{
		MLNodesCollection nodesML = new();

			// Añade los parámetros
			foreach (KeyValuePair<string, object> parameter in parameters)
				nodesML.Add(GetNodeParameter(parameter.Key, parameter.Value));
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene el nodo de un parámetro
	/// </summary>
	private MLNode GetNodeParameter(string key, object value)
	{
		MLNode nodeML = new(TagParameter);

			// Asigna los datos
			nodeML.Attributes.Add(TagKey, key);
			// Asigna el tipo y el valor
			switch (value)
			{
				case null:
						nodeML.Attributes.Add(TagValue, string.Empty);
					break;
				case string valueString:
						nodeML.Attributes.Add(TagType, TagTypeString);
						nodeML.Attributes.Add(TagValue, valueString);
					break;
				case DateTime valueDate:
						nodeML.Attributes.Add(TagType, TagTypeDate);
						nodeML.Attributes.Add(TagValue, $"{valueDate:yyyy-MM-dd HH:mm:ss}");
					break;
				case bool valueBool:
						nodeML.Attributes.Add(TagType, TagTypeBoolean);
						nodeML.Attributes.Add(TagValue, valueBool.ToString());
					break;
				default:
						nodeML.Attributes.Add(TagType, TagTypeNumeric);
						nodeML.Attributes.Add(TagValue, (value as double?)?.ToString(System.Globalization.CultureInfo.InvariantCulture));
					break;
			}
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene los nodos de las dimensions
	/// </summary>
	private MLNodesCollection GetNodesDimensions(List<DimensionRequestModel> dimensions)
	{
		MLNodesCollection nodesML = new();

			// Añade los nodos de dimensión
			foreach (DimensionRequestModel dimension in dimensions)
			{
				MLNode nodeML = new(TagDimension);

					// Añade los atributos de la dimensión
					nodeML.Attributes.Add(TagId, dimension.DimensionId);
					// Añade las columnas
					nodeML.Nodes.AddRange(GetNodesDimensionColumns(dimension.Columns));
					// Añade los nodos hijo
					nodeML.Nodes.AddRange(GetNodesDimensions(dimension.Childs));
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Añade los nodos de las columnas de dimensión
	/// </summary>
	private MLNodesCollection GetNodesDimensionColumns(List<DimensionColumnRequestModel> columns)
	{
		MLNodesCollection nodesML = new();

			// Añade los nodos de columnas
			foreach (DimensionColumnRequestModel column in columns)
			{
				MLNode nodeML = new(TagColumn);

					// Añade los atributos
					nodeML.Attributes.Add(TagId, column.ColumnId);
					// Añade los atributos básicos de la columna
					GetBaseColumnAttributes(column, nodeML);
					// Añade las solicitudes de columnas hija
					nodeML.Nodes.AddRange(GetNodesDimensionColumns(column.Childs));
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene los nodos de las expresiones
	/// </summary>
	private MLNodesCollection GetNodesExpressions(List<ExpressionRequestModel> expressions)
	{
		MLNodesCollection nodesML = new();

			// Añade los nodos de expresión
			foreach (ExpressionRequestModel expression in expressions)
			{
				MLNode nodeML = new(TagExpression);

					// Añade los atributos
					nodeML.Attributes.Add(TagId, expression.ReportDataSourceId);
					// Añade las columnas
					foreach (ExpressionColumnRequestModel column in expression.Columns)
					{
						MLNode columnML = nodeML.Nodes.Add(TagColumn);

							// Añade los valores
							columnML.Attributes.Add(TagId, column.ColumnId);
							columnML.Attributes.Add(TagAggregatedBy, column.AggregatedBy.ToString());
							// Añade los atributos base de la columna
							GetBaseColumnAttributes(column, columnML);
					}
					// Añade los datos del nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Añade a un nodo los datos básicos de una columna
	/// </summary>
	private void GetBaseColumnAttributes(BaseColumnRequestModel column, MLNode columnML)
	{
		// Añade los atributos de la columna
		columnML.Attributes.Add(TagVisible, column.Visible);
		columnML.Attributes.Add(TagOrderIndex, column.OrderIndex);
		columnML.Attributes.Add(TagOrderBy, column.OrderBy.ToString());
		// Añade los filtros
		columnML.Nodes.AddRange(GetNodesFilter(TagWhere, column.FiltersWhere));
		columnML.Nodes.AddRange(GetNodesFilter(TagHaving, column.FiltersHaving));
	}

	/// <summary>
	///		Obtiene los nodos del filtro
	/// </summary>
	private MLNodesCollection GetNodesFilter(string tag, List<FilterRequestModel> filters)
	{
		MLNodesCollection nodesML = new();

			// Añade las condiciones
			foreach (FilterRequestModel filter in filters)
			{
				MLNode nodeML = new(tag);

					// Añade los atributos
					nodeML.Attributes.Add(TagCondition, filter.Condition.ToString());
					// Añade los valores del filtro
					foreach (object value in filter.Values)
						nodeML.Nodes.Add(GetNodeParameter("Value", value));
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve la colección de nodos
			return nodesML;
	}
}