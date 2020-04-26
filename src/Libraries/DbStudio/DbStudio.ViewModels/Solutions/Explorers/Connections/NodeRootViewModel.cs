using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo raíz (raíz de conexiones, raíz de distribución, ...)
	/// </summary>
	public class NodeRootViewModel : BaseTreeNodeViewModel
	{
		public NodeRootViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, NodeType type, string text) :
					base(trvTree, parent, text, type, IconType.Connection, type, true, true, MvvmColor.Red)
		{
			switch (type)
			{
				case NodeType.ConnectionRoot:
						Icon = IconType.Connection;
					break;
				case NodeType.DeploymentRoot:
						Icon = IconType.Deployment;
					break;
			}
		}

		/// <summary>
		///		Carga los nodos del tipo
		/// </summary>
		protected override void LoadNodes()
		{
			switch (Type)
			{
				case NodeType.ConnectionRoot:
						LoadConnectionNodes();
					break;
				case NodeType.DeploymentRoot:
						LoadDeploymentNodes();
					break;
			}
		}

		/// <summary>
		///		Carga los nodos de conexión
		/// </summary>
		private void LoadConnectionNodes()
		{
			// Ordena las conexiones
			TreeViewModel.SolutionViewModel.Solution.Connections.SortByName();
			// Añade los nodos
			foreach (Models.Connections.ConnectionModel connection in TreeViewModel.SolutionViewModel.Solution.Connections)
				Children.Add(new NodeConnectionViewModel(TreeViewModel, this, connection));
		}

		/// <summary>
		///		Carga los nodos de distribución
		/// </summary>
		private void LoadDeploymentNodes()
		{
			// Ordena las distribuciones
			TreeViewModel.SolutionViewModel.Solution.Deployments.SortByName();
			// Añade los nodos
			foreach (Models.Deployments.DeploymentModel deployment in TreeViewModel.SolutionViewModel.Solution.Deployments)
				Children.Add(new NodeDeploymentViewModel(TreeViewModel, this, deployment));
		}
	}
}
