using System;

using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects
{
	/// <summary>
	///		ViewModel de creación de archivos XML de pruebas
	/// </summary>
	public class CreateTestXmlViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _provider, _dataBase;
		private string _pathVariable, _dataBaseVariable, _sufixTestTables;
		private string _outputPath, _fileNameProject, _fileNameTest, _fileNameAssert;
		private Connections.ComboConnectionsViewModel _comboConnections;

		public CreateTestXmlViewModel(SolutionViewModel solutionViewModel)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			ComboConnections = new Connections.ComboConnectionsViewModel(SolutionViewModel, string.Empty);
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			Provider = "SparkDbCompute";
			DataBase = string.Empty;
			PathVariable = "MountPath";
			DataBaseVariable = "DbCompute";
			SufixTestTables = "Test";
			OutputPath = SolutionViewModel.MainViewModel.LastPathSelected;
			FileNameProject = "00. Test database.xml";
			FileNameTest = "05. Create test tables.xml";
			FileNameAssert = "10. Compare with test tables.xml";
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (ComboConnections.GetSelectedConnection() == null)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione la conexión");
				else if (string.IsNullOrWhiteSpace(Provider))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del proveedor en el archivo");
				else if (string.IsNullOrWhiteSpace(DataBase))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la base de datos");
				else if (string.IsNullOrWhiteSpace(PathVariable))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la variable con el directorio de salida");
				else if (string.IsNullOrWhiteSpace(DataBaseVariable))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la variable de base de datos");
				else if (string.IsNullOrWhiteSpace(OutputPath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el directorio de grabación de los proyectos");
				else if (string.IsNullOrWhiteSpace(FileNameProject))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del archivo XML de proyecto");
				else if (string.IsNullOrWhiteSpace(FileNameTest))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del archivo XML de pruebas");
				else if (string.IsNullOrWhiteSpace(FileNameAssert))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del archivo XML de comparación");
				else 
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Guarda el directorio seleccionado
				SolutionViewModel.MainViewModel.LastPathSelected = OutputPath;
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Datos de conexión
		/// </summary>
		public ConnectionModel Connection { get; }

		/// <summary>
		///		Proveedor
		/// </summary>
		public string Provider
		{
			get { return _provider; }
			set { CheckProperty(ref _provider, value); }
		}

		/// <summary>
		///		Base de datos
		/// </summary>
		public string DataBase
		{
			get { return _dataBase; }
			set { CheckProperty(ref _dataBase, value); }
		}

		/// <summary>
		///		Variable con el directorio de montaje
		/// </summary>
		public string PathVariable
		{
			get { return _pathVariable; }
			set { CheckProperty(ref _pathVariable, value); }
		}

		/// <summary>
		///		Variable con el nombre de base de datos
		/// </summary>
		public string DataBaseVariable
		{
			get { return _dataBaseVariable; }
			set { CheckProperty(ref _dataBaseVariable, value); }
		}

		/// <summary>
		///		Sufijo de las tablas de pruebas
		/// </summary>
		public string SufixTestTables
		{
			get { return _sufixTestTables; }
			set { CheckProperty(ref _sufixTestTables, value); }
		}

		/// <summary>
		///		Directorio de salida
		/// </summary>
		public string OutputPath
		{
			get { return _outputPath; }
			set { CheckProperty(ref _outputPath, value); }
		}

		/// <summary>
		///		Nombre del archivo de proyecto
		/// </summary>
		public string FileNameProject
		{
			get { return _fileNameProject; }
			set { CheckProperty(ref _fileNameProject, value); }
		}

		/// <summary>
		///		Nombre del archivo de pruebas
		/// </summary>
		public string FileNameTest
		{
			get { return _fileNameTest; }
			set { CheckProperty(ref _fileNameTest, value); }
		}

		/// <summary>
		///		Nombre del archivo de comparación
		/// </summary>
		public string FileNameAssert
		{
			get { return _fileNameAssert; }
			set { CheckProperty(ref _fileNameAssert, value); }
		}

		/// <summary>
		///		Combo de conexiones
		/// </summary>
		public Connections.ComboConnectionsViewModel ComboConnections
		{
			get { return _comboConnections; }
			set { CheckObject(ref _comboConnections, value); }
		}
	}
}