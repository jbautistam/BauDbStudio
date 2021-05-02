using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de origen de datos
	/// </summary>
	public class NodeDataSourceViewModel : BaseTreeNodeViewModel
	{
		public NodeDataSourceViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, BaseDataSourceModel dataSource) : 
					base(trvTree, parent, dataSource.Id, TreeReportingViewModel.NodeType.DataSource.ToString(), TreeReportingViewModel.IconType.Table.ToString(), 
						 dataSource, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
		{
			DataSource = dataSource;
			if (dataSource is DataSourceSqlModel)
				Icon = TreeReportingViewModel.IconType.DataSourceSql.ToString();
		}

		/// <summary>
		///		Carga los nodos de la tabla
		/// </summary>
		protected override void LoadNodes()
		{
			foreach (DataSourceColumnModel column in DataSource.Columns.EnumerateValuesSorted())
				Children.Add(new NodeDataSourceColumnViewModel(TreeViewModel, this, column));
		}

		/// <summary>
		///		Tabla asociada al nodo
		/// </summary>
		public BaseDataSourceModel DataSource { get; }
	}
}
