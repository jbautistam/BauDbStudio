using System.Windows;

using Bau.Libraries.RestManager.ViewModel.Project.Connections;

namespace Bau.Libraries.RestManager.Plugin.Views.Connections;

/// <summary>
///		Vista para mostrar los datos de una <see cref="ConnectionViewModel"/>
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
		lstHeaders.Parameters = viewModel.Headers;
	}

	/// <summary>
	///		ViewModel del parámetro
	/// </summary>
	public ConnectionViewModel ViewModel { get; }
}
