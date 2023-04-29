using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

/// <summary>
///		ViewModel del nodo que indica que se está cargando el archivo
/// </summary>
public class NodeFileLoadingViewModel : BaseTreeNodeViewModel
{
	public NodeFileLoadingViewModel(BaseTreeViewModel? trvTree, IHierarchicalViewModel? parent, string text) 
				: base(trvTree, parent, text, TreeFilesViewModel.NodeType.File.ToString(), string.Empty, null, false, false, MvvmColor.Gray)
	{
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void LoadNodes()
	{
		// ... no hace nada, sólo implementa la interface
	}
}