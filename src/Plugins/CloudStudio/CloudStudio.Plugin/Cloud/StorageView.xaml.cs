using System.Windows;

using Bau.Libraries.CloudStudio.ViewModels.Details.Cloud;

namespace Bau.Libraries.CloudStudio.Plugin.Cloud;

/// <summary>
///		Vista para mostrar los datos de una conexión
/// </summary>
public partial class StorageView : Window
{
	public StorageView(StorageViewModel viewModel)
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
	///		ViewModel del storage
	/// </summary>
	public StorageViewModel ViewModel { get; }
}
