using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de informe
	/// </summary>
	public class NodeReportAdvancedViewModel : BaseTreeNodeViewModel
	{
		public NodeReportAdvancedViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, ReportAdvancedModel report) : 
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
			foreach (string dimensionKey in Report.DimensionKeys)
			{
				DimensionModel dimension = Report.DataWarehouse.Dimensions[dimensionKey];

					if (dimension is not null)
						Children.Add(new NodeDimensionViewModel(TreeViewModel, this, dimension));
					else
						Children.Add(new NodeRootViewModel(TreeViewModel, this, TreeReportingViewModel.NodeType.Error,
														   $"Can't find the dimension {dimensionKey}", false, false,
														   BauMvvm.ViewModels.Media.MvvmColor.Red));
			}
		}

		/// <summary>
		///		Informe asociado al nodo
		/// </summary>
		public ReportAdvancedModel Report { get; }
	}
}
