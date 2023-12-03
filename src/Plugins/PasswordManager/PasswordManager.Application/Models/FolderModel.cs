namespace Bau.Libraries.PasswordManager.Application.Models;

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
	public bool Delete(EntryModel entry)
	{
		bool deleted = false;

			// Busca la entrada y la borra
			if (Entries.Search(entry.GlobalId) != null)
			{
				Entries.RemoveById(entry.GlobalId);
				deleted = true;
			}
			else
				foreach (FolderModel childFolder in Folders)
					if (!deleted && childFolder.Delete(entry))
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
	public EntriesModelCollection Entries { get; } = new();
}
