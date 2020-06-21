using System;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions
{
	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public class SolutionViewModel : BaseObservableObject
	{
		// Variables privadas
		private string _workspace;
		private Explorers.Connections.TreeConnectionsViewModel _treeConnectionsViewModel;
		private Explorers.Files.TreeFilesViewModel _treeFoldersViewModel;
		private Explorers.Cloud.TreeStorageViewModel _treeStoragesViewModel;
		private Details.Connections.ConnectionExecutionViewModel _connectionsViewModel;

		public SolutionViewModel(MainViewModel mainViewModel, string workspace)
		{
			// Asigna las propiedades
			MainViewModel = mainViewModel;
			Workspace = workspace;
			TreeConnectionsViewModel = new Explorers.Connections.TreeConnectionsViewModel(this);
			TreeFoldersViewModel = new Explorers.Files.TreeFilesViewModel(this);
			TreeStoragesViewModel = new Explorers.Cloud.TreeStorageViewModel(this);
			ConnectionExecutionViewModel = new Details.Connections.ConnectionExecutionViewModel(this);
			// Asigna los comandos
			NewWorkspaceCommand = new BaseCommand(_ => NewWorkspace());
			DeleteWorkspaceCommand = new BaseCommand(_ => DeleteWorkspace());
			CreateTestXmlCommand = new BaseCommand(async _ => await CreateTestXmlAsync());
		}

		/// <summary>
		///		Carga un archivo de solución
		/// </summary>
		public void Load()
		{
			// Carga la solución
			Solution = MainViewModel.Manager.LoadConfiguration(Workspace);
			// Carga los exploradores
			TreeConnectionsViewModel.Load();
			ConnectionExecutionViewModel.Initialize();
			TreeFoldersViewModel.Load();
			TreeStoragesViewModel.Load();
		}

		/// <summary>
		///		Crea un nuevo espacio de trabajo
		/// </summary>
		private void NewWorkspace()
		{
			string workspace = string.Empty;
			
				if (MainViewModel.MainController.HostController.SystemController.ShowInputString("Nombre del espacio de trabajo", ref workspace) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					if (!string.IsNullOrWhiteSpace(workspace))
					{
						// Cambia el Workspace
						UpdateWorkspace(workspace);
						// Graba para que se cree el archivo
						MainViewModel.SaveSolution();
						// y lanza el evento de modificación
						MainViewModel.RaiseEventWorkSpaceChanged();
					}
				}
		}

		/// <summary>
		///		Modifica el espacio de trabajos
		/// </summary>
		public void UpdateWorkspace(string workspace)
		{
			// Cambia el espacio de trabajo
			Workspace = workspace;
			// Carga el espacio de trabajo
			Load();
		}

		/// <summary>
		///		Borra un espacio de trabajo
		/// </summary>
		private void DeleteWorkspace()
		{
			if (MainViewModel.MainController.HostController.SystemController.ShowQuestion($"¿Desea eliminar el espacio de trabajo '{Workspace}'?"))
			{
				// Borra el archivo
				MainViewModel.Manager.DeleteConfiguration(Workspace);
				// Pasa al workspace predeterminado
				UpdateWorkspace(MainViewModel.Manager.WorkSpace);
				// Lanza el evento de carga del menú
				MainViewModel.RaiseEventWorkSpaceChanged();
			}
		}

		/// <summary>
		///		Abre la ventana de creación de archivos de pruebas
		/// </summary>
		private async Task CreateTestXmlAsync()
		{
			Details.EtlProjects.CreateTestXmlViewModel viewModel = new Details.EtlProjects.CreateTestXmlViewModel(this);

				if (MainViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
					using (BlockLogModel block = MainViewModel.MainController.Logger.Default.CreateBlock(LogModel.LogType.Info, "Comienzo de la creación de proyectos de pruebas"))
					{
						XmlTestProjectGenerator generator = new XmlTestProjectGenerator(MainViewModel.Manager, viewModel.ComboConnections.GetSelectedConnection(),
																						viewModel.DataBase, viewModel.OutputPath);

							try
							{
								if (!await generator.GenerateAsync(block, viewModel.Provider, viewModel.PathVariable, viewModel.DataBaseVariable, viewModel.SufixTestTables,
																   viewModel.FileNameTest, viewModel.FileNameAssert, 
																   System.Threading.CancellationToken.None))
									block.Error($"Error en la generación de los archivos de pruebas. {generator.Errors.Concatenate()}");
								else
								{
									block.Info("Fin de la creación de proyectos de pruebas");
									MainViewModel.MainController.HostController.SystemController.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
																												  "Generación de proyectos XML",
																												  "Ha terminado correctamente la generación del archivo de pruebas",
																												  TimeSpan.FromSeconds(10));
								}
							}
							catch (Exception exception)
							{
								block.Error($"Error en la generación de los archivos de pruebas {exception.Message}");
							}
					}
		}

		/// <summary>
		///		ViewModel de la ventana principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionModel Solution { get; private set; }

		/// <summary>
		///		Espacio de trabajo
		/// </summary>
		public string Workspace
		{
			get { return _workspace; }
			set { CheckProperty(ref _workspace, value); }
		}

		/// <summary>
		///		ViewModel del árbol de conexiones
		/// </summary>
		public Explorers.Connections.TreeConnectionsViewModel TreeConnectionsViewModel
		{
			get { return _treeConnectionsViewModel; }
			set { CheckObject(ref _treeConnectionsViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de carpetas
		/// </summary>
		public Explorers.Files.TreeFilesViewModel TreeFoldersViewModel
		{
			get { return _treeFoldersViewModel; }
			set { CheckObject(ref _treeFoldersViewModel, value); }
		}

		/// <summary>
		///		ViewModel del árbol de storage
		/// </summary>
		public Explorers.Cloud.TreeStorageViewModel TreeStoragesViewModel
		{
			get { return _treeStoragesViewModel; }
			set { CheckObject(ref _treeStoragesViewModel, value); }
		}

		/// <summary>
		///		ViewModel de los datos de conexiones
		/// </summary>
		public Details.Connections.ConnectionExecutionViewModel ConnectionExecutionViewModel
		{
			get { return _connectionsViewModel; }
			set { CheckObject(ref _connectionsViewModel, value); }
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

		/// <summary>
		///		Crea los archivos XML de proyecto de pruebas
		/// </summary>
		public BaseCommand CreateTestXmlCommand { get; }
	}
}
