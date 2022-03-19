using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.JobsProcessor.ViewModel.Processor.LogTree
{
	/// <summary>
	///		Clase base para los nodos del árbol <see cref="TreeLogViewModel"/>
	/// </summary>
	public class LogNodeViewModel : BaseTreeNodeViewModel
	{
		public LogNodeViewModel(TreeLogViewModel trvTree, IHierarchicalViewModel parent, string text, TreeLogViewModel.NodeType type, object tag)
						: base(trvTree, parent, text, type.ToString(), string.Empty, tag, false, type == TreeLogViewModel.NodeType.Context)
		{
			switch (type)
			{
				case TreeLogViewModel.NodeType.Context:
						Foreground = BauMvvm.ViewModels.Media.MvvmColor.Navy;
					break;
				case TreeLogViewModel.NodeType.Command:
						Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
					break;
			}
			trvTree.RaiseEvent(type, text);
		}

		/// <summary>
		///		Carga los nodos
		/// </summary>
		protected override void LoadNodes()
		{
			// en este caso no hace nada, simplemente implementa la interface
		}

		/// <summary>
		///		Tipo de nodo
		/// </summary>
		public TreeLogViewModel.NodeType NodeType
		{
			get { return Type.GetEnum(TreeLogViewModel.NodeType.Unknown); }
		}
	}
}
