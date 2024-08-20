using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

namespace Bau.Libraries.ToDoManager.Plugin.Views.TimeManagement;

/// <summary>
///		Vista para mostrar los datos de un control de tiempo
/// </summary>
public partial class TimeView : Window
{
	public TimeView(TimeViewModel viewModel)
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
	///		ViewModel del elemento
	/// </summary>
	public TimeViewModel ViewModel { get; }
}
