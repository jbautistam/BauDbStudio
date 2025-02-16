using System.Collections.ObjectModel;

using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;
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
														Id = parameter.Id,
														IsPrimaryKey = false,
														Alias = parameter.Id,
														Type = parameter.Type,
														Visible = true,
														Required = true
													};
				NodeColumnViewModel node = new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.ParameterField, parameter.Id, column);
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
		NodeColumnViewModel root = new(this, null, NodeColumnViewModel.NodeColumnType.DataSourcesRoot, "Orígenes de datos", null);

			// Añade los orígenes de datos
			foreach (ReportDataSourceModel dataSource in report.DataSources)
			{
				NodeColumnViewModel node = new(this, root, NodeColumnViewModel.NodeColumnType.DataSource, dataSource.DataSource.Id, null);

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
		NodeColumnViewModel root = new(this, null, NodeColumnViewModel.NodeColumnType.ExpressionsRoot, "Expresiones", null);

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
				NodeColumnViewModel node = new(this, root, nodeColumnType, column.Id, column);

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
	internal ReportRequestModel GetReportRequest() => new Tools.RequestHelper(this).GetRequest();

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
			ReportViewModel.ViewModel.SolutionViewModel.MainController.HostController.SystemController.ShowMessage($"The query isn't requested for report {request.ReportId}");
		else
			foreach (ControlHierarchicalViewModel nodeRoot in Children)
				if (nodeRoot is NodeColumnViewModel node)
					switch (node.ColumnNodeType)
					{
						case NodeColumnViewModel.NodeColumnType.ParametersRoot:
								LoadRequestParameters(request.Parameters, node.Children);
							break;
						case NodeColumnViewModel.NodeColumnType.DimensionsRoot:
								LoadRequestDadata(request.Dimensions, node.Children);
							break;
						case NodeColumnViewModel.NodeColumnType.DataSourcesRoot:
								LoadRequestDadata(request.DataSources, node.Children);
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
	private void LoadRequestDadata(List<DataRequestModel> dataRequests, AsyncObservableCollection<ControlHierarchicalViewModel> children)
	{
		foreach (ControlHierarchicalViewModel baseNode in children)
			if (baseNode is NodeColumnViewModel node && node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.Dimension)
				foreach (DataRequestModel dataRequest in dataRequests)
					if (node.Text.Equals(dataRequest.Id, StringComparison.CurrentCultureIgnoreCase))
					{
						// Selecciona las columnas
						foreach (ControlHierarchicalViewModel childNode in baseNode.Children)
							if (childNode is NodeColumnViewModel nodeColumn && nodeColumn.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DimensionColumn)
							{
								// Limpia el nodo
								nodeColumn.IsChecked = false;
								// Asigna los datos de la solicitud
								foreach (ColumnRequestModel column in dataRequest.Columns)
									if (nodeColumn.Column is not null && nodeColumn.Column.Id.Equals(column.Id, StringComparison.CurrentCultureIgnoreCase))
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
					}
	}

	///// <summary>
	/////		Carga las solicitudes de columnas de los orígenes de datos
	///// </summary>
	//private void LoadRequestDataSource(List<DataSourceRequestModel> dataSources, AsyncObservableCollection<ControlHierarchicalViewModel> children)
	//{
	//	foreach (ControlHierarchicalViewModel baseNode in children)
	//		if (baseNode is NodeColumnViewModel node && node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSource)
	//			foreach (DataSourceRequestModel dataSource in dataSources)
	//				if (node.Text.Equals(dataSource.ReportDataSourceId, StringComparison.CurrentCultureIgnoreCase))
	//					foreach (ControlHierarchicalViewModel childNode in baseNode.Children)
	//						if (childNode is NodeColumnViewModel nodeColumn && nodeColumn.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DataSourceColumn)
	//						{
	//							// Limpia el nodo
	//							nodeColumn.IsChecked = false;
	//							// Asigna los datos de la solicitud
	//							foreach (ColumnRequestModel column in dataSource.Columns)
	//								if (nodeColumn.Column is not null && nodeColumn.Column.Id.Equals(column.Id, StringComparison.CurrentCultureIgnoreCase))
	//								{
	//									// Selecciona el nodo
	//									nodeColumn.IsChecked = true;
	//									// Añade los filtros
	//									nodeColumn.AddFilters(column.FiltersWhere, column.FiltersHaving);
	//								}
	//						}
	//}

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
				foreach (ColumnRequestModel expression in request.Expressions)
					if (node.Text.Equals(expression.Id, StringComparison.CurrentCultureIgnoreCase))
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