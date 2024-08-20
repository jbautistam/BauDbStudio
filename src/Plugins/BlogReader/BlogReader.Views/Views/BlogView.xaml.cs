using System.Windows;

using Bau.Libraries.LibBlogReader.ViewModel.Blogs;

namespace Bau.Libraries.BlogReader.Views.Views;

/// <summary>
///		Formulario para mantenimiento de un <see cref="BlogViewModel"/>
/// </summary>
public partial class BlogView : Window
{
	public BlogView(BlogViewModel viewModel)
	{ 
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa el ViewModel
		DataContext = viewModel;
		viewModel.Close += (sender, result) =>
										{
											DialogResult = result.IsAccepted;
											Close();
										};
		// Inicializa los controles
		udtFolder.InitControl(viewModel.MainViewModel);
	}
}