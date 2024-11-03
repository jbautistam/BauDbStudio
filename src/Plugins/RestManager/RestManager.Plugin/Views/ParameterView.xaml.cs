using System.Windows;

using Bau.Libraries.RestManager.ViewModel.Project.Parameters;

namespace Bau.Libraries.RestManager.Plugin.Views;

/// <summary>
///		Vista para mostrar los datos de un parámetro
/// </summary>
public partial class ParameterView : Window
{
	public ParameterView(ParameterViewModel viewModel)
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
	///		ViewModel del parámetro
	/// </summary>
	public ParameterViewModel ViewModel { get; }
}
