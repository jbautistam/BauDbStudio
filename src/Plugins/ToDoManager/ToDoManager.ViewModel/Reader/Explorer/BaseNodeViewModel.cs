using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader.Explorer;

/// <summary>
///		Clase base para los nodos del árbol <see cref="TreeTasksViewModel"/>
/// </summary>
public abstract class BaseNodeViewModel : BaseTreeNodeAsyncViewModel
{
	public BaseNodeViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, string text, bool lazyLoadChildren = true)
							: base(trvTree, parent, text, "Node", string.Empty, null, lazyLoadChildren)
	{
	}
}
