using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

/// <summary>
///		ViewModel de un nodo de informe
/// </summary>
public class NodeReportViewModel : PluginNodeViewModel
{
	public NodeReportViewModel(PluginTreeViewModel trvTree, PluginNodeViewModel parent, ReportModel report) : 
				base(trvTree, parent, report.Id, TreeReportingViewModel.NodeType.Report.ToString(), TreeReportingViewModel.IconType.Report.ToString(), 
					 report, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
	{
		Report = report;
	}

	/// <summary>
	///		Carga los nodos del informe
	/// </summary>
	protected override void LoadNodes()
	{
		LoadNodesDimensions();
		LoadNodesDataSources();
	}

	/// <summary>
	///		Carga los nodos de dimensiones
	/// </summary>
	private void LoadNodesDimensions()
	{
		foreach (BaseDimensionModel dimension in Report.Dimensions)
			Children.Add(new NodeDimensionViewModel(TreeViewModel, this, dimension));
	}

	/// <summary>
	///		Carga los nodos de orígenes de datos
	/// </summary>
	private void LoadNodesDataSources()
	{
		foreach (ReportDataSourceModel datasource in Report.DataSources)
		{
			NodeRootViewModel parentDataSource = new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.Table, datasource.DataSource.Id, false);

				// Añade el nodo raíz
				Children.Add(parentDataSource);
				// Añade el origen de datos
				parentDataSource.Children.Add(new NodeDataSourceViewModel(TreeViewModel, parentDataSource, datasource.DataSource));
				// Añade las relaciones
				LoadNodesRelations(parentDataSource, datasource);
		}
	}

	/// <summary>
	///		Carga los nodos de relaciones con dimensiones del informe
	/// </summary>
	private void LoadNodesRelations(NodeRootViewModel parentDataSource, ReportDataSourceModel expression)
	{
		foreach (DimensionRelationModel relation in expression.Relations)
		{
			NodeRootViewModel parentDimension = new NodeRootViewModel(TreeViewModel, parentDataSource, TreeReportingViewModel.NodeType.DimensionsRoot, relation.Dimension.Id, false,
																	  true, BauMvvm.ViewModels.Media.MvvmColor.Navy);

				// Añade el campo raíz al árbol
				parentDataSource.Children.Add(parentDimension);
				// Añade los campos de la relación
				foreach (RelationForeignKey column in relation.ForeignKeys)
					parentDimension.Children.Add(new NodeRootViewModel(TreeViewModel, parentDimension, TreeReportingViewModel.NodeType.Field, 
																		$"{column.ColumnId} -> {column.TargetColumnId}", false,
																		false, BauMvvm.ViewModels.Media.MvvmColor.Black));
		}
	}

	/// <summary>
	///		Informe asociado al nodo
	/// </summary>
	public ReportModel Report { get; }
}
