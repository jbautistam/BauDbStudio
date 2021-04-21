using System;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de tabla
	/// </summary>
	public class NodeTableViewModel : BaseTreeNodeViewModel
	{
		public NodeTableViewModel(TreeSolutionBaseViewModel trvTree, NodeConnectionViewModel parent, ConnectionTableModel table, bool isTable) : 
					base(trvTree, parent, table.FullName, NodeType.Table, IconType.Table, table, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
		{
			Table = table;
			IsTable = isTable;
			if (!IsTable)
				Icon = IconType.View;
		}

		/// <summary>
		///		Obtiene el texto de la cadena SQL asociada a la tabla
		/// </summary>
		public string GetSqlSelect(bool fullSql)
		{
			if (TreeViewModel is TreeConnectionsViewModel trvTree)
				return trvTree.GetSqlSelectText(this, fullSql);
			else
				return string.Empty;
		}

		/// <summary>
		///		Carga los nodos de la tabla
		/// </summary>
		protected override void LoadNodes()
		{
			foreach (ConnectionTableFieldModel field in Table.Fields)
				Children.Add(new NodeTableFieldViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, field));
		}

		/// <summary>
		///		Tabla asociada al nodo
		/// </summary>
		public ConnectionTableModel Table { get; }

		/// <summary>
		///		Indica si es una tabla o una vista
		/// </summary>
		public bool IsTable { get; }
	}
}
