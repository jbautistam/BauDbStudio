namespace Bau.Libraries.ToDoManager.Application.Appointments.Models;

/// <summary>
///		Clase con los datos de una cita
/// </summary>
public class AppointmentModel
{
	/// <summary>
	///		Clave principal
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; set; } = default!;

	/// <summary>
	///		Descripción
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Notas
	/// </summary>
	public string Notes { get; set; } = default!;

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	///		Fecha en que debe saltar la cita
	/// </summary>
	public DateTime DueAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	///		Fecha de siguiente aviso
	/// </summary>
	public DateTime NextNoticeAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	///		Indica si se debe considerar una cita para todo el día (sin una hora en concreto)
	/// </summary>
	public bool AllDayLong { get; set; }
}
