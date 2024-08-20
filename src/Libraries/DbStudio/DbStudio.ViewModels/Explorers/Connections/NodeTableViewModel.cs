using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

/// <summary>
///		ViewModel de un nodo de tabla
/// </summary>
public class NodeTableViewModel : PluginNodeViewModel
{
	public NodeTableViewModel(TreeSolutionBaseViewModel trvTree, NodeConnectionViewModel parent, ConnectionTableModel table, bool isTable) : 
				base(trvTree, parent, table.FullName, TreeConnectionsViewModel.NodeType.Table.ToString(), 
					 TreeConnectionsViewModel.IconType.Table.ToString(), table, true, true, BauMvvm.ViewModels.Media.MvvmColor.Navy)
	{
		Table = table;
		IsTable = isTable;
		if (!IsTable)
			Icon = TreeConnectionsViewModel.IconType.View.ToString();
		if (table.IsSystem)
			Foreground = BauMvvm.ViewModels.Media.MvvmColor.Blue;
	}

	/// <summary>
	///		Obtiene el texto que se puede insertar en un editor con los datos de la tabla
	/// </summary>
	public override string GetTextForEditor(bool shiftPressed)
	{
		if (TreeViewModel is TreeConnectionsViewModel trvTree)
			return trvTree.GetSqlSelectText(this, shiftPressed);
		else
			return string.Empty;
	}

	/// <summary>
	///		Carga los nodos de la tabla
	/// </summary>
	protected override void LoadNodes()
	{
		if (TreeViewModel is TreeSolutionBaseViewModel tree)
			foreach (ConnectionTableFieldModel field in Table.Fields)
				Children.Add(new NodeTableFieldViewModel(tree, this, field));
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
