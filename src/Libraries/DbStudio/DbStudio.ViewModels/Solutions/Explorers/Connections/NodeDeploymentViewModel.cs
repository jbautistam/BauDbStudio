using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.DbStudio.Models.Deployments;
using Bau.Libraries.DbStudio.ViewModels.Core.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections
{
	/// <summary>
	///		ViewModel de un nodo de distribución
	/// </summary>
	public class NodeDeploymentViewModel : BaseTreeNodeViewModel
	{
		public NodeDeploymentViewModel(TreeSolutionBaseViewModel trvTree, IHierarchicalViewModel parent, DeploymentModel deployment) : 
					base(trvTree, parent, deployment.Name, NodeType.Deployment, IconType.Deployment, deployment, false, false, MvvmColor.Black)
		{
			Deployment = deployment;
		}

		/// <summary>
		///		Carga los nodos de la distribución
		/// </summary>
		protected override void LoadNodes()
		{
			// en este caso simplemente implementa la interface
		}

		/// <summary>
		///		Distribución asociada al nodo
		/// </summary>
		public DeploymentModel Deployment { get; }
	}
}
