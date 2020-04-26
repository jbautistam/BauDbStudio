using System;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers
{
	/// <summary>
	///		ViewModel de un mensaje sobre el árbol
	/// </summary>
	public class NodeMessageViewModel : BaseTreeNodeViewModel
	{
		public NodeMessageViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, string message) : 
					base(trvTree, parent, message, NodeType.Table, IconType.Error, null, false, true, BauMvvm.ViewModels.Media.MvvmColor.Red)
		{
		}

		/// <summary>
		///		Carga los nodos de la tabla
		/// </summary>
		protected override void LoadNodes()
		{
			// No hace nada, sólo implementa la interface
		}
	}
}
