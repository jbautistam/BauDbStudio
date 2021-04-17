using System;

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
					base(trvTree, parent, restApi.Name, NodeType.Connection, IconType.Connection, restApi, true, true, MvvmColor.Red)
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
	}
}
