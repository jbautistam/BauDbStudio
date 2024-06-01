namespace Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

/// <summary>
///		Datos del proyecto
/// </summary>
public class ProjectModel : LibDataStructures.Base.BaseExtendedModel
{
	/// <summary>
	///		Añade un control de tiempo a una tarea
	/// </summary>
	public void Add(string taskName, DateTime start, DateTime end)
	{
		TaskModel? task = Tasks.SearchByName(taskName);

			// Si no se ha definido la tarea se crea
			if (task is null)
			{
				// Se crea la tareas
				task = new TaskModel(this)
								{
									Name = taskName
								};
				// Añade el proyecto a la colección
				Tasks.Add(task);
			}
			// Añade el tiempo a la tarea
			task.Add(start, end);
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	internal void DeleteTime(string id)
	{
		foreach (TaskModel task in Tasks)
			task.Times.RemoveById(id);
	}

	/// <summary>
	///		Tareas
	/// </summary>
	public LibDataStructures.Base.BaseExtendedModelCollection<TaskModel> Tasks { get; } = [];
}
