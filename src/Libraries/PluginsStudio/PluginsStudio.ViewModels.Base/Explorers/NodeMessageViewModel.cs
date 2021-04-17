using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers
{
	/// <summary>
	///		ViewModel de un mensaje sobre el árbol
	/// </summary>
	public class NodeMessageViewModel : BaseTreeNodeViewModel
	{
		public NodeMessageViewModel(BaseTreeViewModel trvTree, BaseTreeNodeViewModel parent, string message, IconType icon = IconType.Loading) : 
					base(trvTree, parent, message, NodeType.Message, icon, null, false, false, BauMvvm.ViewModels.Media.MvvmColor.DarkGreen)
		{
			if (icon == IconType.Error)
				Foreground = BauMvvm.ViewModels.Media.MvvmColor.Red;
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
