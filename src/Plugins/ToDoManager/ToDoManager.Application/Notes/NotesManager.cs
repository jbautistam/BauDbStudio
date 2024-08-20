namespace Bau.Libraries.ToDoManager.Application.Notes;

/// <summary>
///		Manager de notas de la aplicación
/// </summary>
public class NotesManager
{
	public NotesManager(ToDoManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Carga las notas
	/// </summary>
	public void Load()
	{
		Notes.Notes.Clear();
		Notes.Notes.AddRange(new Repository.NotesRepository().Load(GetFileName(Manager.Folder)));
	}

	/// <summary>
	///		Graba las notas
	/// </summary>
	public void Save()
	{
		new Repository.NotesRepository().Save(GetFileName(Manager.Folder), Notes.Notes);
	}

	/// <summary>
	///		Obtiene el nombre del archivo
	/// </summary>
	private string GetFileName(string path) => Path.Combine(path, "Notes.ToDo.xml");

	/// <summary>
	///		Manager
	/// </summary>
	public ToDoManager Manager { get; }

	/// <summary>
	///		Notas
	/// </summary>
	public Models.NotesRootModel Notes { get; } = new();
}
