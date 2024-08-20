using System.Windows;

using Bau.Libraries.PluginsStudio.ViewModels.Tools;

namespace Bau.DbStudio.Views.Files;

/// <summary>
///		Vista para grabar archivos abiertos
/// </summary>
public partial class SaveOpenFilesView : Window
{
	public SaveOpenFilesView(SaveOpenFilesViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.Close += (sender, eventArgs) => 
								{
									DialogResult = eventArgs.IsAccepted; 
									Close();
								};
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public SaveOpenFilesViewModel ViewModel { get; }
}
