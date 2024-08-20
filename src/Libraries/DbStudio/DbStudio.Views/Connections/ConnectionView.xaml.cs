using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.Connections;

namespace Bau.Libraries.DbStudio.Views.Connections;

/// <summary>
///		Vista para mostrar los datos de una conexión
/// </summary>
public partial class ConnectionView : Window
{
	public ConnectionView(ConnectionViewModel viewModel)
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
	///		ViewModel de la conexión
	/// </summary>
	public ConnectionViewModel ViewModel { get; }
}
