using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

namespace Bau.Libraries.BlogReader.Views.Views;

/// <summary>
///		Ventana que muestra el árbol de blogs
/// </summary>
public partial class BlogTreeControlView : UserControl
{
	public BlogTreeControlView(TreeBlogsViewModel viewModel)
	{ 
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa el formulario
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel del formulario
	/// </summary>
	public TreeBlogsViewModel ViewModel { get; }

	private void trvBlogs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (trvBlogs.DataContext is TreeBlogsViewModel dataContext && sender is TreeView treeView &&
				treeView.SelectedItem is BaseBlogsNodeViewModel node)
			dataContext.SelectedNode = node;
	}

	private void trvBlogs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		ViewModel.SeeNewsCommand.Execute(null);
	}

	private void trvBlogs_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
			ViewModel.SelectedNode = null;
	}
}
