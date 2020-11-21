using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de informe
	/// </summary>
	public class NodeReportViewModel : BaseTreeNodeViewModel
	{
		public NodeReportViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, ReportModel report) : 
					base(trvTree, parent, report.Name, NodeType.Report, IconType.Table, report, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
		{
			Report = report;
			if (string.IsNullOrWhiteSpace(report.Name))
				Text = report.GlobalId;
		}

		/// <summary>
		///		Carga los nodos del informe
		/// </summary>
		protected override void LoadNodes()
		{
			foreach (ReportDataSourceModel expression in Report.Expressions)
			{
				NodeRootViewModel parentDataSource = new NodeRootViewModel(TreeViewModel, this, NodeType.FilesRoot, expression.DataSource.Name, false);

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
			NodeRootViewModel parent = new NodeRootViewModel(TreeViewModel, parentDataSource, NodeType.FilesRoot, "Relaciones", false);

				// Añade el nodo raíz de dimensiones
				parentDataSource.Children.Add(parent);
				// Añade las dimensiones
				foreach (DimensionRelationModel relation in expression.Relations)
				{
					NodeRootViewModel parentDimension = new NodeRootViewModel(TreeViewModel, parent, NodeType.DataSourcesRoot, relation.Dimension.Name, false);

						// Añade el campo raíz al árbol
						parent.Children.Add(parentDimension);
						// Añade los campos de la relación
						foreach (RelationForeignKey column in relation.ForeignKeys)
							parentDimension.Children.Add(new NodeRootViewModel(TreeViewModel, parentDimension, NodeType.File, 
																			   $"{column.ColumnId} -> {column.TargetColumnId}"));
				}
		}

		/// <summary>
		///		Informe asociado al nodo
		/// </summary>
		public ReportModel Report { get; }
	}
}
