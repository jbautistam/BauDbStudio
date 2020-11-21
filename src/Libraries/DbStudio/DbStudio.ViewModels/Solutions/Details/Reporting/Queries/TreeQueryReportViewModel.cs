using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bau.Libraries.LibDataStructures.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries
{
	/// <summary>
	///		ViewModel para el árbol con los campos a consultar en un informe
	/// </summary>
	public class TreeQueryReportViewModel : BaseTreeViewModel
	{
		public TreeQueryReportViewModel(ReportViewModel reportViewModel) : base(reportViewModel.ReportingSolutionViewModel.SolutionViewModel)
		{
			ReportViewModel = reportViewModel;
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			// Añade los nodos de dimensiones
			AddDimensionNodes(ReportViewModel.Report);
			// Añade los nodos de expresiones
			AddExpressionNodes(ReportViewModel.Report);
			// Expande los nodos
			ExpandAll();
		}

		/// <summary>
		///		Añade los nodos de dimensiones
		/// </summary>
		private void AddDimensionNodes(ReportModel report)
		{
			NodeColumnViewModel root = new NodeColumnViewModel(this, null, "Dimensiones", null);
			BaseExtendedModelCollection<DimensionModel> dimensions = GetDimensions(report);

				// Ordena las dimensiones
				dimensions.SortByName();
				// Añade las dimensiones
				foreach (DimensionModel dimension in dimensions)
					AddDimensionNodes(root, dimension);
				// Añade el nodo raíz al árbol
				Children.Add(root);
		}

		/// <summary>
		///		Añade un nodo de dimensión (y sus hijos)
		/// </summary>
		private void AddDimensionNodes(NodeColumnViewModel root, DimensionModel dimension)
		{
			NodeColumnViewModel node = new NodeColumnViewModel(this, root, string.IsNullOrEmpty(dimension.Name) ? dimension.GlobalId : dimension.Name, null);
			BaseExtendedModelCollection<DimensionModel> childs = new BaseExtendedModelCollection<DimensionModel>();

				// Añade los campos de la dimensión
				AddColumnNodes(node, dimension.DataSource.Columns, dimension.GlobalId, string.Empty);
				// Crea la colección de dimensiones hija a partir de las relaciones
				foreach (DimensionRelationModel relation in dimension.Relations)
					if (relation.Dimension != null)
						childs.Add(relation.Dimension);
				// Ordena las dimensiones hija
				childs.SortByName();
				// Añade los nodos de dimensión hija
				foreach (DimensionModel child in childs)
					AddDimensionNodes(node, child);
				// Añade el nodo a la raíz
				root.Children.Add(node);
		}

		/// <summary>
		///		Obtiene las dimensiones de un informe
		/// </summary>
		private BaseExtendedModelCollection<DimensionModel> GetDimensions(ReportModel report)
		{
			BaseExtendedModelCollection<DimensionModel> dimensions = new BaseExtendedModelCollection<DimensionModel>();

				// Añade las dimensiones
				foreach (ReportDataSourceModel dataSource in report.Expressions)
					foreach (DimensionRelationModel relation in dataSource.Relations)
						if (dimensions.Search(relation.Dimension.GlobalId) == null)
							dimensions.Add(relation.Dimension);
				// Devuelve las dimensiones
				return dimensions;
		}

		/// <summary>
		///		Añade los nodos de expresiones
		/// </summary>
		private void AddExpressionNodes(ReportModel report)
		{
			NodeColumnViewModel root = new NodeColumnViewModel(this, null, "Expresiones", null);

				// Añade los orígenes de datos
				foreach (ReportDataSourceModel dataSource in report.Expressions)
				{
					NodeColumnViewModel node = new NodeColumnViewModel(this, root, dataSource.DataSource.Name, null);

						// Añade las columnas
						AddColumnNodes(node, dataSource.DataSource.Columns, string.Empty, dataSource.DataSource.GlobalId);
						// Añade el nodo a la raíz
						root.Children.Add(node);
				}
				// Añade el nodo raíz al árbol
				Children.Add(root);
		}

		/// <summary>
		///		Añade los nodos de columnas
		/// </summary>
		private void AddColumnNodes(NodeColumnViewModel root, BaseExtendedModelCollection<DataSourceColumnModel> columns, string dimensionId, string dataSourceId)
		{
			// Ordena las columnas
			columns.SortByName();
			// Añade las columnas adecuadas al árbol
			foreach (DataSourceColumnModel column in columns)
				if (!column.IsPrimaryKey && column.Visible)
				{
					NodeColumnViewModel node = new NodeColumnViewModel(this, root, string.IsNullOrWhiteSpace(column.Name) ? column.ColumnId : column.Name, column);

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
				request.ReportId = ReportViewModel.Report.GlobalId;
				// Obtiene las columnas de dimensión y de expresión
				request.Columns.AddRange(GetRequestColumns(Children));
				// Devuelve la solicitud
				return request;
		}

		/// <summary>
		///		Obtiene las columnas solicitadas
		/// </summary>
		private List<BaseColumnRequestModel> GetRequestColumns(ObservableCollection<IHierarchicalViewModel> nodes)
		{
			List<BaseColumnRequestModel> requestColumns = new List<BaseColumnRequestModel>();

				// Obtiene las columnas seleccionadas de los nodos
				foreach (IHierarchicalViewModel baseNode in nodes)
					if (baseNode is NodeColumnViewModel node && MustIncludeAtQuery(node))
					{
						BaseColumnRequestModel requestColumn = null;

							// Obtiene la solicitud de dimensión o expresión
							if (!string.IsNullOrWhiteSpace(node.DimensionId))
								requestColumn = new DimensionRequestModel
																{
																	DimensionId = node.DimensionId,
																	ColumnId = node.Column.GlobalId
																};
							else if (!string.IsNullOrWhiteSpace(node.DataSourceId))
								requestColumn = new ExpressionRequestModel
																{
																	ReportDataSourceId = node.DataSourceId,
																	ColumnId = node.Column.GlobalId,
																	AggregatedBy = node.GeSelectedAggregation()
																};
							// Si es una columna de solicitud, la añade a la colección
							if (requestColumn != null)
							{
								// Asigna las propiedades adicionales a la dimensión: filtros, ordenación ....
								AssignProperties(requestColumn, node, !string.IsNullOrWhiteSpace(node.DataSourceId));
								// Añade la dimensión solicitada a las columnas
								requestColumns.Add(requestColumn);
							}
					}
					else
						requestColumns.AddRange(GetRequestColumns(baseNode.Children));
				// Devuelve las columnas
				return requestColumns;
		}

		/// <summary>
		///		Comprueba si se debe incluir una columna en la consulta: si es un nodo con una columna de origen de datos y se ha seleccionado el nodo
		///	o se le tiene que aplicar algún filtro en la consulta
		/// </summary>
		private bool MustIncludeAtQuery(NodeColumnViewModel node)
		{
			return node.Column != null && (node.IsChecked || node.FilterWhere.FiltersViewModel.Count > 0 || node.FilterHaving.FiltersViewModel.Count > 0);
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
		///		ViewModel con el informe a consultar
		/// </summary>
		public ReportViewModel ReportViewModel { get; }
	}
}