using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo raíz (raíz de conexiones, raíz de distribución, ...)
	/// </summary>
	public class NodeRootViewModel : BaseTreeNodeViewModel
	{
		public NodeRootViewModel(TreeSolutionBaseViewModel trvTree, IHierarchicalViewModel parent, NodeType type, string text, bool lazyLoad = true) :
					base(trvTree, parent, text, type, IconType.Connection, type, lazyLoad, true, MvvmColor.Red)
		{
			switch (type)
			{
				case NodeType.ConnectionRoot:
						Icon = IconType.Connection;
					break;
				case NodeType.SchemaRoot:
						Icon = IconType.Schema;
					break;
				case NodeType.DeploymentRoot:
						Icon = IconType.Deployment;
					break;
			}
			PropertyChanged += (sender, args) => {
													if (args.PropertyName.Equals(nameof(IsChecked), StringComparison.CurrentCultureIgnoreCase))
														CheckChildNodes();
												 };
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
			(TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.Solution.Connections.SortByName();
			// Añade los nodos
			foreach (Models.Connections.ConnectionModel connection in (TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.Solution.Connections)
				Children.Add(new NodeConnectionViewModel((TreeViewModel as TreeSolutionBaseViewModel), this, connection));
		}

		/// <summary>
		///		Carga los nodos de distribución
		/// </summary>
		private void LoadDeploymentNodes()
		{
			// Ordena las distribuciones
			(TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.Solution.Deployments.SortByName();
			// Añade los nodos
			foreach (Models.Deployments.DeploymentModel deployment in (TreeViewModel as TreeSolutionBaseViewModel).SolutionViewModel.Solution.Deployments)
				Children.Add(new NodeDeploymentViewModel(TreeViewModel as TreeSolutionBaseViewModel, this, deployment));
		}

		/// <summary>
		///		Selecciona los nodos hijo
		/// </summary>
		private void CheckChildNodes()
		{
			if (Type == NodeType.SchemaRoot)
				foreach (BaseTreeNodeViewModel node in Children)
					node.IsChecked = IsChecked;
		}
	}
}
