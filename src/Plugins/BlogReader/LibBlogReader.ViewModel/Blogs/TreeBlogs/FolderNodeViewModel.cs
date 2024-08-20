using Bau.Libraries.LibBlogReader.Model;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

/// <summary>
///		Nodo del árbol <see cref="FolderModel"/>
/// </summary>
public class FolderNodeViewModel : BaseBlogsNodeViewModel
{
	public FolderNodeViewModel(PluginTreeViewModel trvTree, BlogReaderViewModel mainViewModel, FolderNodeViewModel? parent, FolderModel folder) 
				: base(trvTree, parent, folder.FullName, folder.Name, false)
	{
		MainViewModel = mainViewModel;
		Folder = folder;
		Icon = TreeBlogsViewModel.NodeType.Folder.ToString();
		Foreground = BauMvvm.ViewModels.Media.MvvmColor.Navy;
	}

	/// <summary>
	///		Obtiene los nodos, en este caso sólo implementa la interface
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
			// Carga los blog hijos
			Folder.Blogs.SortByName();
			foreach (BlogModel blog in Folder.Blogs)
				nodes.Add(new BlogNodeViewModel(TreeViewModel, MainViewModel, this, blog));
			// Carga los hipervínculos hijo
			Folder.Hyperlinks.SortByName();
			foreach (HyperlinkModel hyperlink in Folder.Hyperlinks)
				nodes.Add(new HyperlinkNodeViewModel(TreeViewModel, MainViewModel, this, hyperlink));
			// Devuelve la lista de nodos
			return nodes;
	}

	/// <summary>
	///		Obtiene el número de elementos no leídos
	/// </summary>
	public override void ComputeNumberNotRead()
	{
		int numberNotRead = 0;

			// Calcula el número de elementos no leidos
			foreach (BlogModel blog in GetBlogs())
				numberNotRead += blog.NumberNotRead;
			// Cambia el texto
			if (numberNotRead == 0)
			{
				Text = Folder.Name;
				IsBold = false;
			}
			else
			{
				Text = $"{Folder.Name} ({numberNotRead})";
				IsBold = true;
			}
			// Calcula el número de elementos no leidos de los hijos
			foreach (BauMvvm.ViewModels.Forms.ControlItems.Trees.ControlHierarchicalViewModel node in Children)
				if (node is BaseBlogsNodeViewModel child)
					child.ComputeNumberNotRead();
	}

	/// <summary>
	///		Obtiene los blogs asociados
	/// </summary>
	public override BlogsModelCollection GetBlogs()
	{
		BlogsModelCollection blogs = new();

			// Obtiene los blogs de la carpeta
			if (Folder != null)
				blogs = Folder.GetBlogsRecursive();
			// Devuelve la colección de blogs
			return blogs;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		Carpeta
	/// </summary>
	public FolderModel Folder { get; }
}
