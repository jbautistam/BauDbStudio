using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries.Tools;

/// <summary>
///		Clase de ayuda para la generación de solicitudes a partir de los datos del explorador
/// </summary>
internal class RequestHelper
{
	internal RequestHelper(TreeQueryReportViewModel explorer)
	{
		Explorer = explorer;
	}

	/// <summary>
	///		Obtiene los datos de solicitud con los elementos seleccionados en el exploraros
	/// </summary>
	internal ReportRequestModel GetRequest()
	{
		ReportRequestModel request = new(Explorer.ReportViewModel.Report.DataWarehouse.Id, Explorer.ReportViewModel.Report.Id);
		List<NodeColumnViewModel> nodes = GetSelectedNodes(Explorer.Children);

			// Obtiene las columnas de dimensión y de expresión
			request.Dimensions.AddRange(GetRequestData(NodeColumnViewModel.NodeColumnType.DimensionColumn, nodes));
			request.Expressions.AddRange(GetRequestExpressions(nodes));
			request.DataSources.AddRange(GetRequestData(NodeColumnViewModel.NodeColumnType.DataSourceColumn, nodes));
			request.Parameters.AddRange(GetRequestParameters(nodes));
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Obtiene los nodos seleccionados en el árbol
	/// </summary>
	private List<NodeColumnViewModel> GetSelectedNodes(ObservableCollection<ControlHierarchicalViewModel> nodes)
	{
		List<NodeColumnViewModel> selected = [];

			// Obtiene los nodos
			foreach (ControlHierarchicalViewModel node in nodes)
			{
				// Añade el nodo a la lista de nodos seleccionados
				if (node is NodeColumnViewModel nodeColumn && nodeColumn.MustAddToRequest())
					selected.Add(nodeColumn);
				// Añade los nodos hijo
				selected.AddRange(GetSelectedNodes(node.Children));
			}
			// Devuelve los nodos seleccionados
			return selected;
	}

	/// <summary>
	///		Obtiene los datos solicitados
	/// </summary>
	private List<DataRequestModel> GetRequestData(NodeColumnViewModel.NodeColumnType columnType, List<NodeColumnViewModel> nodes)
	{
		List<DataRequestModel> dataRequests = [];

			// Añade las dimensiones de los nodos
			foreach (NodeColumnViewModel node in nodes)
				if (node.ColumnNodeType == columnType && node.Column is not null)
				{
					DataRequestModel dataRequest = new(node.DimensionId);

						// Añade los datos de la columna a la lista de salida
						dataRequest.Columns.Add(GetColumnRequest(node, false));
						dataRequests.Add(dataRequest);
				}
			// Devuelve la lista de dimensiones
			return dataRequests;
	}

	/// <summary>
	///		Asigna las propiedades adicionales a una columna solicitada: ordenación, filtros, etc...
	/// </summary>
	private ColumnRequestModel GetColumnRequest(NodeColumnViewModel node, bool withHaving)
	{
		ColumnRequestModel column = new(node.Column?.Id ?? "Unknown");

			// Indica si es visible
			column.Visible = node.IsChecked;
			// Añade el filtro Where
			column.FiltersWhere.AddRange(GetFilters(node.FilterWhere));
			// Añade la ordenación y el filtro HAVING en su caso
			if (node.IsChecked)
			{
				// Añade la ordenación
				column.OrderIndex = node.SortIndex;
				column.OrderBy = node.SortOrder;
				// Añade el filtro para la cláusula HAVING
				if (withHaving)
					column.FiltersHaving.AddRange(GetFilters(node.FilterHaving));
			}
			// Devuelve la columna
			return column;
	}

	/// <summary>
	///		Obtiene los filtros
	/// </summary>
	private List<FilterRequestModel> GetFilters(ListReportColumnFilterViewModel filtersViewModel)
	{
		List<FilterRequestModel> request = [];

			// Añade los filtros
			foreach (ControlItemViewModel item in filtersViewModel.FiltersViewModel)
				if (item is ListItemReportColumnFilterViewModel filterViewModel)
					request.Add(filterViewModel.Filter);
			// Devuelve los filtros
			return request;
	}

	/// <summary>
	///		Obtiene las expresiones solicitadas
	/// </summary>
	private List<ColumnRequestModel> GetRequestExpressions(List<NodeColumnViewModel> nodes)
	{
		List<ColumnRequestModel> expressions = [];

			// Añade las expresiones a partir de los nodos
			foreach (NodeColumnViewModel node in nodes)
				if (node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ExpressionField)
				{
					ColumnRequestModel expression = new(node.Text);

						// Asigna los datos de la expresión
						expression.OrderBy = node.SortOrder;
						expression.OrderIndex = node.SortIndex;
						// Añade la expresión
						expressions.Add(expression);
				}
			// Devuelve la lista de expresiones
			return expressions;
	}

	/// <summary>
	///		Obtiene los parámetros solicitados
	/// </summary>
	private List<ParameterRequestModel> GetRequestParameters(List<NodeColumnViewModel> nodes)
	{
		List<ParameterRequestModel> parameters = [];

			// Añade los datos de los parámetros
			foreach (NodeColumnViewModel node in nodes)
				if (node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ParameterField)
					parameters.Add(new ParameterRequestModel
												{
													Key = node.Text,
													Type = Convert(node.Column?.Type),
													Value = node.FilterWhere.GetDefaultValue()
												}
								  );
			// Devuelve la lista de parámetros
			return parameters;

		// Convierte el tipo de parámetro
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
	///		Árbol del explorador
	/// </summary>
	internal TreeQueryReportViewModel Explorer { get; }
}
