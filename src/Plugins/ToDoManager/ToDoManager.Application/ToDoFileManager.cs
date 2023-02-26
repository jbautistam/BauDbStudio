namespace Bau.Libraries.ToDoManager.Application;

/// <summary>
///		Clase principal del manager de tareas
/// </summary>
public class ToDoFileManager
{
	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	public void Load(string fileName)
	{
		File = new Repository.ToDoRepository().Load(fileName);
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void Save(string fileName)
	{
		new Repository.ToDoRepository().Save(fileName, File);
	}

	/// <summary>
	///		Archivo con los elementos de tarea
	/// </summary>
	public Models.FileModel File { get; private set; } = new();
}