using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.ToDoManager.Application.Appointments.Models;

namespace Bau.Libraries.ToDoManager.Application.Appointments.Repository;

/// <summary>
///		Repositorio de <see cref="AppointmentModel"/>
/// </summary>
internal class AppointmentRepository
{
	// Constantes privadas
	private const string TagRoot = "Appointments";
	private const string TagAppointment = "Appointment";
	private const string TagId = "Id";
	private const string TagHeader = "Header";
	private const string TagDescription = "Description";
	private const string TagNotes = "Notes";
	private const string TagCreatedAt = "CreatedAt";
	private const string TagDueAt = "DueAt";
	private const string TagNextNoticeAt = "NextNoticeAt";
	private const string TagAllDayLong = "AllDayLong";

	/// <summary>
	///		Carga <see cref="AppointmentsRootModel"/> de un archivo
	/// </summary>
	internal AppointmentsRootModel Load(string fileName)
	{
		AppointmentsRootModel appointments = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga las citas
			foreach (MLNode rootML in fileML.Nodes)
				if (rootML.Name == TagRoot)
					foreach (MLNode nodeML in rootML.Nodes)
						if (nodeML.Name == TagAppointment)
							appointments.Add(LoadAppointment(nodeML));
			// Devuelve las citas
			return appointments;
	}

	/// <summary>
	///		Carga los datos de una cita
	/// </summary>
	private AppointmentModel LoadAppointment(MLNode rootML)
	{
		AppointmentModel appointment = new();

			// Carga los datos
			appointment.Id = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			appointment.Header = rootML.Attributes[TagHeader].Value.TrimIgnoreNull();
			appointment.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
			appointment.Notes = rootML.Nodes[TagNotes].Value.TrimIgnoreNull();
			appointment.CreatedAt = rootML.Attributes[TagCreatedAt].Value.GetDateTime(DateTime.UtcNow);
			appointment.DueAt = rootML.Attributes[TagDueAt].Value.GetDateTime(DateTime.UtcNow);
			appointment.NextNoticeAt = rootML.Attributes[TagNextNoticeAt].Value.GetDateTime(DateTime.UtcNow);
			appointment.AllDayLong = rootML.Attributes[TagAllDayLong].Value.GetBool();
			// Devuelve la cita
			return appointment;
	}

	/// <summary>
	///		Graba <see cref="AppointmentsRootModel"/> en un archivo
	/// </summary>
	internal void Save(string fileName, AppointmentsRootModel appointments)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Graba las citas
			foreach (AppointmentModel appointment in appointments.Appointments)
				rootML.Nodes.Add(GetXmlAppointment(appointment));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene el nodo de un <see cref="AppointmentModel"/>
	/// </summary>
	private MLNode GetXmlAppointment(AppointmentModel appointment)
	{
		MLNode rootML = new(TagAppointment);

			// Añade las propiedades
			rootML.Attributes.Add(TagId, appointment.Id);
			rootML.Attributes.Add(TagHeader, appointment.Header);
			rootML.Nodes.Add(TagDescription, appointment.Description);
			rootML.Nodes.Add(TagNotes, appointment.Notes);
			rootML.Attributes.Add(TagCreatedAt, appointment.CreatedAt);
			rootML.Attributes.Add(TagDueAt, appointment.DueAt);
			rootML.Attributes.Add(TagNextNoticeAt, appointment.NextNoticeAt);
			rootML.Attributes.Add(TagAllDayLong, appointment.AllDayLong);
			// Devuelve el nodo
			return rootML;
	}
}
