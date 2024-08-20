using System.Windows;

using Bau.Libraries.PasswordManager.ViewModel.Generator;

namespace Bau.Libraries.PasswordManager.Plugin.Views;

/// <summary>
///		Vista para generar una contraseña
/// </summary>
public partial class GeneratorView : Window
{
	public GeneratorView(GeneratorViewModel viewModel)
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
	public GeneratorViewModel ViewModel { get; }
}
