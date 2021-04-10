using System;
using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels.Core.Tools.Workspaces
{
	/// <summary>
	///		ViewModel con los datos de espacios de trabajo
	/// </summary>
	public class WorkspaceListViewModel : BaseObservableObject
	{
		// Constantes privadas
		private const string DefaultWorkSpace = "Default";
		private const string WorkspaceExtension = "sxml";
		// Variables privadas
		private ObservableCollection<WorkSpaceViewModel> _items;
		private WorkSpaceViewModel _selectedItem;

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
			if (System.IO.Directory.Exists(path))
				foreach (string childPath in System.IO.Directory.EnumerateDirectories(path))
					foreach (string fileName in System.IO.Directory.GetFiles(childPath, $"*.{WorkspaceExtension}"))
						if (!System.IO.Path.GetFileName(childPath).Equals(DefaultWorkSpace, StringComparison.CurrentCultureIgnoreCase))
							Add(System.IO.Path.GetFileName(childPath));
		}

		/// <summary>
		///		Selecciona un espacio de trabajo
		/// </summary>
		public void Select(string actual)
		{
			if (!string.IsNullOrWhiteSpace(actual) && !actual.Equals(SelectedItem?.Name))
			{
				// Selecciona el espacio de trabajo
				foreach (WorkSpaceViewModel workSpace in Items)
					if (workSpace.Name.Equals(actual, StringComparison.CurrentCultureIgnoreCase))
						SelectedItem = workSpace;
				// Si no se ha seleccionado uno, selecciona el predeterminado
				if (SelectedItem == null && Items.Count > 0)
					SelectedItem = Items[0];
				// Actualiza el espacio de trabajo
				MainViewModel.SelectWorkspace(SelectedItem.Name);
			}
		}

		/// <summary>
		///		Crea un nuevo espacio de trabajo
		/// </summary>
		private void NewWorkspace()
		{
			string workspace = string.Empty;

				if (MainViewModel.MainStudioController.HostController.SystemController.ShowInputString("Nombre del espacio de trabajo", ref workspace) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					if (!string.IsNullOrWhiteSpace(workspace))
					{
						// Crea el directorio
						LibHelper.Files.HelperFiles.MakePath(System.IO.Path.Combine(Path, workspace));
						// Añade el espacio de trabajo
						Add(workspace);
						// Cambia el Workspace
						Select(workspace);
						// Graba un archivo vacío
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
			if (MainViewModel.MainStudioController.HostController.SystemController.ShowQuestion($"¿Desea eliminar el espacio de trabajo '{SelectedItem.Name}'?"))
			{
				// Borra el directorio
				LibHelper.Files.HelperFiles.KillPath(SelectedItem.Path);
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
		public string Path { get; private set; }

		/// <summary>
		///		Espacios de trabajo
		/// </summary>
		public ObservableCollection<WorkSpaceViewModel> Items
		{
			get { return _items; }
			set { CheckObject(ref _items, value); }
		}

		/// <summary>
		///		Espacio de trabajo seleccionado
		/// </summary>
		public WorkSpaceViewModel SelectedItem
		{
			get { return _selectedItem; }
			set { CheckObject(ref _selectedItem, value); }
		}

		/// <summary>
		///		Crea un nuevo espacio de trabajo
		/// </summary>
		public BaseCommand NewWorkspaceCommand { get; }

		/// <summary>
		///		Modifica el espacio de trabajo seleccionado
		/// </summary>
		public BaseCommand UpdateWorkspaceCommand { get; }

		/// <summary>
		///		Borra el espacio de trabajo seleccionado
		/// </summary>
		public BaseCommand DeleteWorkspaceCommand { get; }
	}
}