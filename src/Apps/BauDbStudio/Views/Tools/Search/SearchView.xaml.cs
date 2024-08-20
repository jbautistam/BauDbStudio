using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.ViewModels.Tools.Search;

namespace Bau.DbStudio.Views.Tools.Search;

/// <summary>
///		Vista para la búsqueda de archivos
/// </summary>
public partial class SearchView : UserControl
{
	public SearchView(SearchFilesViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel de la búsqueda de archivos
	/// </summary>
	public SearchFilesViewModel ViewModel { get; }

	private void trvResults_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{ 
		if ((sender as TreeView)?.SelectedItem is TreeResultNodeViewModel node)
			ViewModel.TreeResultsViewModel.SelectedNode = node;
	}

	private void trvResults_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if ((sender as TreeView)?.SelectedItem is TreeResultNodeViewModel)
			ViewModel.TreeResultsViewModel.OpenCommand.Execute(null);
	}
}
