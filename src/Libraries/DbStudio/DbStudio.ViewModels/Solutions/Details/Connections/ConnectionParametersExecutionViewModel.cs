using System;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel para la obtención de los parámetros de ejecución de consultas y proyectos ETL
	/// </summary>
	public class ConnectionParametersExecutionViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _connectionParametersFileName, _etlParametersFileName;

		public ConnectionParametersExecutionViewModel(ConnectionExecutionViewModel connectionExecutionViewModel)
		{
			// Guarda las propiedades
			ConnectionExecutionViewModel = connectionExecutionViewModel;
			// Carga el nombre del archivo de parámetros de conexión de la solución
			if (System.IO.File.Exists(ConnectionExecutionViewModel.SolutionViewModel.Solution.LastConnectionParametersFileName))
				ConnectionParametersFileName = ConnectionExecutionViewModel.SolutionViewModel.Solution.LastConnectionParametersFileName;
			else
				ConnectionParametersFileName = string.Empty;
			// Carga el nombre del archivo de parámetros de conexión de los proyectos ETl de la solución
			if (System.IO.File.Exists(ConnectionExecutionViewModel.SolutionViewModel.Solution.LastEtlParametersFileName))
				EtlParametersFileName = ConnectionExecutionViewModel.SolutionViewModel.Solution.LastEtlParametersFileName;
			else
				EtlParametersFileName = string.Empty;
		}

		/// <summary>
		///		Comprueba los datos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (!string.IsNullOrWhiteSpace(ConnectionParametersFileName) && !System.IO.File.Exists(ConnectionParametersFileName))
					ConnectionExecutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController
							.ShowMessage("Seleccione el nombre del archivo de parámetros de conexión");
				else if (!string.IsNullOrWhiteSpace(EtlParametersFileName) && !System.IO.File.Exists(EtlParametersFileName))
					ConnectionExecutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController
							.ShowMessage("Seleccione el nombre del archivo de parámetros de los proyectos de ETL");
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
				RaiseEventClose(true);
		}

		/// <summary>
		///		ViewModel padre
		/// </summary>
		public ConnectionExecutionViewModel ConnectionExecutionViewModel { get; }

		/// <summary>
		///		Nombre del archivo con los parámetros de ejecución sobre una conexión
		/// </summary>
		public string ConnectionParametersFileName
		{ 
			get { return _connectionParametersFileName; }
			set { CheckProperty(ref _connectionParametersFileName, value); }
		}

		/// <summary>
		///		Nombre del archivo con los parámetros de ejecución para los proyectos de ETL
		/// </summary>
		public string EtlParametersFileName
		{ 
			get { return _etlParametersFileName; }
			set { CheckProperty(ref _etlParametersFileName, value); }
		}
	}
}
