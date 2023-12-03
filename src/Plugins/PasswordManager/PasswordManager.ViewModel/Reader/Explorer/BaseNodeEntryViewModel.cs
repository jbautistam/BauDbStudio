using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader.Explorer;

/// <summary>
///		Clase base para los nodos del árbol <see cref="TreePasswordsViewModel"/>
/// </summary>
public abstract class BaseNodeEntryViewModel : PluginNodeAsyncViewModel
{
	public BaseNodeEntryViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, string text, bool lazyLoadChildren = true)
							: base(trvTree, parent, text, "Node", string.Empty, null, lazyLoadChildren)
	{
	}
}
