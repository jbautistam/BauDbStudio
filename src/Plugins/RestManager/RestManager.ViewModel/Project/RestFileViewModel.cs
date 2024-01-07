using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		ViewModel para ver el contenido de un archivo REST
/// </summary>
public class RestFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName = default!;
	private RestParametersListViewModel? _projectHeaders, _projectVariables;
	private RestFileListStepsViewModel _listStepsViewModel = default!;
	private bool _isExecuting;

	public RestFileViewModel(RestManagerViewModel restManagerViewModel, string fileName)
	{ 
		// Asigna los objetos
		MainViewModel = restManagerViewModel;
		StepsViewModel = new RestFileListStepsViewModel(this);
		ProjectHeaders = new RestParametersListViewModel(this, new ParametersCollectionModel());
		ProjectVariables = new RestParametersListViewModel(this, new ParametersCollectionModel());
		// Asigna las propiedades
		FileName = fileName;
		Header = Path.GetFileNameWithoutExtension(fileName);
		// Indica que no ha habido modificaciones
		IsUpdated = false;
		// Asigna los comandos
		ExecuteCommand = new BaseCommand(_ => Execute(), _ => CanExecute())
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
		ProjectHeaders = new RestParametersListViewModel(this, RestProject.Headers);
		ProjectVariables = new RestParametersListViewModel(this, RestProject.Parameters);
		// Carga los pasos
		StepsViewModel = new RestFileListStepsViewModel(this);
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
			UpdateSteps(RestProject, StepsViewModel);
			UpdateParameters(RestProject.Headers, ProjectHeaders);
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
	private void UpdateSteps(RestProjectModel project, RestFileListStepsViewModel stepsViewModel)
	{
		// Limpia los pasos
		project.Steps.Clear();
		// Añade los pasos
		if (stepsViewModel.Items is not null)
			foreach (RestFileStepItemViewModel stepViewModel in stepsViewModel.Items)
				project.Steps.Add(stepViewModel.GetStep());
	}

	/// <summary>
	///		Actualiza los parámetros
	/// </summary>
	private void UpdateParameters(ParametersCollectionModel parameters, RestParametersListViewModel? parametersViewModel)
	{
		// Limpia la colección
		parameters.Clear();
		// Añade los elementos
		if (parametersViewModel is not null)
			foreach (RestParametersListItemViewModel parameterViewModel in parametersViewModel.Items)
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
	///		Indica si se puede activar la visualización
	/// </summary>
	private bool CanExecute() => !string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName) && !IsExecuting;

	/// <summary>
	///		Indica si se puede detener la visualización
	/// </summary>
	private bool CanStop() => IsExecuting;

	/// <summary>
	///		Comienza la ejecución
	/// </summary>
	public void Execute()
	{
		// Indica que se está ejecutando
		IsExecuting = true;
	}

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
		set { CheckProperty(ref _isExecuting, value); }
	}

	/// <summary>
	///		Lista de pasos
	/// </summary>
	public RestFileListStepsViewModel StepsViewModel 
	{ 
		get { return _listStepsViewModel; }
		set { CheckObject(ref _listStepsViewModel, value); }
	}

	/// <summary>
	///		Cabeceras predeterminadas del proyecto
	/// </summary>
	public RestParametersListViewModel? ProjectHeaders 
	{ 
		get { return _projectHeaders; }
		set { CheckObject(ref _projectHeaders, value); }
	}

	/// <summary>
	///		Variables del proyecto
	/// </summary>
	public RestParametersListViewModel? ProjectVariables 
	{ 
		get { return _projectVariables; }
		set { CheckObject(ref _projectVariables, value); }
	}

	/// <summary>
	///		Cabecera de la ventana
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id del documento
	/// </summary>
	public string TabId => $"{GetType().ToString()}_{FileName}";

	/// <summary>
	///		Comando para ejecutar el archivo multimedia
	/// </summary>
	public BaseCommand ExecuteCommand{ get; }

	/// <summary>
	///		Comando para detener la ejecución
	/// </summary>
	public BaseCommand StopCommand { get; }
}