using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.ToDoManager.Application.ToDo.Models;

namespace Bau.Libraries.ToDoManager.Application.ToDo.Repository;

/// <summary>
///		Repositorio para un archivo de ToDo
/// </summary>
internal class ToDoRepository
{
	// Constantes privadas
	private const string TagRoot = "BauToDo";
	private const string TagFolder = "Folder";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagId = "Id";
	private const string TagCreatedAt = "CreatedAt";
	private const string TagGroup = "Group";
	private const string TagNotes = "Notes";
	private const string TagPending = "Pending";
	private const string TagInProgress = "InProgress";
	private const string TagDone = "Done";
	private const string TagDiscard = "Discard";
	private const string TagTask = "Task";

	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	internal FileModel Load(string fileName)
	{
		FileModel file = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos
			if (fileML != null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagFolder:
										LoadFolders(file.Root, nodeML);
									break;
								case TagGroup:
										file.Root.Groups.Add(LoadGroup(nodeML));
									break;
							}
			// Devuelve el archivo leido
			return file;
	}

	/// <summary>
	///		Carga los datos de una carpeta
	/// </summary>
	private void LoadFolders(FolderModel parent, MLNode rootML)
	{
		foreach (MLNode nodeML in rootML.Nodes)
			switch (nodeML.Name)
			{
				case TagFolder:
						parent.Folders.Add(LoadFolder(nodeML));
					break;
				case TagGroup:
						parent.Groups.Add(LoadGroup(nodeML));
					break;
			}
	}

	/// <summary>
	///		Carga los datos de una carpeta
	/// </summary>
	private FolderModel LoadFolder(MLNode rootML)
	{
		FolderModel folder = new();

			// Asigna las propiedades
			folder.GlobalId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			folder.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
			// Carga los hijos
			LoadFolders(folder, rootML);
			// Devuelve la carpeta cargada
			return folder;
	}

	/// <summary>
	///		Carga los datos de un grupo
	/// </summary>
	private GroupModel LoadGroup(MLNode rootML)
	{
		GroupModel group = new();

			// Carga los datos del grupo
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagName:
							group.Name = nodeML.Value.TrimIgnoreNull();
						break;
					case TagDescription:
							group.Description = nodeML.Value.TrimIgnoreNull();
						break;
					case TagPending:
							group.Pending.AddRange(LoadTasks(nodeML));
						break;
					case TagInProgress:
							group.InProgress.AddRange(LoadTasks(nodeML));
						break;
					case TagDone:
							group.Done.AddRange(LoadTasks(nodeML));
						break;
					case TagDiscard:
							group.Discard.AddRange(LoadTasks(nodeML));
						break;
				}
			// Devuelve el grupo
			return group;
	}

	/// <summary>
	///		Carga la colección de tareas
	/// </summary>
	private TaskModelCollection LoadTasks(MLNode rootML)
	{
		TaskModelCollection tasks = new();

			// Carga los elementos
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagTask)
					tasks.Add(LoadTask(nodeML));
			// Devuelve la colección de elementos
			return tasks;
	}

	/// <summary>
	///		Carga los datos de una tarea
	/// </summary>
	private TaskModel LoadTask(MLNode rootML)
	{
		TaskModel task = new();

			// Asigna las propiedades
			task.GlobalId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			task.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
			task.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
			task.Notes = rootML.Nodes[TagNotes].Value.TrimIgnoreNull();
			task.CreatedAt = rootML.Attributes[TagCreatedAt].Value.GetDateTime(DateTime.Now);
			// Devuelve la entrada
			return task;
	}

	/// <summary>
	///		Graba los datos en un archivo
	/// </summary>
	internal void Save(string fileName, FileModel file)
	{
		MLFile fileML = new MLFile();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade las carpetas
			foreach (FolderModel folder in file.Root.Folders)
				rootML.Nodes.Add(GetXmlFolder(folder));
			// Añade los grupos
			foreach (GroupModel group in file.Root.Groups)
				rootML.Nodes.Add(GetXmlGroup(group));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene los nodos de una carpeta
	/// </summary>
	private MLNode GetXmlFolder(FolderModel folder)
	{
		MLNode rootML = new(TagFolder);

			// Añade los datos de la carpeta
			rootML.Attributes.Add(TagId, folder.GlobalId);
			rootML.Attributes.Add(TagName, folder.Name);
			// Crea los nodos de las carpetas hija
			foreach (FolderModel child in folder.Folders)
				rootML.Nodes.Add(GetXmlFolder(child));
			// Añade los grupos
			foreach (GroupModel group in folder.Groups)
				rootML.Nodes.Add(GetXmlGroup(group));
			// Devuelve el nodo
			return rootML;
	}

	/// <summary>
	///		Obtiene los datos de un nodo de grupo
	/// </summary>
	private MLNode GetXmlGroup(GroupModel group)
	{
		MLNode rootML = new(TagGroup);

			// Añade los nodos
			rootML.Nodes.Add(TagName, group.Name);
			rootML.Nodes.Add(TagDescription, group.Description);
			// Añade los grupos
			rootML.Nodes.Add(GetXmlTasks(TagPending, group.Pending));
			rootML.Nodes.Add(GetXmlTasks(TagInProgress, group.InProgress));
			rootML.Nodes.Add(GetXmlTasks(TagDone, group.Done));
			rootML.Nodes.Add(GetXmlTasks(TagDiscard, group.Discard));
			// Devuelve los datos del nodo
			return rootML;
	}

	/// <summary>
	///		Obtiene los nodos de una serie de tareas
	/// </summary>
	private MLNode GetXmlTasks(string tag, TaskModelCollection items)
	{
		MLNode nodeML = new(tag);

			// Añade los elementos
			foreach (TaskModel item in items)
				nodeML.Nodes.Add(GetXmlTask(item));
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene los datos del nodo de una tarea
	/// </summary>
	private MLNode GetXmlTask(TaskModel ToDo)
	{
		MLNode rootML = new(TagTask);

			// Añade las propiedades
			rootML.Attributes.Add(TagId, ToDo.GlobalId);
			rootML.Nodes.Add(TagName, ToDo.Name);
			rootML.Nodes.Add(TagDescription, ToDo.Description);
			rootML.Nodes.Add(TagNotes, ToDo.Notes);
			rootML.Attributes.Add(TagCreatedAt, ToDo.CreatedAt);
			// Devuelve el nodo
			return rootML;
	}
}