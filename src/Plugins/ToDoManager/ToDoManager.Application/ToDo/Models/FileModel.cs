namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Clase con los datos de un archivo de listas
/// </summary>
public class FileModel
{
	/// <summary>
	///		Carpeta raíz con las carpetas contenidas en el archivo
	/// </summary>
	public FolderModel Root { get; } = new();
}
