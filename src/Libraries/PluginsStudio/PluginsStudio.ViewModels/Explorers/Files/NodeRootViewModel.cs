using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel de un nodo raíz de la solución
/// </summary>
public class NodeRootViewModel : PluginNodeViewModel
{
	public NodeRootViewModel(TreeFilesViewModel trvTree, TreeFilesViewModel.NodeType nodeType) 
				: base(trvTree, null, nodeType.ToString(), TreeFilesViewModel.NodeType.FilesRoot.ToString(), string.Empty, null, true, true, MvvmColor.Green)
	{
		ViewModel = trvTree;
		NodeType = nodeType;
		switch (NodeType)
		{
			case TreeFilesViewModel.NodeType.ComputerRoot:
					Text = "Computer";
				break;
			case TreeFilesViewModel.NodeType.BookmarksRoot:
					Text = "Bookmarks";
				break;
		}
	}

	/// <summary>
	///		Carga los nodos hijo
	/// </summary>
	protected override void LoadNodes()
	{
		switch (NodeType)
		{
			case TreeFilesViewModel.NodeType.ComputerRoot:
					LoadNodesComputer();
				break;
			case TreeFilesViewModel.NodeType.BookmarksRoot:
					LoadNodesBookmark();
				break;
		}
	}

	/// <summary>
	///		Carga las unidades del ordenador
	/// </summary>
	private void LoadNodesComputer()
	{
		List<string> paths = [];

			// Carga las unidades en la lista de directorios
			foreach (DriveInfo drive in DriveInfo.GetDrives())
				paths.Add(drive.Name);
			// Añade los nodos de la lista de directorios
			LoadNodesPaths(paths);
	}

	/// <summary>
	///		Carga las unidades del marcador
	/// </summary>
	private void LoadNodesBookmark()
	{
		List<string> paths = [];

			// Añade los directorios
			if (ViewModel.MainViewModel.WorkspacesViewModel.SelectedItem != null)
				foreach (string path in ViewModel.MainViewModel.WorkspacesViewModel.SelectedItem.Folders)
					if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
						paths.Add(path);
			// Ordena por el nombre del directorio
			paths.Sort((first, second) => Path.GetFileName(first).CompareTo(Path.GetFileName(second)));
			// Recarga los nodos
			foreach (string path in paths)
				Children.Add(new NodeFolderRootViewModel(ViewModel, this, path));
	}

	/// <summary>
	///		Carga los nodos de una lista de directorios
	/// </summary>
	private void LoadNodesPaths(List<string> paths)
	{
		// Ordena por el nombre del directorio
		paths.Sort((first, second) => Path.GetFileName(first).CompareTo(Path.GetFileName(second)));
		// Añade los nodos
		foreach (string path in paths)
			Children.Add(new NodeFolderRootViewModel(ViewModel, this, path));
	}

	/// <summary>
	///		Comprueba si este nodo es igual a otro
	/// </summary>
	public override bool IsEquals(ControlHierarchicalViewModel node)
	{
		if (node is NodeRootViewModel target)
			return NodeType == target.NodeType;
		else
			return base.IsEquals(node);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public TreeFilesViewModel ViewModel { get; }

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public TreeFilesViewModel.NodeType NodeType { get; }
}
