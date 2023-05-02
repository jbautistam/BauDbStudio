using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.ToDoManager.Application.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader.Explorer;

/// <summary>
///		Nodo del árbol <see cref="FolderModel"/>
/// </summary>
public class FolderNodeViewModel : BaseNodeViewModel
{
	public FolderNodeViewModel(PluginTreeViewModel trvTree, ToDoFileViewModel mainViewModel, FolderNodeViewModel parent, FolderModel folder) 
				: base(trvTree, parent, folder.Name, false)
	{
		MainViewModel = mainViewModel;
		Folder = folder;
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Navy;
	}

	/// <summary>
	///		Obtiene los nodos hijo
	/// </summary>
	protected override async Task<List<PluginNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		List<PluginNodeViewModel> nodes = new();

			// Evita el warning de async
			await Task.Delay(1);
			// Carga las carpetas hija
			Folder.Folders.SortByName();
			foreach (FolderModel folder in Folder.Folders)
				nodes.Add(new FolderNodeViewModel(TreeViewModel, MainViewModel, this, folder));
			// Carga las entradas hijas
			Folder.Groups.SortByName();
			foreach (GroupModel group in Folder.Groups)
				nodes.Add(new GroupNodeViewModel(TreeViewModel, MainViewModel, this, group));
			// Devuelve la lista de nodos
			return nodes;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoFileViewModel MainViewModel { get; }

	/// <summary>
	///		Carpeta
	/// </summary>
	public FolderModel Folder { get; }
}
