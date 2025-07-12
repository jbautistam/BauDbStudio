using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Connections;

/// <summary>
///		ViewModel para una conexión de un proyecto
/// </summary>
public class ConnectionViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = default!;
	private string? _description, _url;
	private int _timeout;
	private Parameters.RestParametersListViewModel? _headers;
	private ConnectionAuthenticationViewModel _authenticationViewModel = default!;
	private bool _isNew;

	public ConnectionViewModel(ConnectionsListViewModel listConnectionsViewModel, ConnectionModel? connection)
	{
		// Asigna las propiedades
		_isNew = connection is null;
		ListConnectionsViewModel = listConnectionsViewModel;
		Connection = connection ?? new ConnectionModel();
		// Inicializa los datos
		InitViewModel();
	}

	/// <summary>
	///		Inicializa los datos
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		Name = Connection.Name;
		Description = Connection.Description;
		Url = Connection.Url?.ToString();
		Timeout = (int) Connection.Timeout.TotalSeconds;
		// Asigna las cabeceras
		Headers = new Parameters.RestParametersListViewModel(ListConnectionsViewModel.FileViewModel);
		if (_isNew)
			Headers.AddParameters(Connection.GetDefaultHeaders("BauDbStudio-Rest-Agent"));
		else
			Headers.AddParameters(Connection.Headers);
		// Asigna el viewModel de autenticación
		AuthenticationViewModel = new ConnectionAuthenticationViewModel(this);
		// Asigna el manejador de eventos
		PropertyChanged += (sender, args) => ListConnectionsViewModel.FileViewModel.IsUpdated = true;
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Enter the connection name");
			else if (Url.GetUrl() is null)
				ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Enter a valid url");
			else
				validated = AuthenticationViewModel.ValidateData();
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
			// Asigna los datos al objeto
			Connection.Name = Name;
			Connection.Description = Description;
			Connection.Url = Url?.GetUrl();
			Connection.Timeout = TimeSpan.FromSeconds(Timeout);
			// Añade las cabeceras
			Connection.Headers.Clear();
			if (Headers is not null)
				Connection.Headers.AddRange(Headers.GetParameters());
			// Añade los datos de autenticación
			AuthenticationViewModel.UpdateAuthentication(Connection);
			// Indica que está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public ConnectionsListViewModel ListConnectionsViewModel { get; }

	/// <summary>
	///		Datos de la conexión
	/// </summary>
	public ConnectionModel Connection { get; }

	/// <summary>
	///		Nombre
	/// </summary>
	public string Name
	{ 
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción
	/// </summary>
	public string? Description 
	{ 
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Url
	/// </summary>
	public string? Url 
	{ 
		get { return _url; }
		set { CheckProperty(ref _url, value); }
	}

	/// <summary>
	///		Tiempo de espera (segundos)
	/// </summary>
	public int Timeout
	{
		get { return _timeout; }
		set { CheckProperty(ref _timeout, value); }
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
	///		ViewModel de autentificación
	/// </summary>
	public ConnectionAuthenticationViewModel AuthenticationViewModel 
	{
		get { return _authenticationViewModel; }
		set { CheckObject(ref _authenticationViewModel!, value); }
	}
}
