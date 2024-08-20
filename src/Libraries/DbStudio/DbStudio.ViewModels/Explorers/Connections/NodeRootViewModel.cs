using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

/// <summary>
///		ViewModel de un nodo raíz (raíz de conexiones, raíz de distribución, ...)
/// </summary>
public class NodeRootViewModel : PluginNodeViewModel
{
	public NodeRootViewModel(TreeSolutionBaseViewModel trvTree, ControlHierarchicalViewModel? parent, 
							 TreeConnectionsViewModel.NodeType type, string text, bool lazyLoad = true) :
				base(trvTree, parent, text, type.ToString(), TreeConnectionsViewModel.IconType.Connection.ToString(), type, lazyLoad, true, MvvmColor.Red)
	{
		switch (type)
		{
			case TreeConnectionsViewModel.NodeType.ConnectionRoot:
					Icon = TreeConnectionsViewModel.IconType.Connection.ToString();
				break;
			case TreeConnectionsViewModel.NodeType.SchemaRoot:
					Icon = TreeConnectionsViewModel.IconType.Schema.ToString();
				break;
		}
		PropertyChanged += (sender, args) => {
												if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
														args.PropertyName.Equals(nameof(IsChecked), StringComparison.CurrentCultureIgnoreCase))
													CheckChildNodes();
											 };
	}

	/// <summary>
	///		Carga los nodos del tipo
	/// </summary>
	protected override void LoadNodes()
	{
		switch (NodeType)
		{
			case TreeConnectionsViewModel.NodeType.ConnectionRoot:
					LoadConnectionNodes();
				break;
		}
	}

	/// <summary>
	///		Carga los nodos de conexión
	/// </summary>
	private void LoadConnectionNodes()
	{
		if (TreeViewModel is TreeSolutionBaseViewModel tree)
		{
			// Ordena las conexiones
			tree.SolutionViewModel.Solution.Connections.SortByName();
			// Añade los nodos
			foreach (Models.Connections.ConnectionModel connection in tree.SolutionViewModel.Solution.Connections)
				Children.Add(new NodeConnectionViewModel(tree, this, connection));
		}
	}

	/// <summary>
	///		Selecciona los nodos hijo
	/// </summary>
	private void CheckChildNodes()
	{
		if (NodeType == TreeConnectionsViewModel.NodeType.SchemaRoot)
			foreach (PluginNodeViewModel node in Children)
				node.IsChecked = IsChecked;
	}

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public TreeConnectionsViewModel.NodeType NodeType => Type.GetEnum(TreeConnectionsViewModel.NodeType.Unknown);
}
