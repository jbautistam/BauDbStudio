using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de informe
	/// </summary>
	public class NodeReportViewModel : BaseTreeNodeViewModel
	{
		public NodeReportViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, ReportModel report) : 
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
			foreach (ReportDataSourceModel expression in Report.ReportDataSources)
			{
				NodeRootViewModel parentDataSource = new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.Table, expression.DataSource.Id, false);

					// Añade el nodo raíz
					Children.Add(parentDataSource);
					// Añade el origen de datos
					parentDataSource.Children.Add(new NodeDataSourceViewModel(TreeViewModel, parentDataSource, expression.DataSource));
					// Añade las relaciones
					LoadNodesRelations(parentDataSource, expression);
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
}
