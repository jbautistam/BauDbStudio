namespace Bau.Libraries.ToDoManager.Application.ToDo;

/// <summary>
///		Clase principal del manager de tareas
/// </summary>
public class ToDoFileManager
{
	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	public Models.ToDoFileModel Load(string fileName) => new Repository.ToDoRepository().Load(fileName);

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void Save(string fileName, Models.ToDoFileModel file)
	{
		new Repository.ToDoRepository().Save(fileName, file);
	}
}