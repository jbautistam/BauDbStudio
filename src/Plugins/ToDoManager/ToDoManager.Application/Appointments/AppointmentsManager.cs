namespace Bau.Libraries.ToDoManager.Application.Appointments;

/// <summary>
///		Manager de citas de la aplicación
/// </summary>
public class AppointmentsManager
{
	public AppointmentsManager(ToDoManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Carga las citas
	/// </summary>
	public Models.AppointmentsRootModel Load() => new Repository.AppointmentRepository().Load(GetFileName(Manager.Folder));

	/// <summary>
	///		Graba las citas
	/// </summary>
	public void Save(Models.AppointmentsRootModel appointmentsRoot)
	{
		new Repository.AppointmentRepository().Save(GetFileName(Manager.Folder), appointmentsRoot);
	}

	/// <summary>
	///		Obtiene el nombre del archivo
	/// </summary>
	private string GetFileName(string path) => Path.Combine(path, "Appointments.appnt.xml");

	/// <summary>
	///		Manager
	/// </summary>
	public ToDoManager Manager { get; }
}
