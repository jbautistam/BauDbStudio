namespace Bau.Libraries.ToDoManager.Application.Notes.Models;

/// <summary>
///		Raíz de las notas
/// </summary>
public class NotesRootModel
{
	/// <summary>
	///		Borra una nota
	/// </summary>
	public void Delete(NoteModel note)
	{
		for (int index = Notes.Count - 1; index >= 0; index--)
			if (Notes[index].Id == note.Id)
				Notes.RemoveAt(index);
	}

	/// <summary>
	///		Añade una nota (si hay algún dato)
	/// </summary>
	public void Add(NoteModel note)
	{
		if (!string.IsNullOrWhiteSpace(note.Title) || !string.IsNullOrWhiteSpace(note.Content))
			Notes.Add(note);
	}

	/// <summary>
	///		Notas asociadas al elemento
	/// </summary>
	public List<NoteModel> Notes { get; } = new();
}
