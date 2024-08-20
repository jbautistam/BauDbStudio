namespace Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

/// <summary>
///		Clase de mantenimiento de proyectos / horas
/// </summary>
public class TimeControlModel
{
	public TimeControlModel(DateOnly date)
	{
		Date = date;
	}

	/// <summary>
	///		Añade un control de tiempo a un proyecto
	/// </summary>
	public void Add(string projectName, string taskName, DateTime start, DateTime end)
	{
		ProjectModel? project = Projects.SearchByName(projectName);

			// Si no se ha definido el proyecto se crea
			if (project is null)
			{
				// Se crea el proyecto
				project = new ProjectModel()
									{
										Name = projectName
									};
				// Añade el proyecto a la colección
				Projects.Add(project);
			}
			// Añade el control de tiempo a la tarea
			project.Add(taskName, start, end);
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	public void DeleteTime(string id)
	{
		foreach (ProjectModel project in Projects)
			project.DeleteTime(id);
	}

	/// <summary>
	///		Combina dos controles horarios
	/// </summary>
	public void Merge(TimeControlModel timeControl)
	{
		foreach (ProjectModel project in timeControl.Projects)
			foreach (TaskModel task in project.Tasks)
				foreach (TimeModel time in task.Times)
					Add(project.Name, task.Name, time.Start, time.End);
	}

	/// <summary>
	///		Colapsa las horas
	/// </summary>
	public void Collapse(TimeSpan timeMinimum, TimeSpan timeGapMaximum, TimeSpan timeGapBetweenTasksConsolidate)
	{
		List<TimeModel> times = GetTimes();

			// Limpia los datos actuales
			Projects.Clear();
			// Ordena la lista de horas
			times.Sort((first, second) => first.Start.CompareTo(second.Start));
			// Colapsa las horas
			for (int index = 0; index < times.Count - 1; index++)
				if (times[index + 1].Start - times[index].End < timeGapMaximum)
					times[index + 1].Start = times[index].End.AddSeconds(1);
			// Colapsa las tareas
			Consolidate(times, timeGapBetweenTasksConsolidate);
			// Elimina las tareas que no superen el mínimo
			for (int index = times.Count - 1; index >= 0; index--)
				if (times[index].End - times[index].Start <= timeMinimum)
					times.RemoveAt(index);
			// Actualiza el proyecto
			foreach (TimeModel time in times)
				Add(time.Task.Project.Name, time.Task.Name, time.Start, time.End);
	}

	/// <summary>
	///		Colapsa las tareas del mismo proyecto si entre una y otra no han pasado más de X minutos. La lista de entrada debe estar ordenada
	/// </summary>
	private void Consolidate(List<TimeModel> times, TimeSpan timeGapBetweenTasksConsolidate)
	{
		bool updated;

			do
			{
				// Indica que en este bucle aún no ha habido modificaciones
				updated = false;
				// Colapsa las horas del mismo proyecto / tarea
				for (int index = times.Count - 1; index > 0; index--)
					if (times[index].Task.Name.Equals(times[index - 1].Task.Name, StringComparison.CurrentCultureIgnoreCase) &&
							times[index].Task.Project.Name.Equals(times[index - 1].Task.Project.Name, StringComparison.CurrentCultureIgnoreCase) &&
							times[index].Start - times[index - 1].End < timeGapBetweenTasksConsolidate)
					{
						// Colapsa ambas tareas
						times[index - 1].End = times[index].End;
						// Elimina la tarea final
						times.RemoveAt(index);
						// Indica que algo se ha modificado
						updated = true;
					}
			}
			while (updated);
	}

	/// <summary>
	///		Crea un control de tiempo aleatorio
	/// </summary>
	private void RandomTime()
	{
		Random rnd = new();
		DateTime start = new DateTime(Date.Year, Date.Month, Date.Day, 9, 0, 0);

			// Limpia los datos
			Projects.Clear();
			// Crea n tareas
			for (int index = 0; index < 20; index++)
			{
				DateTime end = start.AddMinutes(rnd.Next(20));

					// Añade la tarea
					Add($"Project {rnd.Next(3)}", $"Task {rnd.Next(3)}", start, end);
					// Pasa a la siguiente hora
					start = end.AddMinutes(rnd.Next(8));
			}
	}

	/// <summary>
	///		Obtiene una lista de valores de tiempo
	/// </summary>
	public List<TimeModel> GetTimes()
	{
		List<TimeModel> times = [];

			// Crea la lista
			foreach (ProjectModel project in Projects)
				foreach (TaskModel task in project.Tasks)
					foreach (TimeModel time in task.Times)
						times.Add(time);
			// Devuelve la lista
			return times;
	}

	/// <summary>
	///		Fecha
	/// </summary>
	public DateOnly Date { get; }

	/// <summary>
	///		Proyectos
	/// </summary>
	public LibDataStructures.Base.BaseExtendedModelCollection<ProjectModel> Projects { get; } = [];
}
