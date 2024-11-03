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
		ReportRequestModel request = new()
										{
											ReportId = Explorer.ReportViewModel.Report.Id
										};
		List<NodeColumnViewModel> nodes = GetSelectedNodes(Explorer.Children);

			// Obtiene las columnas de dimensión y de expresión
			request.Dimensions.AddRange(GetRequestDimensions(nodes));
			request.Expressions.AddRange(GetRequestExpressions(nodes));
			request.DataSources.AddRange(GetRequestDataSources(nodes));
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
	///		Obtiene las dimensiones solicitadas
	/// </summary>
	private List<DimensionRequestModel> GetRequestDimensions(List<NodeColumnViewModel> nodes)
	{
		List<DimensionRequestModel> dimensions = [];

			// Añade las dimensiones de los nodos
			foreach (NodeColumnViewModel node in nodes)
				if (node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.DimensionColumn && node.Column is not null)
				{
					DimensionRequestModel dimension = new()
														{
															DimensionId = node.DimensionId
														};
					DimensionColumnRequestModel column = new()
															{
																ColumnId = node.Column.Id,
																Visible = node.IsChecked
															};

						// Asigna las propiedades adicionales a la columna: filtros, ordenación ....
						AssignProperties(column, node, !string.IsNullOrWhiteSpace(node.DataSourceId));
						// Añade los datos de la columna a la dimensión y la dimensión a la lista de salida
						dimension.Columns.Add(column);
						dimensions.Add(dimension);
				}
			// Devuelve la lista de dimensiones
			return dimensions;

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
		List<FilterRequestModel> request = [];

			// Añade los filtros
			foreach (ControlItemViewModel item in filtersViewModel.FiltersViewModel)
				if (item is ListItemReportColumnFilterViewModel filterViewModel)
					request.Add(filterViewModel.Filter);
			// Devuelve los filtros
			return request;
	}

	/// <summary>
	///		Obtiene los orígenes de datos solicitados
	/// </summary>
	private List<DataSourceRequestModel> GetRequestDataSources(List<NodeColumnViewModel> nodes)
	{
		List<DataSourceRequestModel> dataSources = [];

			// Añade el origen de datos solicitado
			// Devuelve la lista de orígenes de datos
			return dataSources;
	}

	/// <summary>
	///		Obtiene las expresiones solicitadas
	/// </summary>
	private List<ExpressionColumnRequestModel> GetRequestExpressions(List<NodeColumnViewModel> nodes)
	{
		List<ExpressionColumnRequestModel> expressions = [];

			// Añade las expresiones a partir de los nodos
			foreach (NodeColumnViewModel node in nodes)
				if (node.ColumnNodeType == NodeColumnViewModel.NodeColumnType.ExpressionField)
				{
					ExpressionColumnRequestModel expression = new();

						// Añade los datos de la expresión
						expressions.Add(new ExpressionColumnRequestModel
													{
														ColumnId = node.Text
													}
											);
						// Añade la expresión
						expressions.Add(expression);
				}
			// Devuelve la lista de orígenes de datos
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
