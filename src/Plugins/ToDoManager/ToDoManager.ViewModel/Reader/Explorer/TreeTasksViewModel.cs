using Bau.Libraries.ToDoManager.Application.ToDo.Models;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader.Explorer;

/// <summary>
///		ViewModel para el árbol de tareas
/// </summary>
public class TreeTasksViewModel : PluginTreeViewModel
{   
	// Variables privadas
	private ControlHierarchicalViewModel _selectedNode;

	public TreeTasksViewModel(ToDoFileViewModel mainViewModel)
	{
		// Asigna las propiedades
		FileViewModel = mainViewModel;
		// Inicializa los comandos
		NewFolderCommand = new BaseCommand(_ => ExecuteAction(nameof(NewFolderCommand)), _ => CanExecuteAction(nameof(NewFolderCommand)))
								.AddListener(this, nameof(SelectedNode));
		NewGroupCommand = new BaseCommand(_ => ExecuteAction(nameof(NewGroupCommand)), _ => CanExecuteAction(nameof(NewGroupCommand)))
								.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void AddRootNodes()
	{
		AddChildFolderNodes(null, FileViewModel.ToDoManager.File.Root);
	}

	/// <summary>
	///		Añade los nodos de una carpeta
	/// </summary>
	private void AddChildFolderNodes(FolderNodeViewModel? parent, FolderModel folder)
	{
		// Ordena las carpetas
		folder.Folders.SortByName();
		// Añade las carpetas
		foreach (FolderModel child in folder.Folders)
		{
			FolderNodeViewModel node = new(this, FileViewModel, parent, child);

				// Añade el nodo
				if (parent != null)
					parent.Children.Add(node);
				else
					Children.Add(node);
				// Añade las carpetas hijo
				AddChildFolderNodes(node, child);
		}
		// Ordena las entradas
		folder.Groups.SortByName();
		// Añade las entradas
		foreach (GroupModel group in folder.Groups)
		{
			GroupNodeViewModel node = new(this, FileViewModel, null, group);

				// Añade el nodo
				if (parent != null)
					parent.Children.Add(node);
				else
					Children.Add(node);
		}
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	private void ExecuteAction(string action)
	{
		switch (action)
		{
			case nameof(NewFolderCommand):
					OpenFormUpdateFolder(null);
				break;
			case nameof(NewGroupCommand):
					OpenFormUpdateGroup(null);
				break;
			case nameof(DeleteCommand):
					if (SelectedNode != null)
					{
						if (SelectedNode is FolderNodeViewModel folderNode)
							DeleteFolder(folderNode.Folder);
						else if (SelectedNode is GroupNodeViewModel entryNode)
							DeleteGroup(entryNode.Group);
					}
				break;
		}
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action)
	{
		switch (action)
		{
			case nameof(NewFolderCommand):
			case nameof(NewGroupCommand):
				return IsSelectedFolder || SelectedNode == null;
			case nameof(OpenCommand):
			case nameof(DeleteCommand):
				return SelectedNode != null;
			default:
				return false;
		}
	}

	/// <summary>
	///		Abre la ventana de propiedades
	/// </summary>
	protected override void OpenProperties()
	{
		if (SelectedNode != null)
		{
			if (SelectedNode is FolderNodeViewModel folderNode)
				OpenFormUpdateFolder(folderNode.Folder);
			else if (SelectedNode is GroupNodeViewModel groupNode)
				OpenFormUpdateGroup(groupNode.Group);
		}
	}

	/// <summary>
	///		Abre el formulario de modificación / creación de una carpeta
	/// </summary>
	private void OpenFormUpdateFolder(FolderModel folder)
	{
		FolderModel parent = null;
		string folderName = string.Empty;

			// Obtiene la carpeta seleccionada
			if (folder == null)
			{
				if (SelectedNode is FolderNodeViewModel node)
					parent = node.Folder;
				else
					parent = FileViewModel.ToDoManager.File.Root;
			}
			else
				folderName = folder.Name;
			// Abre la ventana de modificación
			if (FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowInputString("Introduzca el nombre de la carpeta", 
																											ref folderName) == SystemControllerEnums.ResultType.Yes &&
				!string.IsNullOrWhiteSpace(folderName))
			{
				// Crea la carpeta si no existía
				if (folder == null)
				{
					// Crea la carpeta
					folder = new FolderModel();
					// La añade al padre
					parent?.Folders.Add(folder);
					// y abre el nodo
					if (SelectedNode is not null)
						SelectedNode.IsExpanded = true;
				}
				// Actualiza el nombre de la carpeta
				folder.Name = folderName;
				// Actualiza el árbol e indica que ha habido modificaciones
				SetUpdatedAndRefresh();
			}
	}

	/// <summary>
	///		Abre el formulario de modificación / creación de un grupo
	/// </summary>
	private void OpenFormUpdateGroup(GroupModel group)
	{
		if (!FileViewModel.CanUpdateSelectedGroup())
			FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Grabe los datos de la entrada actual antes de crear una nueva");
		else
		{
			// Obtiene la carpeta a la que se añade la entrada
			if (group == null)
			{
				FolderModel folder;

					// Crea el objeto
					group = new();
					group.Name = "Nueva entrada";
					// Asigna el grupo a la carpeta
					if (SelectedNode is FolderNodeViewModel node)
						folder = node.Folder;
					else
						folder = FileViewModel.ToDoManager.File.Root;
					// Añade la entrada a la carpeta
					folder.Groups.Add(group);
					// Actualiza el árbol e indica que ha habido modificaciones
					SetUpdatedAndRefresh();
			}
			// Actualiza la ventana
			FileViewModel.UpdateSelectedGroup(group);
		}
	}

	/// <summary>
	///		Modifica el valor que indica que se ha modificado y actualiza
	/// </summary>
	private void SetUpdatedAndRefresh()
	{
		// Indica que ha habido modificaciones
		FileViewModel.IsUpdated = true;
		// Actualiza los datos
		Refresh();
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	protected override void DeleteItem()
	{
		switch (SelectedNode)
		{
			case FolderNodeViewModel node:
					DeleteFolder(node.Folder);
				break;
			case GroupNodeViewModel node:
					DeleteGroup(node.Group);
				break;
		}
	}

	/// <summary>
	///		Borra una carpeta
	/// </summary>
	private void DeleteFolder(FolderModel folder)
	{
		if (folder != null && FileViewModel.MainViewModel.ViewsController.SystemController.ShowQuestion($"¿Realmente desea eliminar la carpeta '{folder.Name}'?"))
		{ 
			// Borra la carpeta
			FileViewModel.ToDoManager.File.Root.Delete(folder);
			// Actualiza el elemento seleccionado
			FileViewModel.DiscardSelectedGroup();
			// Actualiza el árbol e indica que ha habido modificaciones
			SetUpdatedAndRefresh();
		}
	}

	/// <summary>
	///		Borra un grupo
	/// </summary>
	private void DeleteGroup(GroupModel group)
	{
		if (group != null && FileViewModel.MainViewModel.ViewsController.SystemController.ShowQuestion($"¿Realmente desea eliminar el grupo '{group.Name}'?"))
		{ 
			// Borra la entrada
			FileViewModel.ToDoManager.File.Root.Delete(group);
			// Actualiza el elemento seleccionado
			FileViewModel.DiscardSelectedGroup();
			// Actualiza el árbol e indica que ha habido modificaciones
			SetUpdatedAndRefresh();
		}
	}

	/// <summary>
	///		Actualiza el árbol
	/// </summary>
	public void Refresh()
	{   
		Load();
	}

	/// <summary>
	///		Indica si está seleccionada una carpeta
	/// </summary>
	public bool IsSelectedFolder
	{
		get { return SelectedNode != null && SelectedNode is FolderNodeViewModel; }
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoFileViewModel FileViewModel { get; }

	/// <summary>
	///		Actualiza el nodo seleccionado
	/// </summary>
	public override ControlHierarchicalViewModel SelectedNode
	{
		get { return _selectedNode; }
		set
		{
			// Las entradas sólo se selecciona si no tiene ninguna modificación pendiente
			if (value is GroupNodeViewModel node && FileViewModel.CanUpdateSelectedGroup())
				FileViewModel.UpdateSelectedGroup(node.Group);
			// Siempe se selecciona el valor
			CheckObject(ref _selectedNode, value);
		}
	}

	/// <summary>
	///		Comando de nueva carpeta
	/// </summary>
	public BaseCommand NewFolderCommand { get; }

	/// <summary>
	///		Comando de nueva entrada
	/// </summary>
	public BaseCommand NewGroupCommand { get; }
}