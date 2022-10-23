using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibDataStructures.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries
{
	/// <summary>
	///		ViewModel para el árbol con los campos a consultar en un informe
	/// </summary>
	public class TreeQueryReportViewModel : BaseTreeViewModel
	{
		public TreeQueryReportViewModel(ReportViewModel viewModel)
		{
			ReportViewModel = viewModel;
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			// Añade los nodos de dimensiones
			AddDimensionNodes(ReportViewModel.Report);
			// Añade los nodos de expresiones
			if (ReportViewModel.Report is ReportModel report)
				AddExpressionNodes(report);
			// Expande los nodos
			ExpandAll();
		}

		/// <summary>
		///		Añade los nodos de dimensiones
		/// </summary>
		private void AddDimensionNodes(ReportBaseModel report)
		{
			BaseReportingDictionaryModel<DimensionModel> dimensions = GetDimensions(report);

				// Añade las dimensiones
				if (dimensions is not null)
				{
					NodeColumnViewModel root = new NodeColumnViewModel(this, null, NodeColumnViewModel.NodeColumnType.DimensionsRoot, "Dimensiones", null);

						// Añade los nodos de dimensión
						foreach (DimensionModel dimension in dimensions.EnumerateValuesSorted())
							AddDimensionNodes(root, dimension);
						// Añade el nodo raíz al árbol
						Children.Add(root);
				}
		}

		/// <summary>
		///		Añade un nodo de dimensión (y sus hijos)
		/// </summary>
		private void AddDimensionNodes(NodeColumnViewModel root, DimensionModel dimension)
		{
			NodeColumnViewModel node = new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.Dimension, 
															   dimension.Id, null);
			BaseReportingDictionaryModel<DimensionModel> childs = new BaseReportingDictionaryModel<DimensionModel>();

				// Asigna el código de dimensión
				node.DimensionId = dimension.Id;
				// Añade los campos de la dimensión
				AddColumnNodes(node, dimension.DataSource.Columns, NodeColumnViewModel.NodeColumnType.DimensionColumn, dimension.Id, string.Empty);
				// Crea la colección de dimensiones hija a partir de las relaciones
				foreach (DimensionRelationModel relation in dimension.Relations)
					if (relation.Dimension != null)
						childs.Add(relation.Dimension);
				// Añade los nodos de dimensión hija
				foreach (DimensionModel child in childs.EnumerateValuesSorted())
					AddDimensionNodes(node, child);
				// Añade el nodo a la raíz
				root.Children.Add(node);
		}

		/// <summary>
		///		Obtiene las dimensiones de un informe
		/// </summary>
		private BaseReportingDictionaryModel<DimensionModel> GetDimensions(ReportBaseModel reportBase)
		{
			switch (reportBase)
			{
				case ReportModel report:
					return GetDimensionsColumns(report);
				case ReportAdvancedModel report:
					return GetDimensionsColumns(report);
				default:
					ReportViewModel.ViewModel.SolutionViewModel.MainController.SystemController.ShowMessage($"Report type unknown: {reportBase.GetType().ToString()}");
					return null;
			}
		}

		/// <summary>
		///		Obtiene las dimensiones de un informe
		/// </summary>
		private BaseReportingDictionaryModel<DimensionModel> GetDimensionsColumns(ReportModel report)
		{
			BaseReportingDictionaryModel<DimensionModel> dimensions = new BaseReportingDictionaryModel<DimensionModel>();

				// Añade las dimensiones
				foreach (ReportDataSourceModel dataSource in report.ReportDataSources)
					foreach (DimensionRelationModel relation in dataSource.Relations)
						if (dimensions[relation.Dimension.Id] == null)
							dimensions.Add(relation.Dimension);
				// Devuelve las dimensiones
				return dimensions;
		}

		/// <summary>
		///		Obtiene las dimensiones de un informe avancado
		/// </summary>
		private BaseReportingDictionaryModel<DimensionModel> GetDimensionsColumns(ReportAdvancedModel report)
		{
			BaseReportingDictionaryModel<DimensionModel> dimensions = new BaseReportingDictionaryModel<DimensionModel>();

				// Añade las dimensiones
				foreach (string key in report.DimensionKeys)
				{
					DimensionModel dimension = report.DataWarehouse.Dimensions[key];

						if (dimension is not null)
							dimensions.Add(dimension);
				}
				// Devuelve las dimensiones
				return dimensions;
		}

		/// <summary>
		///		Añade los nodos de expresiones
		/// </summary>
		private void AddExpressionNodes(ReportModel report)
		{
			NodeColumnViewModel root = new NodeColumnViewModel(this, null, NodeColumnViewModel.NodeColumnType.ExpressionsRoot, "Expresiones", null);

				// Añade los orígenes de datos
				foreach (ReportDataSourceModel dataSource in report.ReportDataSources)
				{
					NodeColumnViewModel node = new NodeColumnViewModel(this, root, NodeColumnViewModel.NodeColumnType.Expression, dataSource.DataSource.Id, null);

						// Asigna el Id del origen de datos
						node.DataSourceId = dataSource.DataSource.Id;
						// Añade las columnas
						AddColumnNodes(node, dataSource.DataSource.Columns, NodeColumnViewModel.NodeColumnType.Expression, 
									   string.Empty, dataSource.DataSource.Id);
						// Añade el nodo a la raíz
						root.Children.Add(node);
				}
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
				// Devuelve la solicitud
				return request;
		}

		/// <summary>
		///		Obtiene las columnas de las dimensiones solicitadas
		/// </summary>
		private List<DimensionRequestModel> GetRequestDimensions(ObservableCollection<IHierarchicalViewModel> nodes)
		{
			List<DimensionRequestModel> dimensions = new List<DimensionRequestModel>();

				// Obtiene las columnas seleccionadas de los nodos
				foreach (IHierarchicalViewModel baseNodeRoot in nodes)
					if (baseNodeRoot is NodeColumnViewModel root && root.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DimensionsRoot)
						foreach (IHierarchicalViewModel baseNodeDimension in root.Children)
							if (baseNodeDimension is NodeColumnViewModel nodeDimension &&
								nodeDimension.ColumnNodeType == NodeColumnViewModel.NodeColumnType.Dimension)
							{
								DimensionRequestModel dimension = GetRequestDimension(nodeDimension);

									if (dimension != null)
										dimensions.Add(dimension);
							}
				// Devuelve las columnas de dimensión
				return dimensions;
		}

		/// <summary>
		///		Obtiene la dimensión asociada a un nodo
		/// </summary>
		private DimensionRequestModel GetRequestDimension(NodeColumnViewModel root)
		{
			DimensionRequestModel dimension = null;

				// Obtiene los datos de la dimensión
				if (MustIncludeAtQuery(root))
				{
					// Crea la dimensión
					dimension = new DimensionRequestModel
											{
												DimensionId = root.DimensionId,
											};
					// Añade las columnas y dimensiones hija
					foreach (IHierarchicalViewModel baseChild in root.Children)
						if (baseChild is NodeColumnViewModel node && MustIncludeAtQuery(node))
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
											DimensionRequestModel child = GetRequestDimension(node);

												if (child != null)
													dimension.Childs.Add(child);
										break;
								}
				}
				// Devuelve la dimensión
				return dimension;
		}

		/// <summary>
		///		Obtiene las columnas de las expresiones solicitadas
		/// </summary>
		private List<ExpressionRequestModel> GetRequestExpressions(ObservableCollection<IHierarchicalViewModel> nodes)
		{
			List<ExpressionRequestModel> expressions = new List<ExpressionRequestModel>();

				// Obtiene las columnas seleccionadas de los nodos
				foreach (IHierarchicalViewModel baseNodeRoot in nodes)
					if (baseNodeRoot is NodeColumnViewModel root && root.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ExpressionsRoot)
					{
						foreach (IHierarchicalViewModel baseNodeExpression in root.Children)
							if (baseNodeExpression is NodeColumnViewModel nodeNameExpression && 
								nodeNameExpression.ColumnNodeType == NodeColumnViewModel.NodeColumnType.Expression &&
								MustIncludeAtQuery(nodeNameExpression))
							{
								ExpressionRequestModel expression = new ExpressionRequestModel();

									// Asigna el identificador del origen de datos
									expression.ReportDataSourceId = nodeNameExpression.DataSourceId;
									// Carga las columnas
									foreach (IHierarchicalViewModel baseExpression in nodeNameExpression.Children)
										if (baseExpression is NodeColumnViewModel nodeExpression && MustIncludeAtQuery(nodeExpression))
										{
											ExpressionColumnRequestModel column = new ExpressionColumnRequestModel
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
					}
				// Devuelve las columnas
				return expressions;
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
					foreach (IHierarchicalViewModel baseNode in node.Children)
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
			List<FilterRequestModel> request = new List<FilterRequestModel>();

				// Añade los filtros
				foreach (ControlItemViewModel item in filtersViewModel.FiltersViewModel)
					if (item is ListItemReportColumnFilterViewModel filterViewModel)
						request.Add(filterViewModel.Filter);
				// Devuelve los filtros
				return request;
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			return false;
		}

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
		public ReportViewModel ReportViewModel { get; }
	}
}