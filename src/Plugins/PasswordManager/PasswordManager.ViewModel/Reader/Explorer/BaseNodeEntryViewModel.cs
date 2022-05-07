using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.PasswordManager.Application.Models;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader.Explorer
{
	/// <summary>
	///		Clase base para los nodos del árbol <see cref="TreePasswordsViewModel"/>
	/// </summary>
	public abstract class BaseNodeEntryViewModel : BaseTreeNodeAsyncViewModel
	{
		public BaseNodeEntryViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, string text, bool lazyLoadChildren = true)
								: base(trvTree, parent, text, "Node", string.Empty, null, lazyLoadChildren)
		{
		}
	}
}
