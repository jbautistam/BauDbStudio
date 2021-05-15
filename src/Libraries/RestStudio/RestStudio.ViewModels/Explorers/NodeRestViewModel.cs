using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.RestStudio.Models.Rest;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.RestStudio.ViewModels.Explorers
{
	/// <summary>
	///		ViewModel base de un nodo de API Rest
	/// </summary>
	public class NodeRestViewModel : BaseTreeNodeViewModel
	{
		public NodeRestViewModel(TreeRestApiViewModel trvTree, IHierarchicalViewModel parent, string text, 
								 TreeRestApiViewModel.NodeType type, object tag, bool bold, BauMvvm.ViewModels.Media.MvvmColor color = null) : 
					base(trvTree, parent, text, type.ToString(), null, tag, true, bold, color)
		{
			switch (tag)
			{
				case RestModel:
						Type = TreeRestApiViewModel.NodeType.RestApi.ToString();
					break;
				case ContextModel:
						Type = TreeRestApiViewModel.NodeType.Context.ToString();
						LazyLoad = false;
					break;
				case MethodModel:
						Type = TreeRestApiViewModel.NodeType.Method.ToString();
						LazyLoad = false;
					break;
			}
		}

		/// <summary>
		///		Carga los nodos
		/// </summary>
		protected override void LoadNodes()
		{
			switch (NodeType)
			{
				case TreeRestApiViewModel.NodeType.RestApi:
						LoadRootNodes();
					break;
				case TreeRestApiViewModel.NodeType.ContextsRoot:
					break;
				case TreeRestApiViewModel.NodeType.MethodsRoot:
					break;
			}
		}

		/// <summary>
		///		Añade los nodos raíz
		/// </summary>
		private void LoadRootNodes()
		{
			Children.Add(new NodeRestViewModel(TreeViewModel as TreeRestApiViewModel, null, "Contextos", TreeRestApiViewModel.NodeType.ContextsRoot, null, 
											   true, BauMvvm.ViewModels.Media.MvvmColor.Red));
			Children.Add(new NodeRestViewModel(TreeViewModel as TreeRestApiViewModel, null, "Métodos", TreeRestApiViewModel.NodeType.MethodsRoot, null, 
											   true, BauMvvm.ViewModels.Media.MvvmColor.Red));
		}

		/// <summary>
		///		Tipo de nodo
		/// </summary>
		public TreeRestApiViewModel.NodeType NodeType
		{
			get { return Type.GetEnum(TreeRestApiViewModel.NodeType.Unknown); }
		}

		/// <summary>
		///		Tipo de icono
		/// </summary>
		public TreeRestApiViewModel.IconType IconType
		{
			get { return Icon.GetEnum(TreeRestApiViewModel.IconType.Unknown); }
		}
	}
}
