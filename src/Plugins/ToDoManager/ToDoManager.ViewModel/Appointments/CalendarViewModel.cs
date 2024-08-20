using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Appointments;

/// <summary>
///		ViewModel para los calendarios de <see cref="AppointmentsRootModel"/>
/// </summary>
public class CalendarViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private AppointmentListViewModel _appointmentListViewModel = default!;

	public CalendarViewModel(ToDoManagerViewModel viewModel)
	{
		MainViewModel = viewModel;
		AppointmentsList = new AppointmentListViewModel(this);
	}

	/// <summary>
	///		Inicializa el viewmodels
	/// </summary>
	internal void Load()
	{
		AppointmentsList.Load(DateTime.Now);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Lista de citas
	/// </summary>
	public AppointmentListViewModel AppointmentsList
	{
		get { return _appointmentListViewModel; }
		set { CheckObject(ref _appointmentListViewModel, value); }
	}
}
