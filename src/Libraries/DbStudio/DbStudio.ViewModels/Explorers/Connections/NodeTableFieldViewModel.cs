using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

/// <summary>
///		ViewModel de un nodo de un campo de una tabla
/// </summary>
public class NodeTableFieldViewModel : PluginNodeViewModel
{
	public NodeTableFieldViewModel(TreeSolutionBaseViewModel trvTree, NodeTableViewModel parent, ConnectionTableFieldModel field) : 
				base(trvTree, parent, field.FullName, TreeConnectionsViewModel.NodeType.Table.ToString(), 
					 TreeConnectionsViewModel.IconType.Field.ToString(), field, false)
	{
		Field = field;
		if (field.IsKey)
			Icon = TreeConnectionsViewModel.IconType.Key.ToString();
	}

	/// <summary>
	///		Carga los nodos del campo
	/// </summary>
	protected override void LoadNodes()
	{
		// No hace nada, simplemente implementa la clase
	}

	/// <summary>
	///		Obtiene el texto de la cadena SQL asociada al campo
	/// </summary>
	public override string GetTextForEditor(bool shiftPressed)
	{
		if (TreeViewModel is TreeConnectionsViewModel trvTree)
			return trvTree.GetSqlSelect(this, shiftPressed);
		else
			return string.Empty;
	}

	/// <summary>
	///		Campo asociado al nodo
	/// </summary>
	public ConnectionTableFieldModel Field { get; }
}
