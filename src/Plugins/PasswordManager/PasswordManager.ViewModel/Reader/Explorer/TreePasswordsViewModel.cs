using Bau.Libraries.PasswordManager.Application.Models;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader.Explorer;

/// <summary>
///		ViewModel para el árbol de contraseñas
/// </summary>
public class TreePasswordsViewModel : PluginsStudio.ViewModels.Base.Explorers.PluginTreeViewModel
{   
	// Variables privadas
	private ControlHierarchicalViewModel? _selectedNode;

	public TreePasswordsViewModel(PasswordFileViewModel mainViewModel)
	{
		// Asigna las propiedades
		FileViewModel = mainViewModel;
		// Inicializa los comandos
		NewFolderCommand = new BaseCommand(_ => ExecuteAction(nameof(NewFolderCommand)), _ => CanExecuteAction(nameof(NewFolderCommand)))
								.AddListener(this, nameof(SelectedNode));
		NewEntryCommand = new BaseCommand(_ => ExecuteAction(nameof(NewEntryCommand)), _ => CanExecuteAction(nameof(NewEntryCommand)))
								.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Carga los nodos
	/// </summary>
	protected override void AddRootNodes()
	{
		AddChildFolderNodes(null, FileViewModel.PasswordManager.File.Root);
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
		folder.Entries.SortByName();
		// Añade las entradas
		foreach (EntryModel entry in folder.Entries)
		{
			EntryNodeViewModel node = new(this, FileViewModel, null, entry);

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
			case nameof(NewEntryCommand):
					OpenFormUpdateEntry(null);
				break;
			case nameof(DeleteCommand):
					if (SelectedNode != null)
					{
						if (SelectedNode is FolderNodeViewModel folderNode)
							DeleteFolder(folderNode.Folder);
						else if (SelectedNode is EntryNodeViewModel entryNode)
							DeleteEntry(entryNode.Entry);
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
			case nameof(NewEntryCommand):
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
			else if (SelectedNode is EntryNodeViewModel blogNode)
				OpenFormUpdateEntry(blogNode.Entry);
		}
	}

	/// <summary>
	///		Abre el formulario de modificación / creación de una carpeta
	/// </summary>
	private void OpenFormUpdateFolder(FolderModel? folder)
	{
		FolderModel? parent = null;
		string folderName = string.Empty;

			// Obtiene la carpeta seleccionada
			if (folder == null)
			{
				if (SelectedNode is FolderNodeViewModel node)
					parent = node.Folder;
				else
					parent = FileViewModel.PasswordManager.File.Root;
			}
			else
				folderName = folder.Name;
			// Abre la ventana de modificación
			if (FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowInputString("Introduzca el nombre de la carpeta", 
																											ref folderName) == SystemControllerEnums.ResultType.Yes &&
				!string.IsNullOrWhiteSpace(folderName))
			{
				// Crea la carpeta si no existía
				if (folder is null)
				{
					// Crea la carpeta
					folder = new FolderModel();
					// La añade al padre
					if (parent is not null)
						parent.Folders.Add(folder);
					else
						FileViewModel.PasswordManager.File.Root.Folders.Add(folder);
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
	///		Abre el formulario de modificación / creación de una entrada
	/// </summary>
	private void OpenFormUpdateEntry(EntryModel? entry)
	{
		if (!FileViewModel.CanUpdateSelectedEntry())
			FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Grabe los datos de la entrada actual antes de crear una nueva");
		else
		{
			// Obtiene la carpeta a la que se añade la entrada
			if (entry is null)
			{
				FolderModel folder;

					// Crea el objeto
					entry = new EntryModel();
					entry.Name = "Nueva entrada";
					// Asigna la entrada a la carpeta
					if (SelectedNode is FolderNodeViewModel node)
						folder = node.Folder;
					else
						folder = FileViewModel.PasswordManager.File.Root;
					// Añade la entrada a la carpeta
					folder.Entries.Add(entry);
					// Actualiza el árbol e indica que ha habido modificaciones
					SetUpdatedAndRefresh();
			}
			// Actualiza la ventana
			FileViewModel.UpdateSelectedEntry(entry);
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
			case EntryNodeViewModel node:
					DeleteEntry(node.Entry);
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
			FileViewModel.PasswordManager.File.Root.Delete(folder);
			// Actualiza el elemento seleccionado
			FileViewModel.DiscardSelectedEntry();
			// Actualiza el árbol e indica que ha habido modificaciones
			SetUpdatedAndRefresh();
		}
	}

	/// <summary>
	///		Borra un blog
	/// </summary>
	private void DeleteEntry(EntryModel entry)
	{
		if (entry != null && FileViewModel.MainViewModel.ViewsController.SystemController.ShowQuestion($"¿Realmente desea eliminar la entrada '{entry.Name}'?"))
		{ 
			// Borra la entrada
			FileViewModel.PasswordManager.File.Root.Delete(entry);
			// Actualiza el elemento seleccionado
			FileViewModel.DiscardSelectedEntry();
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
	public bool IsSelectedFolder => SelectedNode is FolderNodeViewModel;

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PasswordFileViewModel FileViewModel { get; }

	/// <summary>
	///		Actualiza el nodo seleccionado
	/// </summary>
	public override ControlHierarchicalViewModel? SelectedNode
	{
		get { return _selectedNode; }
		set
		{
			// Las entradas sólo se selecciona si no tiene ninguna modificación pendiente
			if (value is EntryNodeViewModel node && FileViewModel.CanUpdateSelectedEntry())
				FileViewModel.UpdateSelectedEntry(node.Entry);
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
	public BaseCommand NewEntryCommand { get; }
}