using System;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects
{
	/// <summary>
	///		ViewModel de ejecución de la consola de proyectos de ETL
	/// </summary>
	public class ExecuteEtlConsoleViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
	{
		// Eventos públicos
		public event EventHandler LoadFileRequired;
		// Variables privadas
		private string _header, _etlConsoleFileName, _projectFileName, _contextFileName;

		public ExecuteEtlConsoleViewModel(SolutionViewModel solutionViewModel, string projectFileName) : base(false)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			// Inicializa el viewModel
			InitViewModel(projectFileName);
			// Inicializa los comandos
			ExecuteScriptCommand = new BaseCommand(async _ => await PrepareExecuteScriptAsync());
			ExecuteConsoleCommand = new BaseCommand(_ => ExecuteConsole(), _ => !string.IsNullOrWhiteSpace(EtlConsoleFileName))
											.AddListener(this, nameof(EtlConsoleFileName));
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel(string projectFileName)
		{
			// Asigna las propiedades
			Header = "Ejecución script";
			EtlConsoleFileName = SolutionViewModel.MainController.GetEtlConsoleFileName();
			ProjectFileName = projectFileName;
			ContextFileName = SolutionViewModel.ConnectionExecutionViewModel.EtlParametersFileName;
			// Lanza el evento de carga de archivos
			RaiseEventLoadFile();
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
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca un nombre de archivo válido para la consola de ejecución");
				else if (string.IsNullOrWhiteSpace(ProjectFileName) || !System.IO.File.Exists(ProjectFileName))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca un nombre de archivo válido para el proyecto");
				else if (string.IsNullOrWhiteSpace(ContextFileName) || !System.IO.File.Exists(ContextFileName))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca un nombre de archivo válido para el contexto");
				else 
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Graba los datos (en este caso simplemente implementa la interface)
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// Vacío, sólo implementa la interface
		}

		/// <summary>
		///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return "¿Desea grabar la consulta antes de continuar?";
		}

		/// <summary>
		///		Ejecuta el script sobre una consola
		/// </summary>
		private void ExecuteConsole()
		{
			if (ValidateData())
			{
				LibSystem.Processes.SystemProcessHelper processor = new LibSystem.Processes.SystemProcessHelper();

					// Guarda el archivo de contexto
					SolutionViewModel.ConnectionExecutionViewModel.EtlParametersFileName = ContextFileName;
					SolutionViewModel.SaveSolution();
					// Ejecuta la consola
					processor.ExecuteApplication(EtlConsoleFileName,
												 $"--project \"{ProjectFileName}\" --context \"{ContextFileName}\"",
												 false, true);
					// Indica que ya no es nuevo y está grabado
					IsUpdated = false;
			}
		}

		/// <summary>
		///		Prepara la ejecución del script
		/// </summary>
		private async Task PrepareExecuteScriptAsync()
		{
			await SolutionViewModel.ConnectionExecutionViewModel.ExecuteScriptAsync();
		}

		/// <summary>
		///		Ejecuta el script XML
		/// </summary>
		internal async Task ExecuteXmlScriptAsync(System.Threading.CancellationToken cancellationToken)
		{
			Files.ScriptsManager.JobXmlProjectManager manager = new Files.ScriptsManager.JobXmlProjectManager(SolutionViewModel.Manager.Logger);

				// Guarda el archivo de contexto
				SolutionViewModel.ConnectionExecutionViewModel.EtlParametersFileName = ContextFileName;
				// Ejecuta el script XML
				await manager.ExecuteAsync(ProjectFileName, ContextFileName, cancellationToken);
				// Libera el log
				SolutionViewModel.Manager.Logger.Flush();
		}

		/// <summary>
		///		Lanza el evento de carga de archivos
		/// </summary>
		private void RaiseEventLoadFile()
		{
			LoadFileRequired?.Invoke(this, EventArgs.Empty);
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
			set 
			{ 
				if (CheckProperty(ref _projectFileName, value))
					RaiseEventLoadFile();
			}
		}

		/// <summary>
		///		Nombre del archivo de contexto
		/// </summary>
		public string ContextFileName
		{
			get { return _contextFileName; }
			set 
			{ 
				if (CheckProperty(ref _contextFileName, value))
					RaiseEventLoadFile();
			}
		}

		/// <summary>
		///		Cabecera de la pestaña
		/// </summary>
		public string Header 
		{ 
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id del documento en la ventana principal (sólo puede haber uno de este tipo)
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString(); }
		}

		/// <summary>
		///		Ejecuta el script en la propia aplicación
		/// </summary>
		public BaseCommand ExecuteScriptCommand { get; }
		
		/// <summary>
		///		Ejecuta el script sobre una consola
		/// </summary>
		public BaseCommand ExecuteConsoleCommand { get; }
	}
}