namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Clase con datos de un archivo de tareas
/// </summary>
public class ToDoFileModel
{
	/// <summary>
	///		Tareas
	/// </summary>
	public TaskModelCollection Tasks { get; } = new();

	/// <summary>
	///		Etiquetas asociadas al archivo
	/// </summary>
	public TagModelCollection Tags { get; } = new();
}
