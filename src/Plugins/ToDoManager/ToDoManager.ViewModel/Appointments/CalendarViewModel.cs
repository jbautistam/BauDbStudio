using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Appointments;

/// <summary>
///		ViewModel para los calendarios de <see cref="AppointmentsRootModel"/>
/// </summary>
public class CalendarViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	public CalendarViewModel(ToDoManagerViewModel viewModel)
	{
		MainViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }
}
