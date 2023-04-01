using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels
{
	/// <summary>
	///		ViewModel principal de la aplicación
	/// </summary>
	public class PluginsStudioViewModel : BaseObservableObject
	{
		// Eventos públicos
		public event EventHandler? WorkspacesChanged;
		// Variables privadas
		private Tools.LastFiles.LastFilesListViewModel _lastFilesViewModel = default!;
		private Tools.Workspaces.WorkspaceListViewModel _workspacesViewModel = default!;
		private Tools.LogItems.LogListViewModel _logViewModel = default!;
		private Tools.Search.SearchFilesViewModel _searchFilesViewModel = default!;
		private Base.Interfaces.IDetailViewModel? _selectedDetailsViewModel;
		private Explorers.Files.TreeFilesViewModel _treeFoldersViewModel = default!;

		public PluginsStudioViewModel(Controllers.IPluginsStudioController pluginsStudioController)
		{
			// Asigna las propiedades
			PluginsStudioController = pluginsStudioController;
			// Inicializa los objetos principales
			LastFilesViewModel = new Tools.LastFiles.LastFilesListViewModel(this);
			WorkspacesViewModel = new Tools.Workspaces.WorkspaceListViewModel(this);
			LogViewModel = new Tools.LogItems.LogListViewModel(this);
			SearchFilesViewModel = new Tools.Search.SearchFilesViewModel(this);
			TreeFoldersViewModel = new Explorers.Files.TreeFilesViewModel(this);
			// Asigna los comandos
			SaveCommand = new BaseCommand(_ => Save(false), _ => CanSave())
									.AddListener(this, nameof(SelectedDetailsViewModel));
			SaveAsCommand = new BaseCommand(_ => Save(true), _ => CanSave())
									.AddListener(this, nameof(SelectedDetailsViewModel));
			SaveAllCommand = new BaseCommand(_ => SaveAll(), _ => CanSave())
									.AddListener(this, nameof(SelectedDetailsViewModel));
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
		public void OpenFile(string fileName)
		{
			if (IsImage(fileName))
				PluginsStudioController.OpenWindow(new Files.ImageViewModel(this, fileName));
			else if (IsBrowserFile(fileName))
				OpenWebBrowser(fileName);
			else
				PluginsStudioController.OpenWindow(new Files.FileTextViewModel(this, fileName));
		}

		/// <summary>
		///		Abre un navegador Web
		/// </summary>
		public void OpenWebBrowser(string url)
		{
			PluginsStudioController.OpenWindow(new Tools.Web.WebViewModel(this, url));
		}

		/// <summary>
		///		Comprueba si es un archivo que se puede abrir con el explorador
		/// </summary>
		private bool IsBrowserFile(string fileName)
		{
			return fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Comprueba si es un archivo de imagen
		/// </summary>
		private bool IsImage(string fileName)
		{
			return fileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".tiff", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Comprueba si puede guardar el contenido de la ventana
		/// </summary>
		private bool CanSave()
		{
			return SelectedDetailsViewModel != null;
		}

		/// <summary>
		///		Actualiza las ventanas
		/// </summary>
		internal void Refresh()
		{
			// Actualiza los árboles principales
			TreeFoldersViewModel.Load();
			// Indica que se actualicen los plugins
			PluginsStudioController.Refresh();
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
			foreach (Base.Interfaces.IDetailViewModel viewModel in PluginsStudioController.MainWindowController.GetOpenedDetails())
				if (viewModel.IsUpdated)
					viewModel.SaveDetails(false);
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.IPluginsStudioController PluginsStudioController { get; }

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
			set { CheckObject(ref _lastFilesViewModel, value); }
		}

		/// <summary>
		///		ViewModel de espacios de trabajo
		/// </summary>
		public Tools.Workspaces.WorkspaceListViewModel WorkspacesViewModel
		{
			get { return _workspacesViewModel; }
			set { CheckObject(ref _workspacesViewModel, value); }
		}

		/// <summary>
		///		ViewModel de log
		/// </summary>
		public Tools.LogItems.LogListViewModel LogViewModel
		{
			get { return _logViewModel; }
			set { CheckObject(ref _logViewModel, value); }
		}

		/// <summary>
		///		ViewModel de búsqueda de archivos
		/// </summary>
		public Tools.Search.SearchFilesViewModel SearchFilesViewModel
		{
			get { return _searchFilesViewModel; }
			set { CheckObject(ref _searchFilesViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de archivos
		/// </summary>
		public Explorers.Files.TreeFilesViewModel TreeFoldersViewModel
		{
			get { return _treeFoldersViewModel; }
			set { CheckObject(ref _treeFoldersViewModel, value); }
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
		///		Comando para actualizar los datos
		/// </summary>
		public BaseCommand RefreshCommand { get; }
	}
}
