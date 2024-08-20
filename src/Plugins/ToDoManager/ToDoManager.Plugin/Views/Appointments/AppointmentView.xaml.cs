using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.Appointments;

namespace Bau.Libraries.ToDoManager.Plugin.Views.Appointments;

/// <summary>
///		Vista para mostrar los datos de una cita
/// </summary>
public partial class AppointmentView : Window
{
	public AppointmentView(AppointmentViewModel viewModel)
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
	///		ViewModel de la cita
	/// </summary>
	public AppointmentViewModel ViewModel { get; }
}
