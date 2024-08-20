namespace Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

/// <summary>
///		Datos de tiempo
/// </summary>
public class TimeModel : LibDataStructures.Base.BaseModel
{
	public TimeModel(TaskModel task)
	{
		Task = task;
	}

	/// <summary>
	///		Tarea
	/// </summary>
	public TaskModel Task { get; }

	/// <summary>
	///		Comentarios
	/// </summary>
	public string? Remarks { get; set; }

	/// <summary>
	///		Hora de inicio
	/// </summary>
	public DateTime Start { get; set; } = DateTime.Now;

	/// <summary>
	///		Hora de fin
	/// </summary>
	public DateTime End { get; set; } = DateTime.Now;
}