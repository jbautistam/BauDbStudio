namespace Bau.Libraries.ToDoManager.Application.Notes.Models;

/// <summary>
///		Clase con los datos de una nota
/// </summary>
public class NoteModel
{
	/// <summary>
	///		Id de la nota
	/// </summary>
	public Guid Id { get; } = Guid.NewGuid();

	/// <summary>
	///		Título de la nota
	/// </summary>
	public string Title { get; set; } = default!;

	/// <summary>
	///		Contenido de la nota
	/// </summary>
	public string Content { get; set; } = default!;

	/// <summary>
	///		Indica si se debe mostrar la ventana sobre todas las demás
	/// </summary>
	public bool OverAllWindows { get; set; } = true;

	/// <summary>
	///		Coordenada superior
	/// </summary>
	public int Top { get; set; }

	/// <summary>
	///		Coordenada izquierda
	/// </summary>
	public int Left { get; set; }

	/// <summary>
	///		Ancho de la ventana
	/// </summary>
	public int Width { get; set; } = 500;

	/// <summary>
	///		Alto de la ventana
	/// </summary>
	public int Height { get; set; } = 500;

	/// <summary>
	///		Color de fondo
	/// </summary>
	public string Background { get; set; } = "FFE97F";

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.Now;

	/// <summary>
	///		Fecha de modificación
	/// </summary>
	public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
