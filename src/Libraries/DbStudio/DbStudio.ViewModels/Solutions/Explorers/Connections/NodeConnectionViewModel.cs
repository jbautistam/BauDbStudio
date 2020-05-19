using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de conexión
	/// </summary>
	public class NodeConnectionViewModel : BaseTreeNodeAsyncViewModel
	{
		public NodeConnectionViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, ConnectionModel connection) : 
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
					// Carga el esquema
					await TreeViewModel.SolutionViewModel.MainViewModel.Manager.LoadSchemaAsync(Connection, cancellationToken);
					// Mete las tablas en la lista
					foreach (ConnectionTableModel table in Connection.Tables)
						nodes.Add(new NodeTableViewModel(TreeViewModel, this, table));
				}
				catch (Exception exception)
				{
					nodes.Add(new NodeMessageViewModel(TreeViewModel, this, "No se puede cargar el esquema de la conexión", IconType.Error));
					System.Diagnostics.Trace.TraceError($"Error when load schema {exception.Message}");
					TreeViewModel.SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error("Error when load schema", exception);
					TreeViewModel.SolutionViewModel.MainViewModel.MainController.Logger.Flush();
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
