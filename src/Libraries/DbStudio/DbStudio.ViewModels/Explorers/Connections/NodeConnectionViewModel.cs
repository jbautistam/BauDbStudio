using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

/// <summary>
///		ViewModel de un nodo de conexión
/// </summary>
public class NodeConnectionViewModel : PluginNodeAsyncViewModel
{
	public NodeConnectionViewModel(TreeSolutionBaseViewModel trvTree, ControlHierarchicalViewModel? parent, ConnectionModel connection) : 
				base(trvTree, parent, connection.Name, TreeConnectionsViewModel.NodeType.Connection.ToString(), TreeConnectionsViewModel.IconType.Connection.ToString(), 
					 connection, true, true, MvvmColor.Red)
	{
		Connection = connection;
	}

	/// <summary>
	///		Carga los nodos de forma asíncrona
	/// </summary>
	protected override async Task<List<PluginNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		List<PluginNodeViewModel> nodes = new();

			// Carga el esquema de las conexiones
			if (TreeViewModel is TreeSolutionBaseViewModel tree)
				try
				{
					NodeRootViewModel rootTables = new(tree, this, TreeConnectionsViewModel.NodeType.SchemaRoot, "Tables", false);
					NodeRootViewModel rootViews = new(tree, this, TreeConnectionsViewModel.NodeType.SchemaRoot, "Views", false);

						// Carga el esquema
						await tree.SolutionViewModel.Manager.LoadSchemaAsync(Connection, tree.SolutionViewModel.ConfigurationViewModel.SeeSystemTables, cancellationToken);
						// Añade los nodos raíz
						nodes.Add(rootTables);
						nodes.Add(rootViews);
						// Ordena las tablas
						Connection.Tables.Sort((first, second) => first.FullName.CompareTo(second.FullName));
						// Añade las tablas al nodo
						foreach (ConnectionTableModel table in Connection.Tables)
							rootTables.Children.Add(new NodeTableViewModel(tree, this, table, true));
						// Ordena las vistas
						Connection.Views.Sort((first, second) => first.FullName.CompareTo(second.FullName));
						// Añade las tablas al nodo
						foreach (ConnectionTableModel view in Connection.Views)
							rootViews.Children.Add(new NodeTableViewModel(tree, this, view, false));
				}
				catch (Exception exception)
				{
					nodes.Add(new PluginNodeMessageViewModel(TreeViewModel, this, "No se puede cargar el esquema de la conexión", 
															 TreeConnectionsViewModel.IconType.Error.ToString()));
					tree.SolutionViewModel.MainController.MainWindowController.Logger
							.LogError(exception, "Error when load schema");
				}
			// Devuelve la colección de nodos
			return nodes;
	}

	/// <summary>
	///		Conexión asociada al nodo
	/// </summary>
	public ConnectionModel Connection { get; }
}
