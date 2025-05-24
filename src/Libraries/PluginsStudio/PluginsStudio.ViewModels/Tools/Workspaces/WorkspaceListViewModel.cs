using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Workspaces;

/// <summary>
///		ViewModel con los datos de espacios de trabajo
/// </summary>
public class WorkspaceListViewModel : BaseObservableObject
{
	// Constantes privadas
	private const string DefaultWorkSpace = "Default";
	private const string WorkspaceExtension = "sxml";
	// Variables privadas
	private ObservableCollection<WorkSpaceViewModel> _items = default!;
	private WorkSpaceViewModel? _selectedItem;

	public WorkspaceListViewModel(PluginsStudioViewModel mainViewModel)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		Items = new ObservableCollection<WorkSpaceViewModel>();
		// Asigna los comandos
		NewWorkspaceCommand = new BaseCommand(_ => NewWorkspace());
		DeleteWorkspaceCommand = new BaseCommand(_ => DeleteWorkspace());
	}

	/// <summary>
	///		Obtiene la lista de espacios de trabajo
	/// </summary>
	public void Load(string path)
	{
		// Guarda el directorio
		Path = path;
		// Limpia los espacios de trabajo
		Items.Clear();
		// Añade el espacio de trabajo predeterminado
		Add(DefaultWorkSpace);
		// Obtiene los espacios de trabajo
		if (Directory.Exists(path))
			foreach (string childPath in Directory.EnumerateDirectories(path))
				if (!System.IO.Path.GetFileName(childPath).Equals(DefaultWorkSpace, StringComparison.CurrentCultureIgnoreCase))
					Add(System.IO.Path.GetFileName(childPath));
	}

	/// <summary>
	///		Selecciona un espacio de trabajo
	/// </summary>
	public void Select(string actual)
	{
		// Ajusta el espacio de trabajo al predeterminado si no se ha pasado nada
		if (string.IsNullOrWhiteSpace(actual))
			actual = DefaultWorkSpace;
		// Carga el espacio de trabajo
		if (!string.IsNullOrWhiteSpace(actual) && !actual.Equals(SelectedItem?.Name))
		{
			// Selecciona el espacio de trabajo
			foreach (WorkSpaceViewModel workSpace in Items!)
				if (workSpace.Name.Equals(actual, StringComparison.CurrentCultureIgnoreCase))
					SelectedItem = workSpace;
			// Si no se ha seleccionado uno, selecciona el predeterminado
			if (SelectedItem is null && Items.Count > 0)
				SelectedItem = Items[0];
			// Selecciona el espacio de trabajo
			if (SelectedItem is not null)
			{
				// Carga el espacio de trabajo
				SelectedItem.Load();
				// Actualiza el espacio de trabajo
				MainViewModel.SelectWorkspace(SelectedItem.Name);
			}
		}
	}

	/// <summary>
	///		Crea un nuevo espacio de trabajo
	/// </summary>
	private void NewWorkspace()
	{
		string workspace = string.Empty;

			if (MainViewModel.MainController.MainWindowController.SystemController.ShowInputString("Nombre del espacio de trabajo", ref workspace) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes &&
				!string.IsNullOrWhiteSpace(workspace))
			{
				if (Items.Any(item => item.Name.Equals(workspace, StringComparison.CurrentCultureIgnoreCase)))
					MainViewModel.MainController.MainWindowController.SystemController.ShowMessage($"Ya existe el espacio de trabajo '{workspace}'");
				else
				{
					// Crea el directorio
					LibHelper.Files.HelperFiles.MakePath(System.IO.Path.Combine(Path, workspace));
					// Añade el espacio de trabajo
					Add(workspace);
					// Cambia el Workspace
					Select(workspace);
					// Inicializa las carpetas del espacio de trabajo
					if (SelectedItem is not null)
						LibHelper.Files.HelperFiles.SaveTextFile(SelectedItem.FileName, string.Empty);
					// y lanza el evento de modificación
					MainViewModel.SelectWorkspace(workspace);
				}
			}
	}

	/// <summary>
	///		Añade un espacio de trabajo a la colección
	/// </summary>
	private void Add(string name)
	{
		Items.Add(new WorkSpaceViewModel(this, name, System.IO.Path.Combine(Path, name, $"{name}.{WorkspaceExtension}")));
	}

	/// <summary>
	///		Borra un espacio de trabajo
	/// </summary>
	private void DeleteWorkspace()
	{
		if (SelectedItem is not null && 
			MainViewModel.MainController.MainWindowController.SystemController.ShowQuestion($"¿Desea eliminar el espacio de trabajo '{SelectedItem.Name}'?"))
		{
			// Elimina el elemento en la lista
			Items.Remove(SelectedItem);
			// Borra el directorio
			LibHelper.Files.HelperFiles.KillPath(SelectedItem.Path, false);
			// Lanza el evento de carga del menú
			MainViewModel.SelectWorkspace(DefaultWorkSpace);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Directorio donde se encuentran los espacios de trabajo
	/// </summary>
	public string Path { get; private set; } = default!;

	/// <summary>
	///		Espacios de trabajo
	/// </summary>
	public ObservableCollection<WorkSpaceViewModel> Items
	{
		get { return _items; }
		set { CheckObject(ref _items!, value); }
	}

	/// <summary>
	///		Espacio de trabajo seleccionado
	/// </summary>
	public WorkSpaceViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Crea un nuevo espacio de trabajo
	/// </summary>
	public BaseCommand NewWorkspaceCommand { get; }

	/// <summary>
	///		Borra el espacio de trabajo seleccionado
	/// </summary>
	public BaseCommand DeleteWorkspaceCommand { get; }
}