namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Workspaces;

/// <summary>
///		ViewModel con los datos de un espacio de trabajo
/// </summary>
public class WorkSpaceViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _name = string.Empty, _fileName = string.Empty;

	public WorkSpaceViewModel(WorkspaceListViewModel listViewModel, string name, string fileName)
	{
		ListViewModel = listViewModel;
		Name = name;
		FileName = fileName;
	}

	/// <summary>
	///		Carga el espacio de trabajo
	/// </summary>
	internal void Load()
	{
		// Limpia la lista de carpetas
		Folders.Clear();
		// Carga el archivo de solución
		Folders.AddRange(new Repository.WorkspaceRepository().Load(FileName));
	}

	/// <summary>
	///		Graba el espacio de trabajo
	/// </summary>
	internal void Save()
	{
		new Repository.WorkspaceRepository().Save(FileName, Folders);
	}

	/// <summary>
	///		Añade una carpeta al workspace
	/// </summary>
	internal void AddFolder(string folder)
	{
		if (!string.IsNullOrWhiteSpace(folder) && Folders.FirstOrDefault(item => item.Equals(folder, StringComparison.CurrentCultureIgnoreCase)) is null)
		{
			// Añade la carpeta
			Folders.Add(folder);
			// Graba el espacio de trabajo
			Save();
		}
	}

	/// <summary>
	///		Elimina una carpeta
	/// </summary>
	internal void RemoveFolder(string folder)
	{
		if (!string.IsNullOrWhiteSpace(folder))
		{
			bool deleted = false;

				// Borra la carpeta
				for (int index = Folders.Count - 1; index >= 0; index--)
					if (Folders[index].Equals(folder, StringComparison.CurrentCultureIgnoreCase))
					{
						Folders.RemoveAt(index);
						deleted = true;
					}
				// Si se ha eliminado algo, se graba el espacio de trabajo
				if (deleted)
					Save();
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public WorkspaceListViewModel ListViewModel { get; }

	/// <summary>
	///		Nombre del espacio de trabajo
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Directorio del archivo
	/// </summary>
	public string Path => System.IO.Path.GetDirectoryName(FileName) ?? string.Empty;

	/// <summary>
	///		Carpetas asociadas el espacio de trabajo
	/// </summary>
	public List<string> Folders { get; } = new List<string>();
}