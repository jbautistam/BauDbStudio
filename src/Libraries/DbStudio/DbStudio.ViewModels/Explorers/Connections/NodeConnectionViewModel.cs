using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de conexión
	/// </summary>
	public class NodeConnectionViewModel : BaseTreeNodeAsyncViewModel
	{
		public NodeConnectionViewModel(TreeSolutionBaseViewModel trvTree, IHierarchicalViewModel parent, ConnectionModel connection) : 
					base(trvTree, parent, connection.Name, NodeType.Connection, IconType.Connection, connection, true, true, MvvmColor.Red)
		{
			Connection = connection;
		}

		/// <summary>
		///		Carga los nodos de forma asíncrona
		/// </summary>
		protected override async Task<List<BaseTreeNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
		{
			List<BaseTreeNodeViewModel> nodes = new List<BaseTreeNodeViewModel>();

				// Carga el esquema de las conexiones
				try
				{
					NodeRootViewModel rootTables = new NodeRootViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, NodeType.SchemaRoot, "Tables", false);
					NodeRootViewModel rootViews = new NodeRootViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, NodeType.SchemaRoot, "Views", false);

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
					nodes.Add(new NodeMessageViewModel(TreeViewModel, this, "No se puede cargar el esquema de la conexión", IconType.Error));
					(TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.MainController.MainWindowController.Logger.Default.LogItems.Error("Error when load schema", exception);
					(TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.MainController.Logger.Flush();
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
