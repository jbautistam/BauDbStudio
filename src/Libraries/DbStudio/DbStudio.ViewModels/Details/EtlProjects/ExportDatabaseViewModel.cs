using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects
{
	/// <summary>
	///		ViewModel para la exportación de archivos de base de datos
	/// </summary>
	public class ExportDatabaseViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private Connections.ComboConnectionsViewModel _comboConnections;
		private ComboViewModel _comboFormat;
		private string _dataBase, _outputPath;
		private Application.SolutionManager.FormatType _formatType;
		private long _blockSize;

		public ExportDatabaseViewModel(SolutionViewModel solutionViewModel)
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
			// Combo de formato de los archivos
			ComboFormat = new ComboViewModel(this);
			ComboFormat.AddItem((int) Application.SolutionManager.FormatType.Parquet, "Parquet");
			ComboFormat.AddItem((int) Application.SolutionManager.FormatType.Csv, "CSV");
			ComboFormat.SelectedItem = ComboFormat.Items[0];
			// Asigna las propiedades
			DataBase = string.Empty;
			OutputPath = SolutionViewModel.MainController.DialogsController.LastPathSelected;
			BlockSize = 2_000_000;
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
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la conexión");
				else if (string.IsNullOrWhiteSpace(OutputPath))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el directorio de salida de los archivos");
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
				// Guarda las propiedades
				FormatType = (Application.SolutionManager.FormatType) (ComboFormat.SelectedId ?? (int) Application.SolutionManager.FormatType.Parquet);
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
		///		Base de datos
		/// </summary>
		public string DataBase
		{
			get { return _dataBase; }
			set { CheckProperty(ref _dataBase, value); }
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
		///		Combo de conexiones
		/// </summary>
		public Connections.ComboConnectionsViewModel ComboConnections
		{
			get { return _comboConnections; }
			set { CheckObject(ref _comboConnections, value); }
		}

		/// <summary>
		///		Tipos de exportación
		/// </summary>
		public ComboViewModel ComboFormat
		{
			get { return _comboFormat; }
			set { CheckObject(ref _comboFormat, value); }
		}

		/// <summary>
		///		Formato de los archivos de salida
		/// </summary>
		public Application.SolutionManager.FormatType FormatType
		{
			get { return _formatType; }
			set { CheckProperty(ref _formatType, value); }
		}

		/// <summary>
		///		Tamaño del bloque de escritura
		/// </summary>
		public long BlockSize
		{
			get { return _blockSize; }
			set { CheckProperty(ref _blockSize, value); }
		}
	}
}