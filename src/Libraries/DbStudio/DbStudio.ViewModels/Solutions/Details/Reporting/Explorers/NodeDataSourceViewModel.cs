using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de origen de datos
	/// </summary>
	public class NodeDataSourceViewModel : BaseTreeNodeViewModel
	{
		public NodeDataSourceViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, BaseDataSourceModel dataSource) : 
					base(trvTree, parent, dataSource.Name, NodeType.DataSource, IconType.Table, dataSource, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
		{
			// Asigna las propiedades
			DataSource = dataSource;
			// Ajusta el nombre y los iconos dependiendo del tipo
			switch (dataSource)
			{
				case DataSourceTableModel table:
						if (string.IsNullOrWhiteSpace(dataSource.Name))
							Text = table.FullName;
					break;
				case DataSourceSqlModel sql:
						Icon = IconType.DataSourceSql;
						if (string.IsNullOrWhiteSpace(dataSource.Name))
							Text = sql.GlobalId;
					break;
			}
		}

		/// <summary>
		///		Carga los nodos de la tabla
		/// </summary>
		protected override void LoadNodes()
		{
			foreach (DataSourceColumnModel column in DataSource.Columns)
				Children.Add(new NodeDataSourceColumnViewModel(TreeViewModel, this, column));
		}

		/// <summary>
		///		Tabla asociada al nodo
		/// </summary>
		public BaseDataSourceModel DataSource { get; }
	}
}
