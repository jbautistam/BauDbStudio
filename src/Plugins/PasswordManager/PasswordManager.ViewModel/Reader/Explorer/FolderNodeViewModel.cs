using Bau.Libraries.PasswordManager.Application.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader.Explorer;

/// <summary>
///		Nodo del árbol <see cref="FolderModel"/>
/// </summary>
public class FolderNodeViewModel : BaseNodeEntryViewModel
{
	public FolderNodeViewModel(PluginTreeViewModel trvTree, PasswordFileViewModel mainViewModel, FolderNodeViewModel? parent, FolderModel folder) 
				: base(trvTree, parent, folder.Name, false)
	{
		MainViewModel = mainViewModel;
		Folder = folder;
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Navy;
	}

	/// <summary>
	///		Obtiene los nodos hijo
	/// </summary>
	protected override async Task<List<PluginNodeViewModel>?> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		List<PluginNodeViewModel> nodes = new();

			// Evita el warning de async
			await Task.Delay(1);
			// Carga las carpetas hija
			Folder.Folders.SortByName();
			foreach (FolderModel folder in Folder.Folders)
				nodes.Add(new FolderNodeViewModel(TreeViewModel, MainViewModel, this, folder));
			// Carga las entradas hijas
			Folder.Entries.SortByName();
			foreach (EntryModel entry in Folder.Entries)
				nodes.Add(new EntryNodeViewModel(TreeViewModel, MainViewModel, this, entry));
			// Devuelve la lista de nodos
			return nodes;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PasswordFileViewModel MainViewModel { get; }

	/// <summary>
	///		Carpeta
	/// </summary>
	public FolderModel Folder { get; }
}
