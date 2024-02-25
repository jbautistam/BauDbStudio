using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.PasswordManager.Application.Models;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader.Explorer;

/// <summary>
///		Nodo del árbol <see cref="EntryModel"/>
/// </summary>
public class EntryNodeViewModel : BaseNodeEntryViewModel
{
	public EntryNodeViewModel(PluginTreeViewModel trvTree, PasswordFileViewModel mainViewModel, FolderNodeViewModel? parent, EntryModel entry) 
				: base(trvTree, parent, entry.Name, false)
	{
		MainViewModel = mainViewModel;
		Entry = entry;
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
	}

	/// <summary>
	///		Obtiene los nodos, en este caso sólo implementa la interface
	/// </summary>
	protected override async Task<List<PluginNodeViewModel>?> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
		return null;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PasswordFileViewModel MainViewModel { get; }

	/// <summary>
	///		Blog
	/// </summary>
	public EntryModel Entry { get; }
}
