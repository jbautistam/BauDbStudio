using System;
using System.Collections.Generic;
using System.Linq;
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
					List<string> schemas;

						// Carga el esquema
						await TreeViewModel.SolutionViewModel.MainViewModel.Manager.LoadSchemaAsync(Connection, cancellationToken);
						// Obtiene los esquemas
						schemas = GetSchemas(Connection);
						// Mete las tablas en la lista
						if (schemas.Count == 0)
							foreach (ConnectionTableModel table in Connection.Tables)
								nodes.Add(new NodeTableViewModel(TreeViewModel, this, table));
						else
						{
							// Ordena los esquemas
							schemas.Sort((first, second) => first.CompareTo(second));
							// Añade los nodos de esquema
							foreach (string schema in schemas)
							{
								NodeRootViewModel root = new NodeRootViewModel(TreeViewModel, this, NodeType.SchemaRoot, schema, false);

									// Añade las tablas al nodo
									foreach (ConnectionTableModel table in Connection.Tables)
										if (table.Schema.Equals(schema, StringComparison.CurrentCultureIgnoreCase))
											root.Children.Add(new NodeTableViewModel(TreeViewModel, this, table));
									// Añade el nodo de esquema a la raíz
									nodes.Add(root);
							}
						}
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
		///		Obtiene los esquemas asociados a una conexión
		/// </summary>
		private List<string> GetSchemas(ConnectionModel connection)
		{
			List<string> schemas = new List<string>();

				// Añade los esquemas de las tablas
				foreach (ConnectionTableModel table in connection.Tables)
					if (!string.IsNullOrWhiteSpace(table.Schema) && 
							schemas.FirstOrDefault(item => item.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase)) == null)
						schemas.Add(table.Schema);
				// Devuelve la lista de esquemas
				return schemas;
		}

		/// <summary>
		///		Conexión asociada al nodo
		/// </summary>
		public ConnectionModel Connection { get; }
	}
}
