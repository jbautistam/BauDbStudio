using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader.Explorer;

/// <summary>
///		Clase base para los nodos del árbol <see cref="TreeTasksViewModel"/>
/// </summary>
public abstract class BaseNodeViewModel : BaseTreeNodeAsyncViewModel
{
	public BaseNodeViewModel(BaseTreeViewModel trvTree, ControlHierarchicalViewModel parent, string text, bool lazyLoadChildren = true)
							: base(trvTree, parent, text, "Node", string.Empty, null, lazyLoadChildren)
	{
	}
}
