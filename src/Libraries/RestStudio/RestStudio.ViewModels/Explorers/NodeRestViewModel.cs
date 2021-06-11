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
				case null:
						switch (type)
						{
							case TreeRestApiViewModel.NodeType.ContextsRoot:
									Icon = TreeRestApiViewModel.IconType.ContextsRoot.ToString();
								break;
							case TreeRestApiViewModel.NodeType.MethodsRoot:
									Icon = TreeRestApiViewModel.IconType.MethodsRoot.ToString();
								break;
						}
					break;
				case RestApiModel:
						Type = TreeRestApiViewModel.NodeType.RestApi.ToString();
						Icon = TreeRestApiViewModel.IconType.RestApi.ToString();
					break;
				case ContextModel:
						Type = TreeRestApiViewModel.NodeType.Context.ToString();
						Icon = TreeRestApiViewModel.IconType.Context.ToString();
						LazyLoad = false;
						Children.Clear();
					break;
				case MethodModel:
						Type = TreeRestApiViewModel.NodeType.Method.ToString();
						Icon = TreeRestApiViewModel.IconType.Method.ToString();
						LazyLoad = false;
						Children.Clear();
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
						LoadContextNodes();
					break;
				case TreeRestApiViewModel.NodeType.MethodsRoot:
						LoadMethodNodes();
					break;
			}
		}

		/// <summary>
		///		Añade los nodos raíz
		/// </summary>
		private void LoadRootNodes()
		{
			Children.Add(new NodeRestViewModel(TreeViewModel as TreeRestApiViewModel, this, "Contextos", TreeRestApiViewModel.NodeType.ContextsRoot, null, 
											   true, BauMvvm.ViewModels.Media.MvvmColor.Red));
			Children.Add(new NodeRestViewModel(TreeViewModel as TreeRestApiViewModel, this, "Métodos", TreeRestApiViewModel.NodeType.MethodsRoot, null, 
											   true, BauMvvm.ViewModels.Media.MvvmColor.Red));
		}

		/// <summary>
		///		Carga los nodos de contexto
		/// </summary>
		private void LoadContextNodes()
		{
			RestApiModel restApi = GetRestParent();

				// Carga los nodos
				if (restApi != null)
					foreach (ContextModel context in restApi.Contexts)
						Children.Add(new NodeRestViewModel(TreeViewModel as TreeRestApiViewModel, this, context.Name, TreeRestApiViewModel.NodeType.Context, context,
														   false));
		}

		/// <summary>
		///		Carga los nodos de métods
		/// </summary>
		private void LoadMethodNodes()
		{
			RestApiModel restApi = GetRestParent();

				// Carga los nodos
				if (restApi != null)
					foreach (MethodModel method in restApi.Methods)
						Children.Add(new NodeRestViewModel(TreeViewModel as TreeRestApiViewModel, this, method.Name, TreeRestApiViewModel.NodeType.Method, method,
														   false));
		}

		/// <summary>
		///		Obtiene el <see cref="RestApiModel"/> padre de este nodo
		/// </summary>
		internal RestApiModel GetRestParent()
		{
			RestApiModel GetRestParent(NodeRestViewModel node)
			{
				if (node == null)
					return null;
				else if (node.Tag is RestApiModel rest)
					return rest;
				else
					return GetRestParent(node.Parent as NodeRestViewModel);
			}

				// Obtiene el nodo padre de este nodo
				return GetRestParent(this);
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
