﻿using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.RestManager.Application.Models;
using Bau.Libraries.RestManager.ViewModel.Project.Connections;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		ViewModel para ver el contenido de un archivo REST
/// </summary>
public class RestFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName = default!;
	private Parameters.RestParametersListViewModel? _projectVariables;
	private Steps.RestFileListStepsViewModel _listStepsViewModel = default!;
	private ConnectionsListViewModel _connectionsListViewModel = default!;
	private bool _isExecuting;

	public RestFileViewModel(RestManagerViewModel restManagerViewModel, string fileName)
	{ 
		// Asigna los objetos
		MainViewModel = restManagerViewModel;
		StepsViewModel = new Steps.RestFileListStepsViewModel(this);
		ProjectVariables = new Parameters.RestParametersListViewModel(this);
		ConnectionsListViewModel = new ConnectionsListViewModel(this);
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
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
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
	public Steps.RestFileListStepsViewModel StepsViewModel 
	{ 
		get { return _listStepsViewModel; }
		set { CheckObject(ref _listStepsViewModel, value); }
	}

	/// <summary>
	///		Lista de conexiones
	/// </summary>
	public ConnectionsListViewModel ConnectionsListViewModel
	{
		get { return _connectionsListViewModel; }
		set { CheckObject(ref _connectionsListViewModel, value); }
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