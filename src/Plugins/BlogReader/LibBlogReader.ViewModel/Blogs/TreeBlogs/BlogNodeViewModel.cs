using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

/// <summary>
///		Nodo del árbol <see cref="BlogModel"/>
/// </summary>
public class BlogNodeViewModel : BaseBlogsNodeViewModel
{
	public BlogNodeViewModel(PluginTreeViewModel trvTree, BlogReaderViewModel mainViewModel, FolderNodeViewModel? parent, BlogModel blog) 
				: base(trvTree, parent, blog.GlobalId, blog.Name, false)
	{
		MainViewModel = mainViewModel;
		Blog = blog;
		Icon = TreeBlogsViewModel.NodeType.Blog.ToString();
		if (blog.Enabled)
			Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
		else
			Foreground = BauMvvm.ViewModels.Media.MvvmColor.Gray;
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
	///		Modifica el elemento seleccionado
	/// </summary>
	internal void UpdateItem()
	{
		MainViewModel.ViewsController.OpenDialog(new BlogViewModel(MainViewModel, null, Blog));
	}

	/// <summary>
	///		Calcula el número de elementos no leidos (y ajusta texto y demás)
	/// </summary>
	public override void ComputeNumberNotRead()
	{
		if (Blog.NumberNotRead == 0)
		{
			Text = Blog.Name;
			IsBold = false;
		}
		else
		{
			Text = $"{Blog.Name} ({Blog.NumberNotRead})";
			IsBold = true;
		}
	}

	/// <summary>
	///		Obtiene los blogs asociados
	/// </summary>
	public override BlogsModelCollection GetBlogs()
	{
		BlogsModelCollection blogs = [];

			// Añade el blog a la colección
			if (Blog is not null)
				blogs.Add(Blog);
			// Devuelve la colección de blogs
			return blogs;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		Blog
	/// </summary>
	public BlogModel Blog { get; }
}
