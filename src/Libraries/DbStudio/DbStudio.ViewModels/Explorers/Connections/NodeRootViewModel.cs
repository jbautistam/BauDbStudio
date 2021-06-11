using System;

using Bau.Libraries.LibHelper.Extensors;
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
		public NodeRootViewModel(TreeSolutionBaseViewModel trvTree, IHierarchicalViewModel parent, TreeConnectionsViewModel.NodeType type, string text, bool lazyLoad = true) :
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
				case TreeConnectionsViewModel.NodeType.DeploymentRoot:
						Icon = TreeConnectionsViewModel.IconType.Deployment.ToString();
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
			switch (NodeType)
			{
				case TreeConnectionsViewModel.NodeType.ConnectionRoot:
						LoadConnectionNodes();
					break;
				case TreeConnectionsViewModel.NodeType.DeploymentRoot:
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
			if (NodeType == TreeConnectionsViewModel.NodeType.SchemaRoot)
				foreach (BaseTreeNodeViewModel node in Children)
					node.IsChecked = IsChecked;
		}

		/// <summary>
		///		Tipo de nodo
		/// </summary>
		public TreeConnectionsViewModel.NodeType NodeType
		{
			get { return Type.GetEnum(TreeConnectionsViewModel.NodeType.Unknown); }
		}
	}
}
