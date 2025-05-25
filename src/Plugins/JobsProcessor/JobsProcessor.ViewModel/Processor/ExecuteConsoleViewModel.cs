using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.JobsProcessor.Application;
using Bau.Libraries.JobsProcessor.Application.EventArguments;
using Bau.Libraries.JobsProcessor.Application.Models;

namespace Bau.Libraries.JobsProcessor.ViewModel.Processor;

/// <summary>
///		ViewModel para ejecutar un archivo de comandos
/// </summary>
public class ExecuteConsoleViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName = default!, _logText = default!, _fileText = default!;
	private bool _isExecuting;
	private CancellationTokenSource _projectCancellationTokenSource = default!;
	private SynchronizationContext? _contextUi = SynchronizationContext.Current;

	public ExecuteConsoleViewModel(JobsProcessorViewModel mainViewModel, string fileName) : base(false)
	{ 
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		// Inicializa los comandos
		ExecuteCommand = new BaseCommand(_ => ExecuteProject(), _ => !IsExecuting)
									.AddListener(this, nameof(IsExecuting));
		CancelCommand = new BaseCommand(_ => CancelProject(), _ => IsExecuting)
									.AddListener(this, nameof(IsExecuting));
		ClearCommand = new BaseCommand(_ => ClearLog(), _ => !IsExecuting)
								.AddListener(this, nameof(IsExecuting));
		OpenFileCommand = new BaseCommand(_ => OpenFile(), _ => !IsExecuting)
								.AddListener(this, nameof(IsExecuting));
		RefreshCommand = new BaseCommand(_ => Refresh(), _ => !IsExecuting)
								.AddListener(this, nameof(IsExecuting));
	}

	/// <summary>
	///		Carga el contenido del archivo
	/// </summary>
	public void LoadFile()
	{
		if (!string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName))
			FileText = File.ReadAllText(FileName);
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	public ProjectModel? Parse()
	{
		ProjectModel? project = null;

			// Carga el archivo
			if (!File.Exists(FileName))
				WriteTextLog($"Can't find the file {FileName}", true);
			else
			{
				// Carga el archivo
				try
				{
					project = new JobsProcessorManager().Load(FileName);
				}
				catch (Exception exception)
				{
					WriteTextLog($"Error when parse file {exception.Message}", true);
				}
			}
			// Devuelve el proyecto
			return project;
	}

	/// <summary>
	///		Carga los datos del proyecto
	/// </summary>
	private async void ExecuteProject()
	{
		ProjectModel? project = Parse();

			if (project is not null)
			{
				JobsProcessorManager manager = new();

					if (Validate(manager, project))
					{
						// Crea el token de cancelación
						_projectCancellationTokenSource = new();
						// Asigna el manejador de eventos
						manager.JobProcessing += (_, args) => WriteTextLog(args);
						// Ejecuta el proceso
						IsExecuting = true;
						await manager.ExecuteAsync(project, _projectCancellationTokenSource.Token);
						IsExecuting = false;
					}
			}
	}

	/// <summary>
	///		Cancela la ejecución de un proyecto
	/// </summary>
	private void CancelProject()
	{
		if (IsExecuting && _projectCancellationTokenSource is not null && _projectCancellationTokenSource.Token.CanBeCanceled)
			_projectCancellationTokenSource.Cancel();
	}

	/// <summary>
	///		Valida los datos del proyecto
	/// </summary>
	private bool Validate(JobsProcessorManager manager, ProjectModel project)
	{
		List<string> errors = manager.Validate(project);
		
			// y lo valida
			if (errors.Count > 0)
				foreach (string error in errors)
					WriteTextLog(error, true);
			// Devuelve el valor que indica si el proyecto es correcto
			return errors.Count == 0;
	}

	/// <summary>
	///		Limpia el log
	/// </summary>
	private void ClearLog()
	{
		if (!IsExecuting)
			LogText = string.Empty;
	}

	/// <summary>
	///		Abre el archivo en una ventana
	/// </summary>
	private void OpenFile()
	{
		if (!IsExecuting && !string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName))
			MainViewModel.MainController.HostPluginsController.OpenFile(FileName);
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Actualiza el contenido del archivo
	/// </summary>
	private void Refresh()
	{
		if (!IsExecuting)
			LoadFile();
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage() => string.Empty;

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Cierra la ventana de detalles
	/// </summary>
	public void Close()
	{
		CancelProject();
	}

	/// <summary>
	///		Escribe el texto en el log
	/// </summary>
	private void WriteTextLog(JobProcessEventArgs item)
	{
		string header = string.Empty, body = string.Empty;
		string message = $"{item.Date:HH:mm:ss.fff}: {item.Message}";

			// Prepara cabecera y cuerpo del mensaje
			switch (item.Status)
			{
				case JobProcessEventArgs.StatusType.StartProject:
						header = "START";
					break;
				case JobProcessEventArgs.StatusType.EndProject:
						header = "END";
					break;
				case JobProcessEventArgs.StatusType.StartContext:
						header = "START CONTEXT";
						if (item.Context is not null)
							body = GetBody(item.Context);
					break;
				case JobProcessEventArgs.StatusType.EndContext:
						header = "END CONTEXT";
					break;
				case JobProcessEventArgs.StatusType.StartCommand:
						header = "START COMMAND";
						if (item.Command is not null)
							body = GetBody(item.Command);
					break;
				case JobProcessEventArgs.StatusType.EndCommand:
						header = "END COMMAND";
					break;
				case JobProcessEventArgs.StatusType.Information:
						header = "INFO";
					break;
				case JobProcessEventArgs.StatusType.Error:
						header = "ERROR";
					break;
			}
			// Añade el progreso
			if (item.Actual is not null && item.Total is not null)
				message += $" ({item.Actual:#,##0} / {item.Total:#,##0})";
			// Añade el cuerpo
			if (!string.IsNullOrWhiteSpace(body))
				message += Environment.NewLine + body;
			// Añade la cabecera
			if (!string.IsNullOrWhiteSpace(header))
				message = $"[{header}] {message}";
			// Devuelve el mensaje completo
			WriteTextLog(message, false);
	}

	/// <summary>
	///		Obtiene el cuerpo de un mensaje de comando
	/// </summary>
	private string GetBody(CommandModel command)
	{
		string body = "Command data:" + Environment.NewLine;

			// Añade los parámetros
			body += $"\t-Executable: {command.FileName}" + Environment.NewLine;
			foreach (ArgumentModel argument in command.Arguments)
				body += $"\t-{argument.Parameter.Name}: {argument.Parameter.Value}" + Environment.NewLine;
			// Devuelve el cuerpo del mensaje
			return body;
	}

	/// <summary>
	///		Obtiene el cuerpo de un mensaje de contexto
	/// </summary>
	private string GetBody(ContextModel context)
	{
		string body = "Context data:" + Environment.NewLine;

			// Añade los parámetros
			foreach (ParameterModel parameter in context.Parameters)
				body += $"\t- {parameter.Name}: {parameter.Value}" + Environment.NewLine;
			// Devuelve el cuerpo del mensaje
			return body;
	}

	/// <summary>
	///		Escribe el texto del log
	/// </summary>
	internal void WriteTextLog(string text, bool isError)
	{
		object state = new object();

			//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
			//? Al generarse el log en un evento interno, no se puede añadir a ObservableCollection sin una
			//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
			//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
			// Añade el mensaje
			if (_contextUi is not null)
				_contextUi.Send(_ => {
										if (isError)
											LogText += "ERR: " + text + Environment.NewLine;
										else
											LogText += text + Environment.NewLine;
									 }, state
							   );
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public JobsProcessorViewModel MainViewModel { get; set; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; } = default!;

	/// <summary>
	///		Id de la ficha en pantalla
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
			{
				if (string.IsNullOrWhiteSpace(_fileName))
					Header = "New filename";
				else
					Header = Path.GetFileName(_fileName);
			}
		}
	}

	/// <summary>
	///		Texto del archivo
	/// </summary>
	public string FileText
	{
		get { return _fileText; }
		set { CheckProperty(ref _fileText, value); }
	}

	/// <summary>
	///		Texto de log
	/// </summary>
	public string LogText
	{
		get { return _logText;}
		set { CheckProperty(ref _logText, value); }
	}

	/// <summary>
	///		Indica si se está ejecutando algún proceso
	/// </summary>
	public bool IsExecuting
	{
		get { return _isExecuting; }
		set { CheckProperty(ref _isExecuting, value); }
	}

	/// <summary>
	///		Comando para ejecutar los comandos de un archivo
	///	</summary>
	public BaseCommand ExecuteCommand { get; }

	/// <summary>
	///		Comando para cancelar la ejecución de un archivo
	/// </summary>
	public BaseCommand CancelCommand { get; }

	/// <summary>
	///		Comando para limpiar el log
	/// </summary>
	public BaseCommand ClearCommand { get; }

	/// <summary>
	///		Comando para abrir el archivo
	/// </summary>
	public BaseCommand OpenFileCommand { get; }

	/// <summary>
	///		Comando para recargar el archivo
	/// </summary>
	public BaseCommand RefreshCommand { get; }
}
