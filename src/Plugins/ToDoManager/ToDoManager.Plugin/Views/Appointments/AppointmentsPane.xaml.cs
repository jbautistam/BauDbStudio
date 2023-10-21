using System.Windows.Controls;

using Bau.Libraries.ToDoManager.ViewModel.Appointments;

namespace Bau.Libraries.ToDoManager.Plugin.Views.Appointments;

/// <summary>
///		Panel del calendario
/// </summary>
public partial class AppointmentsPane : UserControl
{
	public AppointmentsPane(CalendarViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel del calendario
	/// </summary>
	public CalendarViewModel ViewModel { get; }

	private void lswAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{

    }
}