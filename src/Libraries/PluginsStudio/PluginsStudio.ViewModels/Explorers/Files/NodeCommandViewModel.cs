using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel de un nodo de archivo
/// </summary>
public class NodeCommandViewModel : PluginNodeCommandViewModel
{
	// Enumerados públicos
	public enum CommandType
	{
		/// <summary>Añade los nodos con las unidades</summary>
		AddDriveNodes,
		/// <summary>Añade una carpeta</summary>
		AddOneFolder
	}

	public NodeCommandViewModel(TreeFilesViewModel trvTree, ControlHierarchicalViewModel? parent, CommandType command, string icon) 
				: base(trvTree, parent, "Command", TreeFilesViewModel.NodeType.Command.ToString(), icon, null, true, MvvmColor.Navy)
	{
		// Asigna las propiedades
		ViewModel = trvTree;
		Command = command;
		// Cambia el texto
		switch (command)
		{
			case CommandType.AddOneFolder:
					Text = "Add folder";
				break;
			case CommandType.AddDriveNodes:
					Text = "Add the computer's drives";
				break;
		}
	}

	/// <summary>
	///		Ejecuta el comando
	/// </summary>
	protected override void Execute()
	{
		if (TreeViewModel is TreeFilesViewModel tree)
			switch (Command)
			{
				case CommandType.AddDriveNodes:
						tree.AddDriveNodes();
					break;
				case CommandType.AddOneFolder:
						tree.AddFolderToExplorer();
					break;
			}
	}

	/// <summary>
	///		Obtiene un nodo
	/// </summary>
	private NodeFileViewModel GetNode(string fileName, bool isFolder) => new NodeFileViewModel(ViewModel, this, fileName, isFolder);

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public TreeFilesViewModel.NodeType NodeType => Type.GetEnum(TreeFilesViewModel.NodeType.Command);

	/// <summary>
	///		Tipo de comando
	/// </summary>
	public CommandType Command { get; }

	/// <summary>
	///		ViewModel
	/// </summary>
	public TreeFilesViewModel ViewModel { get; }
}