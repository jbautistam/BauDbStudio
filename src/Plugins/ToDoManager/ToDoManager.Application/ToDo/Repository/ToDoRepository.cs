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
	private const string TagId = "Id";
	private const string TagTask = "Task";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagStatus = "Status";
	private const string TagPriority = "Priority";
	private const string TagMoskow = "Moskow";
	private const string TagNotes = "Notes";
	private const string TagTag = "Tag";
	private const string TagOrder = "Order";
	private const string TagCreatedAt = "CreatedAt";
	private const string TagDueAt = "DueAt";
	private const string TagFinishedAt = "FinishedAt";

	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	internal ToDoFileModel Load(string fileName)
	{
		ToDoFileModel file = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos
			if (fileML != null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagTask:
										file.Tasks.Add(LoadTask(nodeML));
									break;
								case TagTag:
										file.Tags.Add(LoadTag(nodeML));
									break;
							}
			// Reordena las tareas
			file.Tasks.SetOrder();
			// Devuelve el archivo leido
			return file;
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
			task.Status = rootML.Attributes[TagStatus].Value.GetEnum(TaskModel.StatusType.Planned);
			task.Priority = rootML.Attributes[TagPriority].Value.GetEnum(TaskModel.PriorityType.Normal);
			task.Moskow = rootML.Attributes[TagMoskow].Value.GetEnum(TaskModel.MoskowType.CouldHave);
			task.Order = rootML.Attributes[TagOrder].Value.GetInt(0);
			task.CreatedAt = rootML.Attributes[TagCreatedAt].Value.GetDateTime(DateTime.Now);
			task.DueAt = rootML.Attributes[TagDueAt].Value.GetDateTime();
			task.FinishedAt = rootML.Attributes[TagFinishedAt].Value.GetDateTime();
			// Carga los Id de etiquetas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagTag)
					task.TagsId.Add(nodeML.Attributes[TagId].Value.TrimIgnoreNull());
			// Devuelve la entrada
			return task;
	}

	/// <summary>
	///		Carga una etiqueta
	/// </summary>
	private TagModel LoadTag(MLNode rootML)
	{
		return new TagModel
						{
							GlobalId = rootML.Attributes[TagId].Value.TrimIgnoreNull(),
							Name = rootML.Attributes[TagName].Value.TrimIgnoreNull()
						};
	}

	/// <summary>
	///		Graba los datos en un archivo
	/// </summary>
	internal void Save(string fileName, ToDoFileModel file)
	{
		MLFile fileML = new MLFile();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade las tareas
			foreach (TaskModel task in file.Tasks)
				rootML.Nodes.Add(GetXmlTask(task));
			// Añade las etiquetas
			foreach (TagModel tag in file.Tags)
				rootML.Nodes.Add(GetXmlTag(tag));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene los datos del nodo de una tarea
	/// </summary>
	private MLNode GetXmlTask(TaskModel task)
	{
		MLNode rootML = new(TagTask);

			// Añade las propiedades
			rootML.Attributes.Add(TagId, task.GlobalId);
			rootML.Attributes.Add(TagName, task.Name);
			rootML.Nodes.Add(TagDescription, task.Description);
			rootML.Nodes.Add(TagNotes, task.Notes);
			rootML.Attributes.Add(TagStatus, task.Status.ToString());
			rootML.Attributes.Add(TagPriority, task.Priority.ToString());
			rootML.Attributes.Add(TagMoskow, task.Moskow.ToString());
			rootML.Attributes.Add(TagOrder, task.Order);
			rootML.Attributes.Add(TagCreatedAt, task.CreatedAt);
			rootML.Attributes.Add(TagDueAt, task.DueAt);
			rootML.Attributes.Add(TagFinishedAt, task.FinishedAt);
			// Añade las etiquetas
			foreach (string tagId in task.TagsId)
			{
				MLNode tagML = rootML.Nodes.Add(TagTag);

					tagML.Attributes.Add(TagId, tagId);
			}
			// Devuelve el nodo
			return rootML;
	}

	/// <summary>
	///		Obtiene el XML de una etiqueta
	/// </summary>
	private MLNode GetXmlTag(TagModel tag)
	{
		MLNode rootML = new(TagTag);

			// Añade los atributos
			rootML.Attributes.Add(TagId, tag.GlobalId);
			rootML.Attributes.Add(TagName, tag.Name);
			// Devuelve el nodo
			return rootML;
	}
}