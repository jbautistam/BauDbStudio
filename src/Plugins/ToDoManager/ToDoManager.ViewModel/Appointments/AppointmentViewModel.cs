using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Appointments;

/// <summary>
///		ViewModel de modificación de un <see cref="AppointmentModel"/>
/// </summary>
public class AppointmentViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _header = default!, _description = default!, _notes = default!;
	private DateTime _dueAt;
	private bool _allDayLong;

	public AppointmentViewModel(AppointmentListViewModel appointmentListViewModel, AppointmentModel appointment)
	{
		AppointmentListViewModel = appointmentListViewModel;
		Appointment = appointment;
		Header = appointment.Header;
		Description = appointment.Description;
		Notes = appointment.Notes;
		DueAt = appointment.DueAt;
		AllDayLong = appointment.AllDayLong;
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Header))
				AppointmentListViewModel.CalendarViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Enter the header text");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Asigna los datos
			Appointment.Header = Header;
			Appointment.Description = Description;
			Appointment.Notes = Notes;
			Appointment.DueAt = DueAt;
			Appointment.AllDayLong = AllDayLong;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public AppointmentListViewModel AppointmentListViewModel { get; }

	/// <summary>
	///		Cita
	/// </summary>
	public AppointmentModel Appointment { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header 
	{ 
		get { return _header; } 
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Descripción
	/// </summary>
	public string Description 
	{ 
		get { return _description; } 
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Notas
	/// </summary>
	public string Notes 
	{ 
		get { return _notes; } 
		set { CheckProperty(ref _notes, value); }
	}

	/// <summary>
	///		Fecha en la que debe saltar la cita
	/// </summary>
	public DateTime DueAt
	{
		get { return _dueAt; }
		set { CheckProperty(ref _dueAt, value); }
	}

	/// <summary>
	///		Indica si se debe considerar una cita para todo el día (sin una hora en concreto)
	/// </summary>
	public bool AllDayLong 
	{ 
		get { return _allDayLong; } 
		set { CheckProperty(ref _allDayLong, value); }
	}
}
