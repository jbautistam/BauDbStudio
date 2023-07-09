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
	public void Load()
	{
		Appointments = new Repository.AppointmentRepository().Load(GetFileName(Manager.Folder));
	}

	/// <summary>
	///		Graba las citas
	/// </summary>
	public void Save()
	{
		new Repository.AppointmentRepository().Save(GetFileName(Manager.Folder), Appointments);
	}

	/// <summary>
	///		Obtiene el nombre del archivo
	/// </summary>
	private string GetFileName(string path) => Path.Combine(path, "Appointments.appnt.xml");

	/// <summary>
	///		Manager
	/// </summary>
	public ToDoManager Manager { get; }

	/// <summary>
	///		Notas
	/// </summary>
	public Models.AppointmentsRootModel Appointments { get; private set; } = new();
}
