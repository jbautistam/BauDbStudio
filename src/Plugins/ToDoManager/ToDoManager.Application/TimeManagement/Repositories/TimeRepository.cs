using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.Application.TimeManagement.Repositories;

/// <summary>
///		Repositorio para <see cref="Models.ProjectModel"/>
/// </summary>
internal class TimeRepository
{
	// Constantes privadas
	private const string TagRoot = "TimeControl";
	private const string TagProject = "Project";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagRemarks = "Remarks";
	private const string TagTask = "Task";
	private const string TagTime = "Time";
	private const string TagStart = "Start";
	private const string TagEnd = "End";

	/// <summary>
	///		Carga los datos del archivo
	/// </summary>
	internal TimeControlModel Load(string folder, DateOnly date)
	{
		TimeControlModel timeControl = new(date);
		MLFile? fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(GetFileName(folder, date));

			// Carga los datos del archivo
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagProject)
								timeControl.Projects.Add(LoadProject(nodeML));
			// Devuelve el objeto
			return timeControl;
	}

	/// <summary>
	///		Carga los datos del proyecto
	/// </summary>
	private ProjectModel LoadProject(MLNode rootML)
	{
		ProjectModel project = new()
								{
									Name = rootML.Nodes[TagName].Value.TrimIgnoreNull(),
									Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull()
								};

			// Carga las tareas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagTask)
					project.Tasks.Add(LoadTask(project, nodeML));
			// Devuelve los datos
			return project;
	}

	/// <summary>
	///		Carga los datos de una tarea
	/// </summary>
	private TaskModel LoadTask(ProjectModel project, MLNode rootML)
	{
		TaskModel task = new(project)
							{
								Name = rootML.Nodes[TagName].Value.TrimIgnoreNull(),
								Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull()
							};

			// Carga las horas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagTime)
					task.Times.Add(LoadTime(task, nodeML));
			// Devuelve los datos
			return task;
	}

	/// <summary>
	///		Carga los datos de la hora
	/// </summary>
	private TimeModel LoadTime(TaskModel task, MLNode rootML)
	{
		return new TimeModel(task)
						{
							Remarks = rootML.Nodes[TagRemarks].Value.TrimIgnoreNull(),
							Start = rootML.Attributes[TagStart].Value.GetDateTime(DateTime.Now),
							End = rootML.Attributes[TagEnd].Value.GetDateTime(DateTime.Now)
						};
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	internal void Save(TimeControlModel scheduler, string folder)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los datos de proyectos
			foreach (ProjectModel project in scheduler.Projects)
				rootML.Nodes.Add(GetNodeProject(project));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(GetFileName(folder, scheduler.Date), fileML);
	}

	/// <summary>
	///		Obtiene el XML del proyecto
	/// </summary>
	private MLNode GetNodeProject(ProjectModel project)
	{
		MLNode rootML = new(TagProject);

			// Asigna las propiedas
			rootML.Nodes.Add(TagName, project.Name);
			rootML.Nodes.Add(TagDescription, project.Description);
			// Añade las tareas
			foreach (TaskModel task in project.Tasks)
				rootML.Nodes.Add(GetNodeTask(task));
			// Devuelve el nodo
			return rootML;
	}

	/// <summary>
	///		Obtiene el XML de la tarea
	/// </summary>
	private MLNode GetNodeTask(TaskModel task)
	{
		MLNode rootML = new(TagTask);

			// Asigna las propiedades
			rootML.Nodes.Add(TagName, task.Name);
			rootML.Nodes.Add(TagDescription, task.Description);
			// Añade los nodos de horas
			foreach (TimeModel time in task.Times)
				rootML.Nodes.Add(GetNodeTime(time));
			// Devuelve el nodo
			return rootML;
	}

	/// <summary>
	///		Obtiene el XML del tiempo
	/// </summary>
	private MLNode GetNodeTime(TimeModel time)
	{
		MLNode root = new(TagTime);

			// Añade los datos
			root.Nodes.Add(TagRemarks, time.Remarks);
			root.Attributes.Add(TagStart, time.Start);
			root.Attributes.Add(TagEnd, time.End);
			// Devuelve el nodo
			return root;
	}

	/// <summary>
	///		Obtiene el nombre de archivo
	/// </summary>
	private string GetFileName(string folder, DateOnly date) => Path.Combine(folder, $"{date.Year:0000}/{date.Month:00}/schedule_{date:yyyy-MM-dd}.time.xml");
}