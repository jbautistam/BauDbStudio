using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Microsoft.Extensions.Logging;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de conexión
	/// </summary>
	public class NodeConnectionViewModel : PluginNodeAsyncViewModel
	{
		public NodeConnectionViewModel(TreeSolutionBaseViewModel trvTree, ControlHierarchicalViewModel parent, ConnectionModel connection) : 
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
			List<PluginNodeViewModel> nodes = new List<PluginNodeViewModel>();

				// Carga el esquema de las conexiones
				try
				{
					NodeRootViewModel rootTables = new NodeRootViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, TreeConnectionsViewModel.NodeType.SchemaRoot, "Tables", false);
					NodeRootViewModel rootViews = new NodeRootViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, TreeConnectionsViewModel.NodeType.SchemaRoot, "Views", false);

						// Carga el esquema
						await (TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.Manager.LoadSchemaAsync(Connection, cancellationToken);
						// Añade los nodos raíz
						nodes.Add(rootTables);
						nodes.Add(rootViews);
						// Ordena las tablas
						Connection.Tables.Sort((first, second) => first.FullName.CompareTo(second.FullName));
						// Añade las tablas al nodo
						foreach (ConnectionTableModel table in Connection.Tables)
							rootTables.Children.Add(new NodeTableViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, table, true));
						// Ordena las vistas
						Connection.Views.Sort((first, second) => first.FullName.CompareTo(second.FullName));
						// Añade las tablas al nodo
						foreach (ConnectionTableModel view in Connection.Views)
							rootViews.Children.Add(new NodeTableViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, view, false));
				}
				catch (Exception exception)
				{
					nodes.Add(new PluginNodeMessageViewModel(TreeViewModel, this, "No se puede cargar el esquema de la conexión", TreeConnectionsViewModel.IconType.Error.ToString()));
					(TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.MainController.MainWindowController.Logger
							.LogError(exception, "Error when load schema", exception);
				}
				// Devuelve la colección de nodos
				return nodes;
		}

		/// <summary>
		///		Conexión asociada al nodo
		/// </summary>
		public ConnectionModel Connection { get; }
	}
}
