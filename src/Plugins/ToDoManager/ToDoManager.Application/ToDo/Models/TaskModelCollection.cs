namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Colección de <see cref="TaskModel"/>
/// </summary>
public class TaskModelCollection : LibDataStructures.Base.BaseModelCollection<TaskModel>
{
	/// <summary>
	///		Ordena por la fecha de planificación
	/// </summary>
	public void SortByDueAt()
	{
		Sort((first, second) => -1 * (first.DueAt ?? first.CreatedAt).CompareTo(second.DueAt ?? second.CreatedAt));
	}
}
