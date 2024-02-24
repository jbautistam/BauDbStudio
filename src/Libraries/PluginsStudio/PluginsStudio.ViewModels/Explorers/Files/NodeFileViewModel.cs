using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel de un nodo de archivo
/// </summary>
public class NodeFileViewModel : PluginNodeAsyncViewModel
{
	// Variables privadas
	private string _fileName = string.Empty;
	private bool _isFolder;

	public NodeFileViewModel(TreeFilesViewModel trvTree, ControlHierarchicalViewModel? parent, string fileName, bool isFolder) 
				: base(trvTree, parent, string.Empty, TreeFilesViewModel.NodeType.File.ToString(), 
					   string.Empty, fileName, isFolder, isFolder, isFolder ? MvvmColor.Navy : MvvmColor.Black)
	{
		ViewModel = trvTree;
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
	protected override async Task<List<PluginNodeViewModel>?> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Devuelve la lista
		return GetChildNodes();
	}

	/// <summary>
	///		Obtiene la lista de nodos hijo
	/// </summary>
	private List<PluginNodeViewModel> GetChildNodes()
	{
		List<PluginNodeViewModel> nodes = new();

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
	///		Carga los nodos y expande el nodo actual
	/// </summary>
	public void LoadAndExpandNodes()
	{
		// Limpia los nodos
		Children.Clear();
		// Añade los nodos
		foreach (PluginNodeViewModel node in GetChildNodes())
			Children.Add(node);
		// Expande el nodo (antes indica que ya se han cargado los nodos)
		LazyLoad = false;
		IsExpanded = true;
	}

	/// <summary>
	///		Comprueba si dos nodos son iguales
	/// </summary>
	public override bool IsEquals(ControlHierarchicalViewModel node)
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
	private NodeFileViewModel GetNode(string fileName, bool isFolder) => new NodeFileViewModel(ViewModel, this, fileName, isFolder);

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
				ToolTipText = value;
		}
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

	/// <summary>
	///		ViewModel
	/// </summary>
	public TreeFilesViewModel ViewModel { get; }
}