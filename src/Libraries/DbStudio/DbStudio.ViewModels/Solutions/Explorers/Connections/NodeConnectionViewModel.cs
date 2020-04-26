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
	public class NodeConnectionViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private SynchronizationContext _contextUi = SynchronizationContext.Current;

		public NodeConnectionViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, ConnectionModel connection) : 
					base(trvTree, parent, connection.Name, NodeType.Connection, IconType.Connection, connection, true, true, MvvmColor.Red)
		{
			Connection = connection;
		}

		/// <summary>
		///		Carga los nodos de la conexión
		/// </summary>
		protected override void LoadNodes()
		{
			Children.Add(new NodeMessageViewModel(TreeViewModel, this, "Cargando ..."));
			Task.Run(async () => await LoadNodesAsync(new CancellationToken()));
		}

		/// <summary>
		///		Carga los nodos de forma asíncrona
		/// </summary>
		private async Task LoadNodesAsync(CancellationToken cancellationToken)
		{
			object state = new object();
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
					nodes.Add(new NodeMessageViewModel(TreeViewModel, this, "No se puede cargar el esquema de la conexión"));
					System.Diagnostics.Trace.TraceError($"Error when load schema {exception.Message}");
					TreeViewModel.SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error("Error when load schema", exception);
					TreeViewModel.SolutionViewModel.MainViewModel.MainController.Logger.Flush();
				}
				// Muestra las tablas
				//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
				//? Al generarse las tablas en otro hilo, no se puede añadir a ObservableCollection sin una
				//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
				//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
				_contextUi.Send(_ => {
										// Limpia la lista de nodos
										Children.Clear();
										// Añade la lista de tablas leidas
										foreach (BaseTreeNodeViewModel node in nodes)
											Children.Add(node);
									 }, 
								state);
		}

		/// <summary>
		///		Conexión asociada al nodo
		/// </summary>
		public ConnectionModel Connection { get; }
	}
}
