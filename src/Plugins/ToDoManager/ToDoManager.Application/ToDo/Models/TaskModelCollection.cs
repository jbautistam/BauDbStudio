namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Colección de <see cref="TaskModel"/>
/// </summary>
public class TaskModelCollection : LibDataStructures.Base.BaseModelCollection<TaskModel>
{
	/// <summary>
	///		Asigna el orden a las tareas
	/// </summary>
	public void SetOrder()
	{
		// Ordena las tareas
		SortByOrder();
		// Reordena las listas
		SetOrder(TaskModel.StatusType.Planned);
		SetOrder(TaskModel.StatusType.Doing);
		SetOrder(TaskModel.StatusType.Done);
	}

	/// <summary>
	///		Cambia el orden a las tareas de determinado estado
	/// </summary>
	private void SetOrder(TaskModel.StatusType status)
	{
		int order = 0;

			// Asigna el orden a las tareas
			foreach (TaskModel task in this)
				if (task.Status == status)
					task.Order = order++;
	}

	/// <summary>
	///		Ordena por el orden
	/// </summary>
	public void SortByOrder()
	{
		Sort((first, second) => first.Order.CompareTo(second.Order));
	}

	/// <summary>
	///		Ordena por la fecha de planificación
	/// </summary>
	public void SortByDueAt()
	{
		Sort((first, second) => -1 * (first.DueAt ?? first.CreatedAt).CompareTo(second.DueAt ?? second.CreatedAt));
	}

	/// <summary>
	///		Obtiene el orden máximo de los elementos en un estado
	/// </summary>
	public int GetMaxOrder(TaskModel.StatusType status)
	{
		int maxOrder = 0;

			// Busca el orden máximo
			foreach (TaskModel task in this)
				if (task.Status == status && task.Order > maxOrder)
					maxOrder = task.Order;
			// Devuelve el orden máximo
			return maxOrder;
	}

	/// <summary>
	///		Cambia el orden de una tarea
	/// </summary>
	public void Move(TaskModel task, bool moveUp)
	{
		int second = -1;

			// Cambia el orden a las tareas
			SetOrder(task.Status);
			// Cambia los órdenes
			for (int index = 0; index < Count; index++)
				if (this[index].Status == task.Status)
				{
					if (moveUp && this[index].Order == task.Order - 1)
						second = index;
					if (!moveUp && this[index].Order == task.Order + 1)
						second = index;
				}
			// Cambia los objetos
			if (second >= 0)
			{
				int first = IndexOf(task);
				TaskModel other = this[second];

					// Cambia los objetos
					this[second] = task;
					this[first] = other;
			}
			// Asigna los órdenes
			SetOrder(task.Status);
	}
}
