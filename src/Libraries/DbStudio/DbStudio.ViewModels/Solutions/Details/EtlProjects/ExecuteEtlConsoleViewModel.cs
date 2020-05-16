using System;

using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects
{
	/// <summary>
	///		ViewModel de ejecución de la consola de proyectos de ETL
	/// </summary>
	public class ExecuteEtlConsoleViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _etlConsoleFileName, _projectFileName, _contextFileName;

		public ExecuteEtlConsoleViewModel(SolutionViewModel solutionViewModel, string projectFileName)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			// Inicializa el viewModel
			InitViewModel(projectFileName);
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel(string projectFileName)
		{
			// Asigna las propiedades
			EtlConsoleFileName = SolutionViewModel.MainViewModel.MainController.GetEtlConsoleFileName();
			ProjectFileName = projectFileName;
			ContextFileName = SolutionViewModel.ConnectionExecutionViewModel.EtlParametersFileName;
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
				if (string.IsNullOrWhiteSpace(EtlConsoleFileName) || !System.IO.File.Exists(EtlConsoleFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca un nombre de archivo válido para la consola de ejecución");
				else if (string.IsNullOrWhiteSpace(ProjectFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca un nombre de archivo válido para el proyecto");
				else if (string.IsNullOrWhiteSpace(ContextFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca un nombre de archivo válido para el contexto");
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
				LibSystem.Processes.SystemProcessHelper processor = new LibSystem.Processes.SystemProcessHelper();

					// Ejecuta la consola
					processor.ExecuteApplication(EtlConsoleFileName,
												 $"--project \"{ProjectFileName}\" --context \"{ContextFileName}\"",
												 false, true);
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
		///		Nombre del ejecutable
		/// </summary>
		public string EtlConsoleFileName
		{
			get { return _etlConsoleFileName; }
			set { CheckProperty(ref _etlConsoleFileName, value); }
		}

		/// <summary>
		///		Nombre del archivo de proyecto
		/// </summary>
		public string ProjectFileName
		{
			get { return _projectFileName; }
			set { CheckProperty(ref _projectFileName, value); }
		}

		/// <summary>
		///		Nombre del archivo de contexto
		/// </summary>
		public string ContextFileName
		{
			get { return _contextFileName; }
			set { CheckProperty(ref _contextFileName, value); }
		}
	}
}