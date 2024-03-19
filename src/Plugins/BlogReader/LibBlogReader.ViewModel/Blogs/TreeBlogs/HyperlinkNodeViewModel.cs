using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

/// <summary>
///		Nodo del árbol <see cref="HyperlinkModel"/>
/// </summary>
public class HyperlinkNodeViewModel : BaseBlogsNodeViewModel
{
	public HyperlinkNodeViewModel(PluginTreeViewModel trvTree, BlogReaderViewModel mainViewModel, FolderNodeViewModel? parent, HyperlinkModel hyperlink) 
				: base(trvTree, parent, hyperlink.Id.ToString() ?? string.Empty, hyperlink.Name, false)
	{
		MainViewModel = mainViewModel;
		Hyperlink = hyperlink;
		Icon = TreeBlogsViewModel.NodeType.Hyperlink.ToString();
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
	}

	/// <summary>
	///		Obtiene los nodos, en este caso sólo implementa la interface
	/// </summary>
	protected override async Task<List<PluginNodeViewModel>?> GetChildNodesAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1);
		return null;
	}

	/// <summary>
	///		Calcula elnúmero de elementos no leidos
	/// </summary>
	public override void ComputeNumberNotRead()
	{
		// ... en este caso no hace nada
	}

	/// <summary>
	///		Obtiene los blogs hijo
	/// </summary>
	public override BlogsModelCollection GetBlogs() => [];

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		Hipervínculo
	/// </summary>
	public HyperlinkModel Hyperlink { get; }
}
