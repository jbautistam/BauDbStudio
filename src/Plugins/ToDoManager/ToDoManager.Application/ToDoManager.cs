namespace Bau.Libraries.ToDoManager.Application;

/// <summary>
///		Manager de la aplicación
/// </summary>
public class ToDoManager
{
	public ToDoManager()
	{
		AppointmentsManager = new Appointments.AppointmentsManager(this);
		NotesManager = new Notes.NotesManager(this);
		TimeManagementManager = new TimeManagement.TimeManagementManager(this);
	}

	/// <summary>
	///		Carga los datos de un directorio
	/// </summary>
	public void Load(string folder)
	{
		// Guarda el directorio
		Folder = folder;
		// Carga los datos
		AppointmentsManager.Load();
		NotesManager.Load();
	}

	/// <summary>
	///		Manager de <see cref="Appointments.Models.AppointmentModel"/>
	/// </summary>
	public Appointments.AppointmentsManager AppointmentsManager { get; }

	/// <summary>
	///		Manager de <see cref="Notes.Models.NoteModel"/>
	/// </summary>
	public Notes.NotesManager NotesManager { get; }

	/// <summary>
	///		Manager de <see cref="TimeManagement.Models.ProjectModel"/>
	/// </summary>
	public TimeManagement.TimeManagementManager TimeManagementManager { get; }

	/// <summary>
	///		Carpeta
	/// </summary>
	public string Folder { get; private set; } = default!;
}
