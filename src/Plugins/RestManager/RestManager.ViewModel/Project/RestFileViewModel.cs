using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;
using Bau.Libraries.RestManager.Application.Models;
using Bau.Libraries.RestManager.ViewModel.Project.Connections;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		ViewModel para ver el contenido de un archivo REST
/// </summary>
public class RestFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName = default!, _log = default!;
	private Parameters.RestParametersListViewModel? _projectVariables;
	private Steps.RestFileListStepsViewModel _listStepsViewModel = default!;
	private ConnectionsListViewModel _connectionsListViewModel = default!;
	private bool _isExecuting;
	private SynchronizationContext? _contextUi = SynchronizationContext.Current;

	public RestFileViewModel(RestManagerViewModel restManagerViewModel, string fileName)
	{ 
		// Asigna los objetos
		MainViewModel = restManagerViewModel;
		StepsViewModel = new Steps.RestFileListStepsViewModel(this);
		ProjectVariables = new Parameters.RestParametersListViewModel(this);
		ConnectionsListViewModel = new ConnectionsListViewModel(this);
		// Asigna las propiedades
		FileName = fileName;
		Header = Path.GetFileName(fileName);
		// Indica que no ha habido modificaciones
		IsUpdated = false;
		// Asigna los comandos
		ExecuteCommand = new BaseCommand(async _ => await ExecuteAsync(true, CancellationToken.None), _ => CanExecute(true))
								.AddListener(this, nameof(IsExecuting));
		ExecuteStepCommand = new BaseCommand(async _ => await ExecuteAsync(false, CancellationToken.None), _ => CanExecute(false))
								.AddListener(this, nameof(IsExecuting));
		StopCommand = new BaseCommand(_ => Stop(), _ => CanStop())
								.AddListener(this, nameof(IsExecuting));
	}

	/// <summary>
	///		Carga el proyecto
	/// </summary>
	public void Load()
	{
		// Carga el proyecto
		RestProject = new Application.RestProjectManager().Load(FileName);
		// Asigna los datos del proyecto
		ProjectVariables = new Parameters.RestParametersListViewModel(this);
		ProjectVariables.AddParameters(RestProject.Parameters);
		// Carga las conexiones
		ConnectionsListViewModel = new ConnectionsListViewModel(this);
		ConnectionsListViewModel.AddConnections(RestProject.Connections);
		// Carga los pasos
		StepsViewModel = new Steps.RestFileListStepsViewModel(this);
		foreach (RestStepModel step in RestProject.Steps)
			StepsViewModel.Add(step);
		// Selecciona el primer paso
		if (StepsViewModel.Items is not null && StepsViewModel.Items.Count > 0)
			StepsViewModel.SelectedItem = StepsViewModel.Items[0];
		// Indica que por ahora no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "Do you want to save the file before continuing?";
		else
			return $"Do you want to save the file '{Path.GetFileName(FileName)}' before continuing?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// Obtiene el nombre de archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string? newFileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave(string.Empty, "Rest file (*.rest)|*.rest|All files (*.*)|*.*",
																								 FileName, ".rest");

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Guarda los datos
			RestProject.Connections.Clear();
			RestProject.Connections.AddRange(ConnectionsListViewModel.GetConnections());
			UpdateSteps(RestProject, StepsViewModel);
			UpdateParameters(RestProject.Parameters, ProjectVariables);
			// Graba el archivo
			new Application.RestProjectManager().Save(RestProject, FileName);
			// Añade el archivo a los últimos archivos abiertos
			MainViewModel.ViewsController.HostPluginsController.AddFileUsed(FileName);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Añade los pasos al proyecto
	/// </summary>
	private void UpdateSteps(RestProjectModel project, Steps.RestFileListStepsViewModel stepsViewModel)
	{
		// Limpia los pasos
		project.Steps.Clear();
		// Añade los pasos
		if (stepsViewModel.Items is not null)
			foreach (Steps.RestFileStepItemViewModel stepViewModel in stepsViewModel.Items)
				project.Steps.Add(stepViewModel.GetStep());
	}

	/// <summary>
	///		Actualiza los parámetros
	/// </summary>
	private void UpdateParameters(ParametersCollectionModel parameters, Parameters.RestParametersListViewModel? parametersViewModel)
	{
		// Limpia la colección
		parameters.Clear();
		// Añade los elementos
		if (parametersViewModel is not null)
			foreach (Parameters.RestParametersListItemViewModel parameterViewModel in parametersViewModel.Items)
				parameters.Add(new ParameterModel(parameterViewModel.Key, parameterViewModel.Value));
	}

	/// <summary>
	///		Cierra el documento
	/// </summary>
	public void Close() 
	{
		Stop();
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine("Execute by external command");
	}

	/// <summary>
	///		Comienza la ejecución
	/// </summary>
	public async Task ExecuteAsync(bool allSteps, CancellationToken cancellationToken)
	{
		if (IsExecuting)
			MainViewModel.ViewsController.SystemController.ShowMessage("Cannot be executed. There is another process running");
		else
		{
			Controllers.Processors.RestProcessor processor = new(this, allSteps ? null : StepsViewModel.SelectedItem);

				// Log
				WriteLog($"# {DateTime.Now:HH:mm:ss} Start execution");
				// Asigna el tratamiento de eventos
				processor.Log += (sender, args) => WriteLog(args);
				// Indica que se está ejecutando
				IsExecuting = true;
				// Encola el proceso
				await MainViewModel.ViewsController.MainWindowController.EnqueueProcessAsync(processor, cancellationToken);
				// Indica que ha finalizado la ejecución
				IsExecuting = false;
		}
	}

	/// <summary>
	///		Indica si se puede activar la visualización
	/// </summary>
	private bool CanExecute(bool allSteps) 
	{
		bool canExecute = !string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName) && !IsExecuting && StepsViewModel.Items?.Count != 0;

			// Si no se tienen que ejecutar todos los pasos, comprueba que se haya seleccionado algún elemento
			if (!allSteps)
				canExecute = StepsViewModel.SelectedItem is not null;
			// Devuelve el valor que indica si se debe ejecutar
			return canExecute;
	}

	/// <summary>
	///		Añade un evento al log
	/// </summary>
	private void WriteLog(LogEventArgs args)
	{
		WriteLog($"**[{DateTime.Now:HH:mm:ss}] {args.State.ToString().ToUpper()} :** {args.Message}");
	}

	/// <summary>
	///		Añade un texto al log
	/// </summary>
	private void WriteLog(string log)
	{
		object state = new();

			//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
			//? Al generarse el log en un evento interno, no se puede añadir el texto si no está en el mismo contexto
			// Añade el mensaje
			if (_contextUi is not null)
				_contextUi.Send(_ => Log += log + Environment.NewLine, state);
	}

	/// <summary>
	///		Indica si se puede detener la visualización
	/// </summary>
	private bool CanStop() => IsExecuting;

	/// <summary>
	///		Detiene la ejecución del archivo multimedia
	/// </summary>
	public void Stop()
	{
		// Indica que ya no se está ejecutando
		IsExecuting = false;
	}

	/// <summary>
	///		ViewModel de la lista de archivos
	/// </summary>
	public RestManagerViewModel MainViewModel { get; set; }

	/// <summary>
	///		Proyecto Rest
	/// </summary>
	public RestProjectModel RestProject { get; private set; } = new();

	/// <summary>
	///		Cabecera de la ventana
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id del documento
	/// </summary>
	public string TabId => $"{GetType().ToString()}_{FileName}";

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
				Header = Path.GetFileNameWithoutExtension(_fileName);
		}
	}

	/// <summary>
	///		Indica si se está ejecutando
	/// </summary>
	public bool IsExecuting
	{
		get { return _isExecuting; }
		set { CheckProperty(ref _isExecuting, value, false); }
	}

	/// <summary>
	///		Lista de pasos
	/// </summary>
	public Steps.RestFileListStepsViewModel StepsViewModel 
	{ 
		get { return _listStepsViewModel; }
		set { CheckObject(ref _listStepsViewModel!, value); }
	}

	/// <summary>
	///		Lista de conexiones
	/// </summary>
	public ConnectionsListViewModel ConnectionsListViewModel
	{
		get { return _connectionsListViewModel; }
		set { CheckObject(ref _connectionsListViewModel!, value); }
	}

	/// <summary>
	///		Variables del proyecto
	/// </summary>
	public Parameters.RestParametersListViewModel? ProjectVariables 
	{ 
		get { return _projectVariables; }
		set { CheckObject(ref _projectVariables, value); }
	}

	/// <summary>
	///		Texto de log
	/// </summary>
	public string Log
	{
		get { return _log; }
		set { CheckProperty(ref _log, value, false); }
	}

	/// <summary>
	///		Comando para ejecutar todos los pasos del archivo
	/// </summary>
	public BaseCommand ExecuteCommand{ get; }

	/// <summary>
	///		Comando para ejecutar el paso seleccionado
	/// </summary>
	public BaseCommand ExecuteStepCommand { get; }

	/// <summary>
	///		Comando para detener la ejecución
	/// </summary>
	public BaseCommand StopCommand { get; }
}