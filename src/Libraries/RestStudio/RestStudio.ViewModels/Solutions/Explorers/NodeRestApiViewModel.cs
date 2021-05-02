using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.RestStudio.Models.Rest;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.RestStudio.ViewModels.Solutions.Explorers
{
	/// <summary>
	///		ViewModel de un nodo de API Rest
	/// </summary>
	public class NodeRestApiViewModel : BaseTreeNodeViewModel
	{
		public NodeRestApiViewModel(TreeRestApiViewModel trvTree, IHierarchicalViewModel parent, RestModel restApi) : 
					base(trvTree, parent, restApi.Name, TreeRestApiViewModel.NodeType.Unknown.ToString(), TreeRestApiViewModel.IconType.Unknown.ToString(), restApi, true, true, MvvmColor.Red)
		{
			RestApi = restApi;
		}

		/// <summary>
		///		Carga los nodos de forma asíncrona
		/// </summary>
		protected override void LoadNodes()
		{
		}

		/// <summary>
		///		Api Rest asociada al nodo
		/// </summary>
		public RestModel RestApi { get; }

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
