using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.ToDoManager.Application.ToDo.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader.Explorer;

/// <summary>
///		Nodo del árbol <see cref="GroupModel"/>
/// </summary>
public class GroupNodeViewModel : TodoBaseNodeViewModel
{
	public GroupNodeViewModel(PluginTreeViewModel trvTree, ToDoFileViewModel mainViewModel, FolderNodeViewModel? parent, GroupModel group) 
				: base(trvTree, parent, group.Name, false)
	{
		MainViewModel = mainViewModel;
		Group = group;
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
	}

	/// <summary>
	///		Obtiene los nodos, en este caso sólo implementa la interface
	/// </summary>
	protected override async Task<List<PluginNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1);
		return new();
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
