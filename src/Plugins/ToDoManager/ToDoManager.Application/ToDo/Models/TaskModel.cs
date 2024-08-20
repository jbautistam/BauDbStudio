namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Clase con los datos de una tarea
/// </summary>
public class TaskModel : LibDataStructures.Base.BaseModel
{
	/// <summary>
	///		Estados
	/// </summary>
	public enum StatusType
	{
		/// <summary>Planificado</summary>
		Planned = 1,
		/// <summary>Indica que se está haciendo</summary>
		Doing,
		/// <summary>Finalizado</summary>
		Done
	}
	/// <summary>
	///		Prioridad
	/// </summary>
	public enum PriorityType
	{
		/// <summary>Alta</summary>
		High = 1,
		/// <summary>Normal</summary>
		Normal,
		/// <summary>Baja</summary>
		Low
	}
	/// <summary>
	///		Prioridad en la metodología MoSCoW
	/// </summary>
	public enum MoskowType
	{
		/// <summary>Obligatorio</summary>
		MustHave = 1,
		/// <summary>Deberíamos hacerlo</summary>
		ShouldHave,
		/// <summary>Se podría hacer</summary>
		CouldHave,
		/// <summary>No lo haremos</summary>
		WontHave 
	}

	/// <summary>
	///		Título
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Estado de la tarea
	/// </summary>
	public StatusType Status { get; set; } = StatusType.Planned;

	/// <summary>
	///		Prioridad
	/// </summary>
	public PriorityType Priority { get; set; } = PriorityType.Normal;

	/// <summary>
	///		Prioridad en metodología MoSCoW
	/// </summary>
	public MoskowType Moskow { get; set; } = MoskowType.CouldHave;

	/// <summary>
	///		Notas
	/// </summary>
	public string Notes { get; set; } = default!;

	/// <summary>
	///		Orden del elemento
	/// </summary>
	public int Order { get; set; }

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.Now;

	/// <summary>
	///		Fecha prevista de finalización
	/// </summary>
	public DateTime? DueAt { get; set; }

	/// <summary>
	///		Fecha de finalización
	/// </summary>
	public DateTime? FinishedAt { get; set; }

	/// <summary>
	///		Identificadores de etiquetas asociadas a la tarea
	/// </summary>
	public List<string> TagsId { get; } = new();
}
