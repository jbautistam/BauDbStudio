using System.Windows;

using Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Filters;

namespace Bau.Libraries.StructuredFilesStudio.Views.Files;

/// <summary>
///		Vista para mostrar los datos de un <see cref="ListFileFilterViewModel"/>
/// </summary>
public partial class ListFilterView : Window
{
	public ListFilterView(ListFileFilterViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ListFileFilterViewModel ViewModel { get; set; }

	private void cmdAccept_Click(object sender, RoutedEventArgs e)
	{
		if (ViewModel.ValidateData())
		{
			DialogResult = true;
			Close();
		}
	}
}
