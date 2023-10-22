namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Clase con los datos de una etiqueta
/// </summary>
public class TagModel : LibDataStructures.Base.BaseModel
{
	/// <summary>
	///		Nombre de la etiqueta
	/// </summary>
	public string Name { get; set; } = default!;
}
