namespace Bau.Libraries.ToDoManager.Application.Appointments.Models;

/// <summary>
///		Clase con una serie de <see cref="AppointmentModel"/>
/// </summary>
public class AppointmentsRootModel
{
	/// <summary>
	///		Añade una <see cref="AppointmentModel"/>
	/// </summary>
	public void Add(AppointmentModel appointment)
	{
		Appointments.Add(appointment);
	}

	/// <summary>
	///		Borra una <see cref="AppointmentModel"/>
	/// </summary>
	public void Delete(AppointmentModel appointment)
	{
		for (int index = Appointments.Count - 1; index >= 0; index--)
			if (Appointments[index].Id == appointment.Id)
				Appointments.RemoveAt(index);
	}

	/// <summary>
	///		Ordena las <see cref="AppointmentModel"/>
	/// </summary>
	public void Sort()
	{
		Appointments.Sort((first, second) => first.DueAt.CompareTo(second.DueAt));
	}

	/// <summary>
	///		Lista de <see cref="Appointments"/>
	/// </summary>
	public List<AppointmentModel> Appointments { get; } = new();
}
