namespace Bau.Libraries.ToDoManager.Application.Models;

/// <summary>
///		Clase con los datos de una tarea
/// </summary>
public class TaskModel : LibDataStructures.Base.BaseModel
{ 
	/// <summary>
	///		Título
	/// </summary>
	public string Name { get; set; } = default!;

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
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}
