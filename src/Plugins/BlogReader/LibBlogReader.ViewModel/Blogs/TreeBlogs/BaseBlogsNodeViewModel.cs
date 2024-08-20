using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

/// <summary>
///		Clase base para los nodos del árbol <see cref="TreeBlogsViewModel"/>
/// </summary>
public abstract class BaseBlogsNodeViewModel : PluginNodeAsyncViewModel
{
	public BaseBlogsNodeViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, string nodeID, string text, bool lazyLoadChildren = true)
							: base(trvTree, parent, text, TreeBlogsViewModel.NodeType.Unknown.ToString(), 
								   TreeBlogsViewModel.NodeType.Unknown.ToString(), null, lazyLoadChildren)
	{
		NodeId = nodeID;
	}

	/// <summary>
	///		Calcula el número de elementos no leídos (y ajusta el texto / propiedades)
	/// </summary>
	public abstract void ComputeNumberNotRead();

	/// <summary>
	///		Blogs asociados al nodo
	/// </summary>
	public abstract BlogsModelCollection GetBlogs();

	/// <summary>
	///		Clave del nodo
	/// </summary>
	public string NodeId { get; }

	/// <summary>
	///		Tipo de nodo
	/// </summary>
	public TreeBlogsViewModel.NodeType NodeType => Type.GetEnum(TreeBlogsViewModel.NodeType.Unknown);
}
