using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Appointments;

/// <summary>
///		ViewModel de un <see cref="AppointmentModel"/>
/// </summary>
public class AppointmentListItemViewModel : ControlItemViewModel
{
	// Variables privadas
	private DateTime _dueAt;

	public AppointmentListItemViewModel(AppointmentListViewModel appointmentListViewModel, AppointmentModel appointment)
					: base(appointment.Header, appointment)
	{
		ViewModel = appointmentListViewModel;
		Appointment = appointment;
		DueAt = appointment.DueAt;
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public AppointmentListViewModel ViewModel { get; }

	/// <summary>
	///		Cita
	/// </summary>
	public AppointmentModel Appointment { get; }

	/// <summary>
	///		Fecha en la que debe saltar la cita
	/// </summary>
	public DateTime DueAt
	{
		get { return _dueAt; }
		set { CheckProperty(ref _dueAt, value); }
	}
}
