using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.ToDoManager.Application.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader.Explorer;

/// <summary>
///		Nodo del árbol <see cref="GroupModel"/>
/// </summary>
public class GroupNodeViewModel : BaseNodeViewModel
{
	public GroupNodeViewModel(BaseTreeViewModel trvTree, ToDoFileViewModel mainViewModel, FolderNodeViewModel parent, GroupModel group) 
				: base(trvTree, parent, group.Name, false)
	{
		MainViewModel = mainViewModel;
		Group = group;
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
	}

	/// <summary>
	///		Obtiene los nodos, en este caso sólo implementa la interface
	/// </summary>
	protected override async Task<List<BaseTreeNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1);
		return null;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoFileViewModel MainViewModel { get; }

	/// <summary>
	///		Blog
	/// </summary>
	public GroupModel Group { get; }
}
