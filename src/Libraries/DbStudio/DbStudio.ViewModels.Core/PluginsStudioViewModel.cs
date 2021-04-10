using System;

namespace Bau.Libraries.DbStudio.ViewModels.Core
{
	/// <summary>
	///		ViewModel principal de la aplicación
	/// </summary>
	public class PluginsStudioViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Eventos públicos
		public event EventHandler WorkspacesChanged;
		// Variables privadas
		private Tools.Workspaces.WorkspaceListViewModel _workspacesViewModel;

		public PluginsStudioViewModel(Controllers.IDbStudioCoreController mainStudioController)
		{
			// Asigna las propiedades
			MainStudioController = mainStudioController;
			// Inicializa los objetos principales
			WorkspacesViewModel = new Tools.Workspaces.WorkspaceListViewModel(this);
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
		///		Controlador principal
		/// </summary>
		public Controllers.IDbStudioCoreController MainStudioController { get; }

		/// <summary>
		///		ViewModel de espacios de trabajo
		/// </summary>
		public Tools.Workspaces.WorkspaceListViewModel WorkspacesViewModel
		{
			get { return _workspacesViewModel; }
			set { CheckObject(ref _workspacesViewModel, value); }
		}
	}
}
