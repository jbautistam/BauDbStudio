namespace Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

/// <summary>
///		Datos de la tarea
/// </summary>
public class TaskModel : LibDataStructures.Base.BaseExtendedModel
{
	public TaskModel(ProjectModel project)
	{
		Project = project;
	}

	/// <summary>
	///		Añade el control de tiempo a la tarea
	/// </summary>
	public void Add(DateTime start, DateTime end)
	{
		Times.Add(new TimeModel(this)
							{
								Start = start,
								End = end
							}
				 );
	}

	/// <summary>
	///		Proyecto en el que se ha definido la tarea
	/// </summary>
	public ProjectModel Project { get; }

	/// <summary>
	///		Horas asociadas a la tarea
	/// </summary>
	public LibDataStructures.Base.BaseModelCollection<TimeModel> Times { get; } = [];
}
