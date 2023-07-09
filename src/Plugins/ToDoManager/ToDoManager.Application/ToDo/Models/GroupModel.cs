namespace Bau.Libraries.ToDoManager.Application.ToDo.Models; 

/// <summary>
///		Groupo de elementos de una lista ToDo
/// </summary>
public class GroupModel : LibDataStructures.Base.BaseExtendedModel
{
	/// <summary>
	///		Elementos pendientes
	/// </summary>
	public TaskModelCollection Pending { get; } = new();

	/// <summary>
	///		Elementos en proceso
	/// </summary>
	public TaskModelCollection InProgress { get; } = new();

	/// <summary>
	///		Elementos hechos
	/// </summary>
	public TaskModelCollection Done { get; } = new();

	/// <summary>
	///		Elementos descartados
	/// </summary>
	public TaskModelCollection Discard { get; } = new();
}
