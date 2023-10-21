using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ListView;
using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Appointments;

/// <summary>
///		Lista de <see cref="AppointmentModel"/>
/// </summary>
public class AppointmentListViewModel : ControlGenericListViewModel<AppointmentListItemViewModel>
{
	public AppointmentListViewModel(CalendarViewModel calendarViewModel)
	{
		// Asigna las propiedades
		CalendarViewModel = calendarViewModel;
		// Asigna los comandos
		NewAppointmentCommand = new BaseCommand(_ => OpenAppointment(null));
		UpdateAppointmentCommand = new BaseCommand(_ => OpenAppointment(SelectedItem), _ => SelectedItem is not null)
											.AddListener(this, nameof(SelectedItem));
		DeleteAppointmentCommand = new BaseCommand(_ => DeleteAppointment(SelectedItem), _ => SelectedItem is not null)
											.AddListener(this, nameof(SelectedItem));
	}

	/// <summary>
	///		Carga los datos a partir de una fecha
	/// </summary>
	internal void Load(DateTime date)
	{
		// Carga los datos
		Appointments = CalendarViewModel.MainViewModel.ToDoManager.AppointmentsManager.Load();
		// Ordena las citas
		Appointments.Sort();
		// Muestra las citas en la lista
		Items.Clear();
		foreach (AppointmentModel appointment in Appointments.Appointments)
			Items.Add(new AppointmentListItemViewModel(this, appointment));
	}

	/// <summary>
	///		Abre un <see cref="AppointmentModel"/>
	/// </summary>
	private void OpenAppointment(ControlItemViewModel? selectedItem)
	{
		AppointmentModel? appointment = null;
		bool isNew = true;

			// Obtiene los datos de la cita
			if (selectedItem is null)
				appointment = new AppointmentModel();
			else if (selectedItem is AppointmentListItemViewModel appointmentListItemView)
			{
				appointment = appointmentListItemView.Appointment;
				isNew = false;
			}
			// Abre el cuadro de diálogo de modificación / creación
			if (appointment is not null &&
				CalendarViewModel.MainViewModel.ViewsController.OpenDialog(new AppointmentViewModel(this, appointment)) == 
						BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Añade la cita
				if (isNew)
					Appointments.Add(appointment);
				// Graba los datos
				Save();
			}
	}

	/// <summary>
	///		Borra un <see cref="AppointmentModel"/>
	/// </summary>
	private void DeleteAppointment(ControlItemViewModel? selectedItem)
	{
		if (selectedItem is not null && selectedItem is AppointmentListItemViewModel appointmentViewModel)
		{
			if (CalendarViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowQuestionCancel($"Do you want to delete the appointment {appointmentViewModel.Text}?") == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Borra la cita
				Appointments.Delete(appointmentViewModel.Appointment);
				// Graba y actualiza
				Save();
			}
		}
	}

	/// <summary>
	///		Graba los datos y actualiza
	/// </summary>
	private void Save()
	{
		// Graba los datos
		CalendarViewModel.MainViewModel.ToDoManager.AppointmentsManager.Save(Appointments);
		// Actualiza la pantalla
		Load(DateTime.Now);
	}

	/// <summary>
	///		ViewModel del calendario
	/// </summary>
	public CalendarViewModel CalendarViewModel { get; }

	/// <summary>
	///		Citas
	/// </summary>
	private AppointmentsRootModel Appointments { get; set; } = new();

	/// <summary>
	///		Comando para añadir una cita
	/// </summary>
	public BaseCommand NewAppointmentCommand { get; }

	/// <summary>
	///		Comando para modificar una cita
	/// </summary>
	public BaseCommand UpdateAppointmentCommand { get; }

	/// <summary>
	///		Comando para borrar una cita
	/// </summary>
	public BaseCommand DeleteAppointmentCommand { get; }
}
