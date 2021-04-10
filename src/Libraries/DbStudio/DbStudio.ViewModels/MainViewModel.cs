using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels
{
	/// <summary>
	///		ViewModel principal
	/// </summary>
	public class MainViewModel : BaseObservableObject
	{
		// Constantes privadas
		internal const string MaskFiles = "Archivos de solución (*.dbsln)|*.dbsln|Todos los archivos (*.*)|*.*";
		// Variables privadas
		private string _text;
		private Core.Interfaces.IDetailViewModel _selectedDetailsViewModel;
		private Tools.LogListViewModel _logViewModel;
		private Tools.LastFilesListViewModel _lastFilesViewModel;
		private Tools.Search.SearchFilesViewModel _searchFilesViewModel;

		public MainViewModel(Core.PluginsStudioViewModel pluginsStudioViewModel, Controllers.IDbStudioController mainController)
		{
			// Título de la aplicación
			Text = mainController.AppName;
			// Asigna las propiedades
			PluginsStudioViewModel = pluginsStudioViewModel;
			MainController = mainController;
			// Inicializa la solución
			SolutionViewModel = new Solutions.SolutionViewModel(this);
			// Inicializa los objetos principales
			LogViewModel = new Tools.LogListViewModel(this);
			LastFilesViewModel = new Tools.LastFilesListViewModel(this);
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
		public void Load()
		{
			SolutionViewModel.Load(PluginsStudioViewModel.WorkspacesViewModel.SelectedItem.Path);
		}

		/// <summary>
		///		Graba la solución
		/// </summary>
		internal void SaveSolution()
		{
			SolutionViewModel.Save(PluginsStudioViewModel.WorkspacesViewModel.SelectedItem.Path);
		}

		/// <summary>
		///		Actualiza el árbol
		/// </summary>
		internal void Refresh()
		{
			Load();
		}

		/// <summary>
		///		Comprueba si puede guardar el contenido de la ventana
		/// </summary>
		private bool CanSave()
		{
			return SelectedDetailsViewModel != null;
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
			foreach (Core.Interfaces.IDetailViewModel viewModel in MainController.GetOpenedDetails())
				if (viewModel.IsUpdated)
					viewModel.SaveDetails(false);
		}

		/// <summary>
		///		Solicita al usuario un nombre de archivos. Guarda el directorio seleccionado para que la próxima vez que se consulte
		///	por un archivo, se vaya directamente a ese directorio
		/// </summary>
		internal string OpenDialogSave(string suggestedFileName, string mask, string defaultExtension)
		{
			string path = GetPath(suggestedFileName);
			string fileName = MainController.HostController.DialogsController.OpenDialogSave(path, mask, suggestedFileName, defaultExtension);

				// Si se ha escogido un nombre de archivo se guarda el último directorio seleccionado
				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
					LastPathSelected = System.IO.Path.GetDirectoryName(fileName);
				// Devuelve el nombre de archivo
				return fileName;
		}

		/// <summary>
		///		Obtiene el directorio inicial de grabación de un archivo
		/// </summary>
		private string GetPath(string suggestedFileName)
		{
			string path = string.Empty;

				// Obtiene el directorio
				if (!string.IsNullOrWhiteSpace(suggestedFileName))
				{
					path = System.IO.Path.GetDirectoryName(suggestedFileName);
					if (string.IsNullOrWhiteSpace(path) || path.Equals(suggestedFileName, StringComparison.CurrentCultureIgnoreCase))
						path = LastPathSelected;
				}
				// Si no se ha definido ningún directorio, se coge el de la solución
				if (string.IsNullOrWhiteSpace(path))
					path = SolutionViewModel.Solution.Path;
				// Devuelve el directorio
				return path;
		}

		/// <summary>
		///		ViewModel de PluginsStudio
		/// </summary>
		public Core.PluginsStudioViewModel PluginsStudioViewModel { get; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.IDbStudioController MainController { get; }

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public Solutions.SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		ViewModel de detalles seleccionado en la ventana principal
		/// </summary>
		public Core.Interfaces.IDetailViewModel SelectedDetailsViewModel
		{
			get { return _selectedDetailsViewModel; }
			set { CheckObject(ref _selectedDetailsViewModel, value); }
		}

		/// <summary>
		///		Título de la ventana
		/// </summary>
		public string Text 
		{
			get { return _text; }
			set { CheckProperty(ref _text, value); }
		}

		/// <summary>
		///		ViewModel de log
		/// </summary>
		public Tools.LogListViewModel LogViewModel
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
		///		ViewModel de los últimos archivos abiertos
		/// </summary>
		public Tools.LastFilesListViewModel LastFilesViewModel
		{
			get { return _lastFilesViewModel; }
			set { CheckObject(ref _lastFilesViewModel, value); }
		}

		/// <summary>
		///		Ultimo directorio seleccionado al abrir / grabar un archivo
		/// </summary>
		public string LastPathSelected { get; set; }

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
