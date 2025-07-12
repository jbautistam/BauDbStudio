using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Steps;

/// <summary>
///		ViewModel para un paso de un proyecto
/// </summary>
public class RestFileStepItemViewModel : BaseObservableObject
{
	// Variables privadas
	private string _name = default!, _url = default!, _content = default!;
	private int _timeout;
	private ComboViewModel? _comboConnections, _comboMethods;
	private Parameters.RestParametersListViewModel? _headers, _queryStrings;

	public RestFileStepItemViewModel(RestFileListStepsViewModel listStepsViewModel, RestStepModel restStep)
	{
		// Asigna las propiedades
		ListStepsViewModel = listStepsViewModel;
		RestStep = restStep;
		// Inicializa los datos
		InitViewModel();
	}

	/// <summary>
	///		Inicializa los datos
	/// </summary>
	private void InitViewModel()
	{
		// Carga los combos
		ComboMethods = LoadComboMethods();
		ComboConnections = LoadComboConnections();
		// Asigna las propiedades
		ComboMethods.SelectedId = (int) RestStep.Method;
		ComboConnections.SelectedTag = GetConnection(RestStep.ConnectionId);
		Name = RestStep.Name;
		Url = RestStep.Url;
		Timeout = (int) RestStep.Timeout.TotalSeconds;
		Content = RestStep.Content ?? string.Empty;
		// Cabeceras
		Headers = new Parameters.RestParametersListViewModel(ListStepsViewModel.FileViewModel);
		Headers.AddParameters(RestStep.Headers);
		// Query strings
		QueryStrings = new Parameters.RestParametersListViewModel(ListStepsViewModel.FileViewModel);
		QueryStrings.AddParameters(RestStep.QueryStrings);
		// Asigna el manejador de eventos
		PropertyChanged += (sender, args) => ListStepsViewModel.FileViewModel.IsUpdated = true;
		ComboMethods.PropertyChanged += (sender, args) => ListStepsViewModel.FileViewModel.IsUpdated = true;
		ComboConnections.PropertyChanged += (sender, args) => ListStepsViewModel.FileViewModel.IsUpdated = true;
	}

	/// <summary>
	///		Obtiene la conexión asociada a un Id
	/// </summary>
	private ConnectionModel? GetConnection(string connectionId)
	{
		// Obtiene la conexión
		foreach (ConnectionModel connection in ListStepsViewModel.FileViewModel.ConnectionsListViewModel.GetConnections())
			if (connection.Id.Equals(connectionId, StringComparison.CurrentCultureIgnoreCase))
				return connection;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Carga el combo de métodos
	/// </summary>
	private ComboViewModel LoadComboMethods()
	{
		ComboViewModel cboMethods = new(this);

			// Añade los elementos
			cboMethods.AddItem((int) RestStepModel.RestMethod.Get, "Get");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Post, "Post");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Patch, "Patch");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Delete, "Delete");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Put, "Put");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Options, "Options");
			// Selecciona el primer elemento
			cboMethods.SelectedItem = cboMethods.Items[0];
			// Devuelve el combo
			return cboMethods;
	}

	/// <summary>
	///		Carga el combo de conexiones
	/// </summary>
	private ComboViewModel LoadComboConnections()
	{
		ComboViewModel cboConnections = new(this);

			// Añade un elemento vacío
			cboConnections.AddItem(null, "<Select a connection>", null);
			// Añade las conexiones
			foreach (ConnectionModel connection in ListStepsViewModel.FileViewModel.ConnectionsListViewModel.GetConnections())
				cboConnections.AddItem(cboConnections.Items.Count, connection.Name, connection);
			// Selecciona el primer elemento
			cboConnections.SelectedItem = cboConnections.Items[0];
			// Devuelve el combo
			return cboConnections;
	}

	/// <summary>
	///		Obtiene los datos del paso
	/// </summary>
	internal BaseStepModel GetStep()
	{
		RestStepModel step = new();

			// Asigna los datos
			if (ComboConnections?.SelectedTag is ConnectionModel connection)
				step.ConnectionId = connection.Id;
			if (ComboMethods is not null)
				step.Method = (RestStepModel.RestMethod) (ComboMethods.SelectedId ?? 0);
			step.Name = Name;
			step.Url = Url;
			step.Timeout = TimeSpan.FromSeconds(Timeout);
			step.Content = Content;
			// Añade las cabeceras
			step.Headers.Clear();
			if (Headers is not null)
				foreach (Parameters.RestParametersListItemViewModel header in Headers.Items)
					step.Headers.Add(new ParameterModel(header.Key, header.Value));
			// Añade las query strings
			step.QueryStrings.Clear();
			if (QueryStrings is not null)
				foreach (Parameters.RestParametersListItemViewModel queryString in QueryStrings.Items)
					step.QueryStrings.Add(new ParameterModel(queryString.Key, queryString.Value));
			// Devuelve el paso
			return step;
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public RestFileListStepsViewModel ListStepsViewModel { get; }

	/// <summary>
	///		Datos del paso
	/// </summary>
	public RestStepModel RestStep { get; }

	/// <summary>
	///		Combo con las conexiones
	/// </summary>
	public ComboViewModel? ComboConnections
	{
		get { return _comboConnections; }
		set { CheckObject(ref _comboConnections, value); }
	}

	/// <summary>
	///		Combo con los métodos
	/// </summary>
	public ComboViewModel? ComboMethods
	{
		get { return _comboMethods; }
		set { CheckObject(ref _comboMethods, value); }
	}

	/// <summary>
	///		Nombre
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Url
	/// </summary>
	public string Url 
	{ 
		get { return _url; }
		set { CheckProperty(ref _url, value); }
	}

	/// <summary>
	///		Tiempo de espera
	/// </summary>
	public int Timeout
	{
		get { return _timeout; }
		set { CheckProperty(ref _timeout, value); }
	}

	/// <summary>
	///		Contenido
	/// </summary>
	public string Content
	{
		get { return _content; }
		set { CheckProperty(ref _content, value); }
	}

	/// <summary>
	///		Lista de cabeceras
	/// </summary>
	public Parameters.RestParametersListViewModel? Headers
	{
		get { return _headers; }
		set { CheckObject(ref _headers, value); }
	}

	/// <summary>
	///		Lista de query strings
	/// </summary>
	public Parameters.RestParametersListViewModel? QueryStrings
	{
		get { return _queryStrings; }
		set { CheckObject(ref _queryStrings, value); }
	}
}
