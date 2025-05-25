using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Commands;

namespace Bau.Libraries.PluginsStudio.ViewModels;

/// <summary>
///		ViewModel principal de la aplicación
/// </summary>
public class PluginsStudioViewModel : BaseObservableObject
{
	// Constantes públicas
	public const string PluginName = nameof(PluginsStudioViewModel);
	// Eventos públicos
	public event EventHandler? WorkspacesChanged;
	// Variables privadas
	private Tools.LastFiles.LastFilesListViewModel _lastFilesViewModel = default!;
	private Tools.Workspaces.WorkspaceListViewModel _workspacesViewModel = default!;
	private Tools.LogItems.LogListViewModel _logViewModel = default!;
	private Base.Interfaces.IDetailViewModel? _selectedDetailsViewModel;
	private Explorers.Files.TreeFilesViewModel _treeFoldersViewModel = default!;
	private TasksQueue.TasksQueueListViewModel _tasksQueueListViewModel = default!;
	private Windows.WindowsCollectionViewModel _windowsViewModel = default!;

	public PluginsStudioViewModel(Controllers.IPluginsStudioController pluginsStudioController)
	{
		// Asigna las propiedades
		MainController = pluginsStudioController;
		// Inicializa los objetos principales
		LastFilesViewModel = new Tools.LastFiles.LastFilesListViewModel(this);
		WorkspacesViewModel = new Tools.Workspaces.WorkspaceListViewModel(this);
		LogViewModel = new Tools.LogItems.LogListViewModel(this);
		TasksQueueListViewModel = new TasksQueue.TasksQueueListViewModel(this);
		TreeFoldersViewModel = new Explorers.Files.TreeFilesViewModel(this);
		WindowsViewModel = new Windows.WindowsCollectionViewModel(this);
		// Asigna los comandos
		SaveCommand = new BaseCommand(_ => Save(false), _ => CanSave())
								.AddListener(this, nameof(SelectedDetailsViewModel));
		SaveAsCommand = new BaseCommand(_ => Save(true), _ => CanSave())
								.AddListener(this, nameof(SelectedDetailsViewModel));
		SaveAllCommand = new BaseCommand(_ => SaveAll(), _ => CanSave())
								.AddListener(this, nameof(SelectedDetailsViewModel));
		DeleteCommand = new BaseCommand(_ => Delete(), _ => CanDelete());
		ExecuteCommand = new BaseCommand(_ => Execute(), _ => CanExecute());
		RefreshCommand = new BaseCommand(_ => Refresh());
	}

	/// <summary>
	///		Carga los datos
	/// </summary>
	public void Load(string path)
	{
		WorkspacesViewModel.Load(path);
	}

	/// <summary>
	///		Lanza el evento de modificación de los workspaces
	/// </summary>
	public void SelectWorkspace(string workspace)
	{
		// Selecciona el workspace
		WorkspacesViewModel.Select(workspace);
		// Carga el árbol de carpetas / archivos
		RefreshFiles();
		// Lanza el evento de modificación del espacio de trabajo seleccionado
		WorkspacesChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Actualiza el árbol de archivos
	/// </summary>
	public void RefreshFiles()
	{
		TreeFoldersViewModel.Load();
	}

	/// <summary>
	///		Abre el archivo
	/// </summary>
	public void OpenFile(string fileName, string mask)
	{
		if (IsBrowserFile(fileName))
			OpenWebBrowser(fileName);
		else
			MainController.OpenWindow(new Files.FileTextViewModel(this, fileName, mask));
	}

	/// <summary>
	///		Abre un navegador Web
	/// </summary>
	public void OpenWebBrowser(string url)
	{
		MainController.OpenWindow(new Tools.Web.WebViewModel(this, url));
	}

	/// <summary>
	///		Abre un navegador Web con un texto HTML
	/// </summary>
	public void OpenWebBrowserWithHtml(string html)
	{
		Tools.Web.WebViewModel viewModel = new(this, string.Empty);

			// Abre la ventana con el navegador
			MainController.OpenWindow(viewModel);
			// Muestra el HTML en el navegador
			viewModel.Html = html;
	}

	/// <summary>
	///		Comprueba si es un archivo que se puede abrir con el explorador
	/// </summary>
	private bool IsBrowserFile(string fileName) => fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase);

	/// <summary>
	///		Comprueba si puede guardar el contenido de la ventana
	/// </summary>
	private bool CanSave() => SelectedDetailsViewModel != null;

	/// <summary>
	///		Actualiza las ventanas
	/// </summary>
	internal void Refresh()
	{
		// Actualiza los árboles principales
		TreeFoldersViewModel.Load();
		// Indica que se actualicen los plugins
		MainController.Refresh();
	}

	/// <summary>
	///		Graba el viewModel activo
	/// </summary>
	private void Save(bool newName)
	{
		if (SelectedDetailsViewModel != null)
			SelectedDetailsViewModel.SaveDetails(newName);
	}

	/// <summary>
	///		Graba todos las ventanas de edición abiertas
	/// </summary>
	private void SaveAll()
	{
		foreach (Base.Interfaces.IDetailViewModel viewModel in MainController.MainWindowController.GetOpenedDetails())
			if (viewModel.IsUpdated)
				viewModel.SaveDetails(false);
	}

	/// <summary>
	///		Borra los datos de la ventana actual
	/// </summary>
	private void Delete()
	{
		if (MainController.MainWindowController.GetActiveDetails() is Base.Interfaces.IDetailViewModel details)
			details.Execute(new ExternalCommand(ExternalCommand.ExternalCommandType.Delete));
		else if (MainController.MainWindowController.GetActivePane() is Base.Interfaces.IPaneViewModel pane)
			pane.Execute(new ExternalCommand(ExternalCommand.ExternalCommandType.Delete));
	}

	/// <summary>
	///		Comprueba si puede borrar los datos
	/// </summary>
	private bool CanDelete() => true;

	/// <summary>
	///		Ejecuta los datos de la ventana actual
	/// </summary>
	private void Execute()
	{
		if (MainController.MainWindowController.GetActiveDetails() is Base.Interfaces.IDetailViewModel details)
			details.Execute(new ExternalCommand(ExternalCommand.ExternalCommandType.Execute));
		else if (MainController.MainWindowController.GetActivePane() is Base.Interfaces.IPaneViewModel pane)
			pane.Execute(new ExternalCommand(ExternalCommand.ExternalCommandType.Execute));
	}

	/// <summary>
	///		Comprueba si puede ejecutar
	/// </summary>
	private bool CanExecute() => true;

	/// <summary>
	///		Controlador principal
	/// </summary>
	public Controllers.IPluginsStudioController MainController { get; }

	/// <summary>
	///		ViewModel de detalles seleccionado en la ventana principal
	/// </summary>
	public Base.Interfaces.IDetailViewModel? SelectedDetailsViewModel
	{
		get { return _selectedDetailsViewModel; }
		set { CheckObject(ref _selectedDetailsViewModel, value); }
	}

	/// <summary>
	///		ViewModel de los últimos archivos abiertos
	/// </summary>
	public Tools.LastFiles.LastFilesListViewModel LastFilesViewModel
	{
		get { return _lastFilesViewModel; }
		set { CheckObject(ref _lastFilesViewModel!, value); }
	}

	/// <summary>
	///		ViewModel de espacios de trabajo
	/// </summary>
	public Tools.Workspaces.WorkspaceListViewModel WorkspacesViewModel
	{
		get { return _workspacesViewModel; }
		set { CheckObject(ref _workspacesViewModel!, value); }
	}

	/// <summary>
	///		ViewModel de log
	/// </summary>
	public Tools.LogItems.LogListViewModel LogViewModel
	{
		get { return _logViewModel; }
		set { CheckObject(ref _logViewModel!, value); }
	}

	/// <summary>
	///		ViewModel del árbol de archivos
	/// </summary>
	public Explorers.Files.TreeFilesViewModel TreeFoldersViewModel
	{
		get { return _treeFoldersViewModel; }
		set { CheckObject(ref _treeFoldersViewModel!, value); }
	}

	/// <summary>
	///		ViewModel de la lista de procesos
	/// </summary>
	public TasksQueue.TasksQueueListViewModel TasksQueueListViewModel
	{
		get { return _tasksQueueListViewModel; }
		set { CheckObject(ref _tasksQueueListViewModel!, value); }
	}

	/// <summary>
	///		ViewModel de ventanas abiertas
	/// </summary>
	public Windows.WindowsCollectionViewModel WindowsViewModel
	{
		get { return _windowsViewModel; }
		set { CheckObject(ref _windowsViewModel!, value); }
	}

	/// <summary>
	///		Comando para grabar el elemento actual
	/// </summary>
	public BaseCommand SaveCommand { get; }

	/// <summary>
	///		Comando para grabar el elemento actual con un nuevo nombre
	/// </summary>
	public BaseCommand SaveAsCommand { get; }

	/// <summary>
	///		Comando para grabar todos los elementos abiertos
	/// </summary>
	public BaseCommand SaveAllCommand { get; }

	/// <summary>
	///		Comando para borrar datos del elemento abierto
	/// </summary>
	public BaseCommand DeleteCommand { get; }

	/// <summary>
	///		Comando para ejecutar un script sobre plugins
	/// </summary>
	public BaseCommand ExecuteCommand { get; }

	/// <summary>
	///		Comando para actualizar los datos
	/// </summary>
	public BaseCommand RefreshCommand { get; }
}
