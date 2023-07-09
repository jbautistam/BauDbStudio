namespace Bau.Libraries.ToDoManager.Application.ToDo.Models;

/// <summary>
///		Colección de <see cref="FolderModel"/>
/// </summary>
public class FoldersModelCollection : LibDataStructures.Base.BaseExtendedModelCollection<FolderModel>
{
	/// <summary>
	///		Añade una carpeta
	/// </summary>
	public FolderModel Add(string name)
	{
		FolderModel folder = new FolderModel { Name = name };

			// Añade la carpeta a la colección
			Add(folder);
			// Devuelve la carpeta
			return folder;
	}

	/// <summary>
	///		Borra un grupo de la carpeta
	/// </summary>
	public bool Delete(GroupModel group)
	{
		bool deleted = false;

			// Recorre las carpetas borrando la entrada
			foreach (FolderModel child in this)
				if (!deleted)
					deleted = child.Delete(group);
			// Devuelve el valor que indica si se ha borrado
			return deleted;
	}
}
