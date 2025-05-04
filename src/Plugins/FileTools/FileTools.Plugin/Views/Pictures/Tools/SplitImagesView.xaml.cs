using System.Windows;

using Bau.Libraries.FileTools.ViewModel.Pictures.Tools;

namespace Bau.Libraries.FileTools.Plugin.Views.Pictures.Tools;

/// <summary>
///		Formulario para mantenimiento de un <see cref="SplitImagesViewModel"/>
/// </summary>
public partial class SplitImagesView : Window
{
	public SplitImagesView(SplitImagesViewModel viewModel)
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
	}
}