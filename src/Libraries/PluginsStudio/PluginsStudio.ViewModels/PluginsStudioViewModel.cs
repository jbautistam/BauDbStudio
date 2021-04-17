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
		public event EventHandler WorkspacesChanged;
		// Variables privadas
		private Tools.LastFiles.LastFilesListViewModel _lastFilesViewModel;
		private Tools.Workspaces.WorkspaceListViewModel _workspacesViewModel;
		private Tools.Log.LogListViewModel _logViewModel;
		private Tools.Search.SearchFilesViewModel _searchFilesViewModel;
		private Base.Interfaces.IDetailViewModel _selectedDetailsViewModel;

		public PluginsStudioViewModel(Controllers.IPluginsStudioController mainStudioController)
		{
			// Asigna las propiedades
			MainStudioController = mainStudioController;
			// Inicializa los objetos principales
			LastFilesViewModel = new Tools.LastFiles.LastFilesListViewModel(this);
			WorkspacesViewModel = new Tools.Workspaces.WorkspaceListViewModel(this);
			LogViewModel = new Tools.Log.LogListViewModel(this);
			SearchFilesViewModel = new Tools.Search.SearchFilesViewModel(this);
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
		public void Load(string path, string workspace)
		{
			// Carga los espacios de trabajo
			WorkspacesViewModel.Load(path);
			// Selecciona el espacio de trabajo
			SelectWorkspace(workspace);
		}

		/// <summary>
		///		Lanza el evento de modificación de los workspaces
		/// </summary>
		public void SelectWorkspace(string workspace)
		{
			// Selecciona el workspace
			WorkspacesViewModel.Select(workspace);
			// Lanza el evento de modificación del espacio de trabajo seleccionado
			WorkspacesChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///		Abre el archivo
		/// </summary>
		internal void OpenFile(string fileName)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			foreach (Base.Interfaces.IDetailViewModel viewModel in MainStudioController.MainWindowController.GetOpenedDetails())
				if (viewModel.IsUpdated)
					viewModel.SaveDetails(false);
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.IPluginsStudioController MainStudioController { get; }

		/// <summary>
		///		ViewModel de detalles seleccionado en la ventana principal
		/// </summary>
		public Base.Interfaces.IDetailViewModel SelectedDetailsViewModel
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
		public Tools.Log.LogListViewModel LogViewModel
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
