using System;

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
		public NodeRootViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, NodeType type, string text, bool lazyLoad = true, bool bold = true, MvvmColor color = null) :
					base(trvTree, parent, text, type, IconType.Connection, type, lazyLoad, bold, color ?? MvvmColor.Red)
		{
			switch (type)
			{
				case NodeType.DimensionsRoot:
						Icon = IconType.Dimension;
					break;
				case NodeType.DataSourcesRoot:
						Icon = IconType.Schema;
					break;
				case NodeType.ReportsRoot:
						Icon = IconType.Report;
					break;
				case NodeType.File:
						Icon = IconType.Field;
					break;
				case NodeType.Table:
						Icon = IconType.Path;
					break;
			}
		}

		/// <summary>
		///		Carga los nodos del tipo
		/// </summary>
		protected override void LoadNodes()
		{
			switch (Type)
			{
				case NodeType.DataSourcesRoot:
						LoadDataSources();
					break;
				case NodeType.DimensionsRoot:
						LoadDimensions();
					break;
				case NodeType.ReportsRoot:
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
	}
}
