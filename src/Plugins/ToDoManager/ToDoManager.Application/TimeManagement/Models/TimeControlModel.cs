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
	///		Fecha
	/// </summary>
	public DateOnly Date { get; }

	/// <summary>
	///		Proyectos
	/// </summary>
	public LibDataStructures.Base.BaseExtendedModelCollection<ProjectModel> Projects { get; } = [];
}
