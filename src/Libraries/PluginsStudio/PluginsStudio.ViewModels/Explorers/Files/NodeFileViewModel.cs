using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel de un nodo de archivo
/// </summary>
public class NodeFileViewModel : BaseTreeNodeAsyncViewModel
{
	// Variables privadas
	private string _fileName = string.Empty;
	private bool _isFolder;

	public NodeFileViewModel(TreeFilesViewModel? trvTree, ControlHierarchicalViewModel? parent, string fileName, bool isFolder) 
				: base(trvTree, parent, string.Empty, TreeFilesViewModel.NodeType.File.ToString(), 
					   string.Empty, fileName, isFolder, isFolder,
					   isFolder ? MvvmColor.Navy : MvvmColor.Black)
	{
		FileName = fileName;
		IsFolder = isFolder;
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			Text = Path.GetFileName(FileName);
			ToolTipText = FileName;
		}
		else
			Text = "...";
	}

	/// <summary>
	///		Obtiene la lista de nodos hijo
	/// </summary>
	protected override async Task<List<BaseTreeNodeViewModel>> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		List<BaseTreeNodeViewModel> nodes = new();

			// Evita las advertencias
			await Task.Delay(1);
			// Carga los nodos
			if (!string.IsNullOrWhiteSpace(FileName) && Directory.Exists(FileName))
			{
				// Carga los directorios
				foreach (string fileName in Directory.EnumerateDirectories(FileName))
					nodes.Add(GetNode(fileName, true));
				// Carga los archivos
				foreach (string fileName in Directory.EnumerateFiles(FileName))
					nodes.Add(GetNode(fileName, false));
			}
			// Devuelve la lista
			return nodes;
	}

	/// <summary>
	///		Comprueba si dos nodos son iguales
	/// </summary>
	public override bool IsEquals(BaseTreeNodeViewModel node)
	{
		if (node is NodeFileViewModel target)
			return FileName.Equals(target.FileName, StringComparison.CurrentCultureIgnoreCase);
		else
			return base.IsEquals(node);
	}

	/// <summary>
	///		Obtiene el texto que se debe lanzar al editor
	/// </summary>
	public override string GetTextForEditor(bool shiftPressed) => FileName;

	/// <summary>
	///		Obtiene un nodo
	/// </summary>
	private NodeFileViewModel GetNode(string fileName, bool isFolder)
	{
		return new NodeFileViewModel(TreeViewModel as TreeFilesViewModel, this, fileName, isFolder);
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Indica si se trata de una carpeta
	/// </summary>
	public bool IsFolder
	{
		get { return _isFolder; }
		set { CheckProperty(ref _isFolder, value); }
	}

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public TreeFilesViewModel.NodeType NodeType => Type.GetEnum(TreeFilesViewModel.NodeType.Unknown);
}