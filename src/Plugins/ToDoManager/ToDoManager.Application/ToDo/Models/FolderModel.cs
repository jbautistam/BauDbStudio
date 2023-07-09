namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Clase con los datos de una carpeta
/// </summary>
public class FolderModel : LibDataStructures.Base.BaseExtendedModel
{
	/// <summary>
	///		Borra una carpeta
	/// </summary>
	public void Delete(FolderModel folder)
	{
		if (Folders.Search(folder.GlobalId) != null)
			Folders.RemoveById(folder.GlobalId);
		else
			foreach (FolderModel childFolder in Folders)
				childFolder.Delete(folder);
	}

	/// <summary>
	///		Borra una entrada
	/// </summary>
	public bool Delete(GroupModel group)
	{
		bool deleted = false;

			// Busca la entrada y la borra
			if (Groups.Search(group.GlobalId) != null)
			{
				Groups.RemoveById(group.GlobalId);
				deleted = true;
			}
			else
				foreach (FolderModel childFolder in Folders)
					if (!deleted && childFolder.Delete(group))
						deleted = true;
			// Devuelve el valor que indica si se ha borrado
			return deleted;
	}

	/// <summary>
	///		Carpetas contenidas en esta carpeta
	/// </summary>
	public FoldersModelCollection Folders { get; } = new();

	/// <summary>
	///		Entradas de esta carpeta
	/// </summary>
	public GroupModelCollection Groups { get; } = new();
}
