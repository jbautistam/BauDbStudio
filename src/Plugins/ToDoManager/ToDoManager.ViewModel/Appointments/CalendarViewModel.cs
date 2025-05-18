using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;
using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Appointments;

/// <summary>
///		ViewModel para los calendarios de <see cref="AppointmentsRootModel"/>
/// </summary>
public class CalendarViewModel : BauMvvm.ViewModels.BaseObservableObject, IPaneViewModel
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
	///		Ejecuta un comando externo: sólo implementa el interface
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		// No hace nada sólo implementa el interface
	}

	/// <summary>
	///		Cierra el panel: sólo implementa el interface
	/// </summary>
	public void Close()
	{
		// No hace nada sólo implementa el interface
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera del panel
	/// </summary>
	public string Header => "Calendar";

	/// <summary>
	///		Id del panel
	/// </summary>
	public string TabId => GetType().ToString();

	/// <summary>
	///		Lista de citas
	/// </summary>
	public AppointmentListViewModel AppointmentsList
	{
		get { return _appointmentListViewModel; }
		set { CheckObject(ref _appointmentListViewModel!, value); }
	}
}
