using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo raíz (raíz de origen de datos, informes...)
	/// </summary>
	public class NodeRootViewModel : BaseTreeNodeViewModel
	{
		public NodeRootViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, TreeReportingViewModel.NodeType type, 
								 string text, bool lazyLoad = true, bool bold = true, MvvmColor color = null) :
					base(trvTree, parent, text, type.ToString(), TreeReportingViewModel.IconType.Unknown.ToString(), type, lazyLoad, bold, color ?? MvvmColor.Red)
		{
			switch (NodeType)
			{
				case TreeReportingViewModel.NodeType.DimensionsRoot:
						Icon = TreeReportingViewModel.IconType.Dimension.ToString();
					break;
				case TreeReportingViewModel.NodeType.DataSourcesRoot:
						Icon = TreeReportingViewModel.IconType.Schema.ToString();
					break;
				case TreeReportingViewModel.NodeType.ReportsRoot:
						Icon = TreeReportingViewModel.IconType.Report.ToString();
					break;
				case TreeReportingViewModel.NodeType.File:
						Icon = TreeReportingViewModel.IconType.Field.ToString();
					break;
				case TreeReportingViewModel.NodeType.Table:
						Icon = TreeReportingViewModel.IconType.Path.ToString();
					break;
			}
		}

		/// <summary>
		///		Carga los nodos del tipo
		/// </summary>
		protected override void LoadNodes()
		{
			switch (NodeType)
			{
				case TreeReportingViewModel.NodeType.DataSourcesRoot:
						LoadDataSources();
					break;
				case TreeReportingViewModel.NodeType.DimensionsRoot:
						LoadDimensions();
					break;
				case TreeReportingViewModel.NodeType.ReportsRoot:
						LoadReports();
					break;
			}
		}

		/// <summary>
		///		Carga los orígenes de datos
		/// </summary>
		private void LoadDataSources()
		{
			DataWarehouseModel dataWarehouse = GetDataWarehouse();

				if (dataWarehouse != null)
					foreach (BaseDataSourceModel dataSource in dataWarehouse.DataSources.EnumerateValuesSorted())
						Children.Add(new NodeDataSourceViewModel(TreeViewModel, this, dataSource));
		}

		/// <summary>
		///		Carga las dimensiones
		/// </summary>
		private void LoadDimensions()
		{
			DataWarehouseModel dataWarehouse = GetDataWarehouse();

				if (dataWarehouse != null)
					foreach (DimensionModel dimension in dataWarehouse.Dimensions.EnumerateValuesSorted())
						Children.Add(new NodeDimensionViewModel(TreeViewModel, this, dimension));
		}

		/// <summary>
		///		Carga los informes
		/// </summary>
		private void LoadReports()
		{
			DataWarehouseModel dataWarehouse = GetDataWarehouse();

				if (dataWarehouse != null)
					foreach (ReportModel report in dataWarehouse.Reports.EnumerateValuesSorted())
						Children.Add(new NodeReportViewModel(TreeViewModel, this, report));
		}

		/// <summary>
		///		Obtiene el dataWarehouse padre
		/// </summary>
		private DataWarehouseModel GetDataWarehouse()
		{
			if (Parent is NodeDataWarehouseViewModel nodeDataWarehouse)
				return nodeDataWarehouse.DataWarehouse;
			else
				return null;
		}

		/// <summary>
		///		Tipo de nodo
		/// </summary>
		private TreeReportingViewModel.NodeType NodeType
		{
			get { return Type.GetEnum(TreeReportingViewModel.NodeType.Unknown); }
		}

		/// <summary>
		///		Tipo de icono
		/// </summary>
		private TreeReportingViewModel.IconType IconType
		{
			get { return Icon.GetEnum(TreeReportingViewModel.IconType.Unknown); }
		}
	}
}
