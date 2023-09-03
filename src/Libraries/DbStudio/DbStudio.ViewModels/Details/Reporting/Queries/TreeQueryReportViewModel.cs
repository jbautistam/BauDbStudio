using System.Collections.ObjectModel;

using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;

/// <summary>
///		ViewModel para el árbol con los campos a consultar en un informe
/// </summary>
public class TreeQueryReportViewModel : PluginTreeViewModel
{
	// Variables privadas
	private int _sortIndex;

	public TreeQueryReportViewModel(ReportQueryViewModel viewModel)
	{
		ReportViewModel = viewModel;
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void AddRootNodes()
	{
		// Añade los nodos necesarios al árbol de filtros
		AddParameterNodes(ReportViewModel.Report);
		AddDimensionNodes(ReportViewModel.Report);
		AddDataSourcesNodes(ReportViewModel.Report);
		AddExpressionNodes(ReportViewModel.Report);
		// Expande los nodos
		ExpandAll();
	}

	/// <summary>
	///		Añade los nodos de parámetros
	/// </summary>
	private void AddParameterNodes(ReportModel report)
	{
		NodeColumnViewModel root = new(this, null, NodeColumnViewModel.NodeColumnType.ParametersRoot, "Parameters", null);

			// Carga los parámetros
			foreach (ReportParameterModel parameter in report.Parameters)
			{
				DataSourceColumnModel column = new(null)
													{
														Id = parameter.Key,
														IsPrimaryKey = false,
														Alias = parameter.Key,
														Type = parameter.Type,
														Visible = true,
														Required = true
													};
				NodeColumnViewModel node = new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.ParameterField, parameter.Key, column);
				FilterRequestModel filter = new FilterRequestModel
													{
														Condition = FilterRequestModel.ConditionType.Equals,
													};

					// Añade el valor del filtro
					filter.Values.Add(parameter.DefaultValue);
					// Añade el filtro
					node.FilterWhere.Add(filter);
					// Añade el nodo raíz al árbol
					root.Children.Add(node);
			}
			// Añade el nodo raíz al árbol
			Children.Add(root);
	}

	/// <summary>
	///		Añade los nodos de dimensiones
	/// </summary>
	private void AddDimensionNodes(ReportModel report)
	{
		if (report.Dimensions is not null)
		{
			NodeColumnViewModel root = new NodeColumnViewModel(this, null, NodeColumnViewModel.NodeColumnType.DimensionsRoot, "Dimensiones", null);

				// Ordena las dimensiones
				report.Dimensions.Sort((first, second) => first.Id.CompareTo(second.Id));
				// Añade los nodos de dimensión
				foreach (BaseDimensionModel dimension in report.Dimensions)
					AddDimensionNodes(root, dimension);
				// Añade el nodo raíz al árbol
				Children.Add(root);
		}
	}

	/// <summary>
	///		Añade un nodo de dimensión (y sus hijos)
	/// </summary>
	private void AddDimensionNodes(NodeColumnViewModel root, BaseDimensionModel dimension)
	{
		NodeColumnViewModel node = new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.Dimension, 
														   dimension.Id, null);
		BaseReportingDictionaryModel<BaseDimensionModel> childs = new();

			// Asigna el código de dimensión
			node.DimensionId = dimension.Id;
			// Añade los campos de la dimensión
			AddColumnNodes(node, dimension.GetColumns(), NodeColumnViewModel.NodeColumnType.DimensionColumn, dimension.Id, string.Empty);
			// Crea la colección de dimensiones hija a partir de las relaciones
			foreach (DimensionRelationModel relation in dimension.GetRelations())
				if (relation.Dimension is not null)
					childs.Add(relation.Dimension);
			// Añade los nodos de dimensión hija
			foreach (BaseDimensionModel child in childs.EnumerateValuesSorted())
				AddDimensionNodes(node, child);
			// Añade el nodo a la raíz
			root.Children.Add(node);
	}

	/// <summary>
	///		Añade los nodos de orígenes de datos (tablas de hechos)
	/// </summary>
	private void AddDataSourcesNodes(ReportModel report)
	{
		NodeColumnViewModel root = new NodeColumnViewModel(this, null, NodeColumnViewModel.NodeColumnType.DataSourcesRoot, "Orígenes de datos", null);

			// Añade los orígenes de datos
			foreach (ReportDataSourceModel dataSource in report.DataSources)
			{
				NodeColumnViewModel node = new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.DataSource, dataSource.DataSource.Id, null);

					// Asigna el Id del origen de datos
					node.DataSourceId = dataSource.DataSource.Id;
					// Añade las columnas
					AddColumnNodes(node, dataSource.DataSource.Columns, NodeColumnViewModel.NodeColumnType.DataSourceColumn, 
								   string.Empty, dataSource.DataSource.Id);
					// Añade el nodo a la raíz
					root.Children.Add(node);
			}
			// Añade el nodo raíz al árbol
			Children.Add(root);
	}

	/// <summary>
	///		Añade los nodos de expresiones de un <see cref="ReportModel"/>
	/// </summary>
	private void AddExpressionNodes(ReportModel report)
	{
		NodeColumnViewModel root = new NodeColumnViewModel(this, null, NodeColumnViewModel.NodeColumnType.ExpressionsRoot, "Expresiones", null);

			// Añade las expresiones
			foreach (string expression in report.Expressions)
				root.Children.Add(new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.ExpressionField, expression, null));
			// Añade el nodo raíz al árbol
			Children.Add(root);
	}

	/// <summary>
	///		Añade los nodos de columnas
	/// </summary>
	private void AddColumnNodes(NodeColumnViewModel root, BaseReportingDictionaryModel<DataSourceColumnModel> columns, 
								NodeColumnViewModel.NodeColumnType nodeColumnType, string dimensionId, string dataSourceId)
	{
		// Añade las columnas adecuadas al árbol
		foreach (DataSourceColumnModel column in columns.EnumerateValuesSorted())
			if (column.Visible)
			{
				NodeColumnViewModel node = new NodeColumnViewModel(this, root, nodeColumnType, column.Id, column);

					// Asigna las propiedades
					node.DimensionId = dimensionId;
					node.DataSourceId = dataSourceId;
					// Añade el nodo
					root.Children.Add(node);
			}
	}

	/// <summary>
	///		Obtiene la solicitud de informe
	/// </summary>
	internal ReportRequestModel GetReportRequest()
	{
		ReportRequestModel request = new ReportRequestModel();

			// Asigna el código de informe
			request.ReportId = ReportViewModel.Report.Id;
			// Obtiene las columnas de dimensión y de expresión
			request.Dimensions.AddRange(GetRequestDimensions(Children));
			request.Expressions.AddRange(GetRequestExpressions(Children));
			request.DataSources.AddRange(GetRequestDataSources(Children));
			AddRequestParameters(Children, request.Parameters);
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Obtiene las columnas de las dimensiones solicitadas
	/// </summary>
	private List<DimensionRequestModel> GetRequestDimensions(ObservableCollection<ControlHierarchicalViewModel> nodes)
	{
		List<DimensionRequestModel> dimensions = new();

			// Obtiene las columnas seleccionadas de los nodos
			foreach (ControlHierarchicalViewModel baseNodeRoot in nodes)
				if (baseNodeRoot is NodeColumnViewModel root && root.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DimensionsRoot)
					foreach (ControlHierarchicalViewModel baseNodeDimension in root.Children)
						if (baseNodeDimension is NodeColumnViewModel nodeDimension &&
							nodeDimension.ColumnNodeType == NodeColumnViewModel.NodeColumnType.Dimension)
						{
							DimensionRequestModel? dimension = GetRequestDimension(nodeDimension);

								if (dimension is not null)
									dimensions.Add(dimension);
						}
			// Devuelve las columnas de dimensión
			return dimensions;
	}

	/// <summary>
	///		Obtiene la dimensión asociada a un nodo
	/// </summary>
	private DimensionRequestModel? GetRequestDimension(NodeColumnViewModel root)
	{
		DimensionRequestModel? dimension = null;

			// Obtiene los datos de la dimensión
			if (MustIncludeAtQuery(root))
			{
				// Crea la dimensión
				dimension = new DimensionRequestModel
										{
											DimensionId = root.DimensionId,
										};
				// Añade las columnas y dimensiones hija
				foreach (ControlHierarchicalViewModel baseChild in root.Children)
					if (baseChild is NodeColumnViewModel node && node.Column is not null && MustIncludeAtQuery(node))
						switch (node.ColumnNodeType)
						{
							case NodeColumnViewModel.NodeColumnType.DimensionColumn:
									DimensionColumnRequestModel column = new DimensionColumnRequestModel
																				{
																					ColumnId = node.Column.Id,
																					Visible = node.IsChecked
																				};

										// Asigna las propiedades adicionales a la columna: filtros, ordenación ....
										AssignProperties(column, node, !string.IsNullOrWhiteSpace(root.DataSourceId));
										// Añade la columna a las dimensiones
										dimension.Columns.Add(column);
								break;
							case NodeColumnViewModel.NodeColumnType.Dimension:
									DimensionRequestModel? child = GetRequestDimension(node);

										if (child is not null)
											dimension.Childs.Add(child);
								break;
						}
			}
			// Devuelve la dimensión
			return dimension;
	}

	/// <summary>
	///		Obtiene las columnas seleccionadas para los orígenes de datos
	/// </summary>
	private List<DataSourceRequestModel> GetRequestDataSources(AsyncObservableCollection<ControlHierarchicalViewModel> nodes)
	{
		List<DataSourceRequestModel> requests = new();

			// Obtiene las columnas seleccionadas de los nodos
			foreach (ControlHierarchicalViewModel baseNodeRoot in nodes)
				if (baseNodeRoot is NodeColumnViewModel root && root.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSourcesRoot)
					foreach (ControlHierarchicalViewModel baseNodeDataSources in root.Children)
						if (baseNodeDataSources is NodeColumnViewModel nodeDataSource &&
							nodeDataSource.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSource)
						{
							DataSourceRequestModel? dataSourceRequest = GetRequestDataSource(nodeDataSource);

								if (dataSourceRequest is not null)
									requests.Add(dataSourceRequest);
						}
			// Devuelve las solicitudes
			return requests;
	}

	/// <summary>
	///		Obtiene la solicitud de un origen de datos
	/// </summary>
	private DataSourceRequestModel? GetRequestDataSource(NodeColumnViewModel root)
	{
		DataSourceRequestModel? request = null;

			// Obtiene los datos de la consulta del origen de datos
			if (MustIncludeAtQuery(root))
			{
				// Crea la solicitud
				request = new DataSourceRequestModel
										{
											ReportDataSourceId = root.DataSourceId,
										};
				// Añade las columnas hija
				foreach (ControlHierarchicalViewModel baseChild in root.Children)
					if (baseChild is NodeColumnViewModel node && node.Column is not null && MustIncludeAtQuery(node) && 
						node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSourceColumn)
					{
						DataSourceColumnRequestModel column = new()
																	{
																		ColumnId = node.Column.Id
																	};

							// Asigna las propiedades adicionales a la columna: filtros, ordenación ....
							AssignProperties(column, node, !string.IsNullOrWhiteSpace(root.DataSourceId));
							// Añade la columna a la solicitud
							request.Columns.Add(column);
					}
			}
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Obtiene las columnas de las expresiones solicitadas
	/// </summary>
	private List<ExpressionRequestModel> GetRequestExpressions(ObservableCollection<ControlHierarchicalViewModel> nodes)
	{
		List<ExpressionRequestModel> expressions = new List<ExpressionRequestModel>();

			// Obtiene las columnas seleccionadas de los nodos
			foreach (ControlHierarchicalViewModel baseNodeRoot in nodes)
				if (baseNodeRoot is NodeColumnViewModel root && root.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ExpressionsRoot)
					foreach (ControlHierarchicalViewModel baseNodeExpression in root.Children)
						if (baseNodeExpression is NodeColumnViewModel nodeNameExpression)
							switch (nodeNameExpression.ColumnNodeType)
							{
								case NodeColumnViewModel.NodeColumnType.Expression:
										if (MustIncludeAtQuery(nodeNameExpression))
										{
											ExpressionRequestModel expression = new ExpressionRequestModel();

												// Asigna el identificador del origen de datos
												expression.ReportDataSourceId = nodeNameExpression.DataSourceId;
												// Carga las columnas
												foreach (ControlHierarchicalViewModel baseExpression in nodeNameExpression.Children)
													if (baseExpression is NodeColumnViewModel nodeExpression && nodeExpression.Column is not null && 
														MustIncludeAtQuery(nodeExpression))
													{
														ExpressionColumnRequestModel column = new()
																								{
																									ColumnId = nodeExpression.Column.Id,
																									AggregatedBy = nodeExpression.GeSelectedAggregation()
																								};

															// Asigna las propiedades adicionales a la dimensión: filtros, ordenación ....
															AssignProperties(column, nodeExpression, !string.IsNullOrWhiteSpace(nodeExpression.DataSourceId));
															// Añade la columna a la solicitud de expresión
															expression.Columns.Add(column);
													}
												// Asigna la expresión
												expressions.Add(expression);
										}
									break;
								case NodeColumnViewModel.NodeColumnType.ExpressionField:
										if (nodeNameExpression.IsChecked)
										{
											ExpressionRequestModel expressionField = new ExpressionRequestModel();

												// Añade los datos de la expresión
												expressionField.Columns.Add(new ExpressionColumnRequestModel
																					{
																						ColumnId = nodeNameExpression.Text,
																						AggregatedBy = ExpressionColumnRequestModel.AggregationType.NoAggregated
																					}
																		   );
												// Añade la expresión
												expressions.Add(expressionField);
										}
									break;
							}
			// Devuelve las columnas
			return expressions;
	}

	/// <summary>
	///		Obtiene los parámetros seleccionados
	/// </summary>
	private void AddRequestParameters(ObservableCollection<ControlHierarchicalViewModel> nodes, List<ParameterRequestModel> parameters)
	{
		foreach (ControlHierarchicalViewModel baseNodeRoot in nodes)
			if (baseNodeRoot is NodeColumnViewModel root && root.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ParametersRoot)
				foreach (ControlHierarchicalViewModel baseNodeParameter in root.Children)
					if (baseNodeParameter is NodeColumnViewModel nodeParameter && 
							nodeParameter.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ParameterField)
						parameters.Add(new ParameterRequestModel
												{
													Key = nodeParameter.Text, 
													Type = Convert(nodeParameter.Column?.Type),
													Value = nodeParameter.FilterWhere.GetDefaultValue()
												}
										);

		// Convierte el tipo
		ParameterRequestModel.ParameterType Convert(DataSourceColumnModel.FieldType? type)
		{
			return type switch 
					{
						DataSourceColumnModel.FieldType.Date => ParameterRequestModel.ParameterType.Date,
						DataSourceColumnModel.FieldType.Integer => ParameterRequestModel.ParameterType.Integer,
						DataSourceColumnModel.FieldType.Decimal => ParameterRequestModel.ParameterType.Decimal,
						DataSourceColumnModel.FieldType.Boolean => ParameterRequestModel.ParameterType.Boolean,
						_ => ParameterRequestModel.ParameterType.String
					};
		}
	}

	/// <summary>
	///		Comprueba si se debe incluir una columna en la consulta: si es un nodo con una columna de origen de datos y se ha seleccionado el nodo
	///	o se le tiene que aplicar algún filtro en la consulta
	/// </summary>
	private bool MustIncludeAtQuery(NodeColumnViewModel node)
	{
		bool mustInclude = node.Column != null && (node.IsChecked || node.FilterWhere.FiltersViewModel.Count > 0 || node.FilterHaving.FiltersViewModel.Count > 0);

			// Si no se debe incluir, se comprueba si se debe incluir alguno de los nodos hijo
			if (!mustInclude)
				foreach (ControlHierarchicalViewModel baseNode in node.Children)
					if (!mustInclude && baseNode is NodeColumnViewModel child && MustIncludeAtQuery(child))
						mustInclude = true;
			// Devuelve el valor que indica si se debe incluir
			return mustInclude;
	}

	/// <summary>
	///		Asigna las propiedades adicionales a una columna solicitada: ordenación, filtros, etc...
	/// </summary>
	private void AssignProperties(BaseColumnRequestModel columnRequest, NodeColumnViewModel node, bool withHaving)
	{
		// Indica si es visible
		columnRequest.Visible = node.IsChecked;
		// Añade el filtro Where
		columnRequest.FiltersWhere.AddRange(GetFilters(node.FilterWhere));
		// Añade la ordenación y el filtro HAVING en su caso
		if (node.IsChecked)
		{
			// Añade la ordenación
			columnRequest.OrderIndex = node.SortIndex;
			columnRequest.OrderBy = node.SortOrder;
			// Añade el filtro para la cláusula HAVING
			if (withHaving)
				columnRequest.FiltersHaving.AddRange(GetFilters(node.FilterHaving));
		}
	}

	/// <summary>
	///		Obtiene los filtros
	/// </summary>
	private List<FilterRequestModel> GetFilters(ListReportColumnFilterViewModel filtersViewModel)
	{
		List<FilterRequestModel> request = new();

			// Añade los filtros
			foreach (ControlItemViewModel item in filtersViewModel.FiltersViewModel)
				if (item is ListItemReportColumnFilterViewModel filterViewModel)
					request.Add(filterViewModel.Filter);
			// Devuelve los filtros
			return request;
	}

	/// <summary>
	///		Obtiene el índice para la ordenación
	/// </summary>
	internal int GetSortIndex() => ++_sortIndex;

	/// <summary>
	///		Carga los datos de una solicitud
	/// </summary>
	internal void LoadRequest(ReportRequestModel request)
	{
		if (!request.ReportId.Equals(ReportViewModel.Report.Id, StringComparison.CurrentCultureIgnoreCase))
			ReportViewModel.ViewModel.SolutionViewModel.MainController.HostController.SystemController.ShowMessage($"La consulta pertenece al informe {request.ReportId}");
		else
			foreach (ControlHierarchicalViewModel nodeRoot in Children)
				if (nodeRoot is NodeColumnViewModel node)
					switch (node.ColumnNodeType)
					{
						case NodeColumnViewModel.NodeColumnType.ParametersRoot:
								LoadRequestParameters(request.Parameters, node.Children);
							break;
						case NodeColumnViewModel.NodeColumnType.DimensionsRoot:
								LoadRequestDimensions(request.Dimensions, node.Children);
							break;
						case NodeColumnViewModel.NodeColumnType.DataSourcesRoot:
								LoadRequestDataSource(request.DataSources, node.Children);
							break;
						case NodeColumnViewModel.NodeColumnType.ExpressionsRoot:
								LoadRequestExpressions(request, node.Children);
							break;
					}
	}

	/// <summary>
	///		Carga los datos de los valores de los parámetros en la solicitud
	/// </summary>
	private void LoadRequestParameters(List<ParameterRequestModel> parameters, AsyncObservableCollection<ControlHierarchicalViewModel> children)
	{
		foreach (ControlHierarchicalViewModel baseNode in children)
			if (baseNode is NodeColumnViewModel node && node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ParameterField)
				foreach (ParameterRequestModel parameter in parameters)
					if (node.Text.Equals(parameter.Key, StringComparison.CurrentCultureIgnoreCase))
					{
						FilterRequestModel filter = new()
														{
															Condition = FilterRequestModel.ConditionType.Equals
														};

							// Añade el valor
							filter.Values.Add(parameter.Value);
							// Añade el filtro
							node.FilterWhere.Clear();
							node.FilterWhere.Add(filter);
							node.HasFiltersColumn = true;
					}
	}

	/// <summary>
	///		Carga los datos de las solicitudes de dimensiones
	/// </summary>
	private void LoadRequestDimensions(List<DimensionRequestModel> dimensions, AsyncObservableCollection<ControlHierarchicalViewModel> children)
	{
		foreach (ControlHierarchicalViewModel baseNode in children)
			if (baseNode is NodeColumnViewModel node && node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.Dimension)
				foreach (DimensionRequestModel dimension in dimensions)
					if (node.Text.Equals(dimension.DimensionId, StringComparison.CurrentCultureIgnoreCase))
					{
						// Selecciona las columnas
						foreach (ControlHierarchicalViewModel childNode in baseNode.Children)
							if (childNode is NodeColumnViewModel nodeColumn && nodeColumn.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DimensionColumn)
							{
								// Limpia el nodo
								nodeColumn.IsChecked = false;
								// Asigna los datos de la solicitud
								foreach (DimensionColumnRequestModel column in dimension.Columns)
									if (nodeColumn.Column is not null && nodeColumn.Column.Id.Equals(column.ColumnId, StringComparison.CurrentCultureIgnoreCase))
									{
										// Selecciona el nodo
										nodeColumn.IsChecked = true;
										// Añade los filtros
										nodeColumn.AddFilters(column.FiltersWhere, column.FiltersHaving);
										// Cambia la ordenación
										nodeColumn.SortOrder = column.OrderBy;
										nodeColumn.SortIndex = column.OrderIndex;
									}
							}
						// Selecciona las dimensiones hija
						LoadRequestDimensions(dimension.Childs, node.Children);
					}
	}

	/// <summary>
	///		Carga las solicitudes de columnas de los orígenes de datos
	/// </summary>
	private void LoadRequestDataSource(List<DataSourceRequestModel> dataSources, AsyncObservableCollection<ControlHierarchicalViewModel> children)
	{
		foreach (ControlHierarchicalViewModel baseNode in children)
			if (baseNode is NodeColumnViewModel node && node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSource)
				foreach (DataSourceRequestModel dataSource in dataSources)
					if (node.Text.Equals(dataSource.ReportDataSourceId, StringComparison.CurrentCultureIgnoreCase))
						foreach (ControlHierarchicalViewModel childNode in baseNode.Children)
							if (childNode is NodeColumnViewModel nodeColumn && nodeColumn.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSourceColumn)
							{
								// Limpia el nodo
								nodeColumn.IsChecked = false;
								// Asigna los datos de la solicitud
								foreach (DataSourceColumnRequestModel column in dataSource.Columns)
									if (nodeColumn.Column is not null && nodeColumn.Column.Id.Equals(column.ColumnId, StringComparison.CurrentCultureIgnoreCase))
									{
										// Selecciona el nodo
										nodeColumn.IsChecked = true;
										// Añade los filtros
										nodeColumn.AddFilters(column.FiltersWhere, column.FiltersHaving);
									}
							}
	}

	/// <summary>
	///		Carga las expresiones seleccionadas en la solicitud
	/// </summary>
	private void LoadRequestExpressions(ReportRequestModel request, AsyncObservableCollection<ControlHierarchicalViewModel> children)
	{
		foreach (ControlHierarchicalViewModel baseNode in children)
			if (baseNode is NodeColumnViewModel node && node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ExpressionField)
			{
				// Deselecciona el nodo
				node.IsChecked = false;
				// Selecciona el nodo si es alguna de las expressiones seleccionadas
				foreach (ExpressionRequestModel expression in request.Expressions)
					foreach (ExpressionColumnRequestModel column in expression.Columns)
						if (node.Text.Equals(column.ColumnId, StringComparison.CurrentCultureIgnoreCase))
							node.IsChecked = true;
			}
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action) => false;

	/// <summary>
	///		Abre el cuadro de diálogo de propiedades
	/// </summary>
	protected override void OpenProperties()
	{
		// No hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	protected override void DeleteItem()
	{
		// No hace nada, sólo implementa la interface
	}

	/// <summary>
	///		ViewModel del informe
	/// </summary>
	public ReportQueryViewModel ReportViewModel { get; }
}