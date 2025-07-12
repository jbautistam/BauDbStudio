using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Connections;

/// <summary>
///		ViewModel para una conexión de un proyecto
/// </summary>
public class ConnectionAuthenticationViewModel : BaseObservableObject
{
	// Variables privadas
	private string _urlAuthorization = default!, _user = default!, _password = default!, _bearer = default!;
	private ComboViewModel? _comboTypes;

	public ConnectionAuthenticationViewModel(ConnectionViewModel connectionViewModel)
	{
		// Asigna las propiedades
		ConnectionViewModel = connectionViewModel;
		// Inicializa los datos
		InitViewModel();
	}

	/// <summary>
	///		Inicializa los datos
	/// </summary>
	private void InitViewModel()
	{
		// Carga el combo del tipo
		LoadComboMethods(ConnectionViewModel.Connection.Authentication.Type);
		// Muestra los valores
		switch (ConnectionViewModel.Connection.Authentication.Type)
		{
			case AuthenticationModel.AuthenticationType.Basic:
					User = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("User") ?? string.Empty;
					Password = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("Password") ?? string.Empty;
				break;
			case AuthenticationModel.AuthenticationType.ApiKey:
					Bearer = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("ApiKey") ?? string.Empty;
				break;
			case AuthenticationModel.AuthenticationType.Bearer:
					Bearer = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("Bearer") ?? string.Empty;
				break;
			case AuthenticationModel.AuthenticationType.Jwt:
			case AuthenticationModel.AuthenticationType.OAuth:
					UrlAuthorization = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("Url") ?? string.Empty;
					User = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("User") ?? string.Empty;
					Password = ConnectionViewModel.Connection.Authentication.SecurityOptions.GetValueOrDefault("Password") ?? string.Empty;
				break;
		}
	}

	/// <summary>
	///		Carga el combo de métodos
	/// </summary>
	private void LoadComboMethods(AuthenticationModel.AuthenticationType type)
	{
		// Crea la propiedad
		ComboTypes = new ComboViewModel(this);
		// Añade los elementos
		ComboTypes.AddItem((int) AuthenticationModel.AuthenticationType.None, "None");
		ComboTypes.AddItem((int) AuthenticationModel.AuthenticationType.Basic, "Basic");
		ComboTypes.AddItem((int) AuthenticationModel.AuthenticationType.Jwt, "Jwt");
		ComboTypes.AddItem((int) AuthenticationModel.AuthenticationType.ApiKey, "Api key");
		ComboTypes.AddItem((int) AuthenticationModel.AuthenticationType.Bearer, "Bearer");
		ComboTypes.AddItem((int) AuthenticationModel.AuthenticationType.OAuth, "OAuth");
		// Selecciona el primer elemento
		ComboTypes.SelectedId = (int) type;
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	public bool ValidateData()
	{
		AuthenticationModel.AuthenticationType type = GetSelectedType();

			// Comprueba si los datos son correctos
			return type switch
						{
							AuthenticationModel.AuthenticationType.Basic => ValidateBasicAuthenticationData(),
							AuthenticationModel.AuthenticationType.Jwt or 
								AuthenticationModel.AuthenticationType.OAuth => ValidateJwtAuthenticationData(),
							AuthenticationModel.AuthenticationType.ApiKey or
								AuthenticationModel.AuthenticationType.Bearer => ValidateBearerAuthenticationData(),
							_ => true
						};
	}

	/// <summary>
	///		Comprueba los datos de autenticación básica
	/// </summary>
	private bool ValidateBasicAuthenticationData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(User))
				ConnectionViewModel.ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.
					HostController.SystemController.ShowMessage("Enter the user");
			else if (string.IsNullOrWhiteSpace(Password))
				ConnectionViewModel.ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.
					HostController.SystemController.ShowMessage("Enter the password");
			else
				validated = true;
			// Devuelve el valor que indica si los valores son correctos
			return validated;
	}

	/// <summary>
	///		Comprueba los datos de autenticación JWT
	/// </summary>
	private bool ValidateJwtAuthenticationData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(UrlAuthorization))
				ConnectionViewModel.ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.
					HostController.SystemController.ShowMessage("Enter the authorization URL");
			else if (string.IsNullOrWhiteSpace(User))
				ConnectionViewModel.ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.
					HostController.SystemController.ShowMessage("Enter the user");
			else if (string.IsNullOrWhiteSpace(Password))
				ConnectionViewModel.ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.
					HostController.SystemController.ShowMessage("Enter the password");
			else
				validated = true;
			// Devuelve el valor que indica si los valores son correctos
			return validated;
	}

	/// <summary>
	///		Comprueba los datos de autenticación bearer
	/// </summary>
	private bool ValidateBearerAuthenticationData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(Bearer))
				ConnectionViewModel.ListConnectionsViewModel.FileViewModel.MainViewModel.ViewsController.
					HostController.SystemController.ShowMessage("Enter the bearer code");
			else
				validated = true;
			// Devuelve el valor que indica si los valores son correctos
			return validated;
	}

	/// <summary>
	///		Obtiene los datos de autenticación de la conexión
	/// </summary>
	public void UpdateAuthentication(ConnectionModel connection)
	{
		// Obtiene el tipo seleccionado
		connection.Authentication.Type = GetSelectedType();
		// Asigna los parámetros
		connection.Authentication.SecurityOptions.Clear();
		switch (connection.Authentication.Type)
		{
			case AuthenticationModel.AuthenticationType.Basic:
					connection.Authentication.SecurityOptions.Add("User", User);
					connection.Authentication.SecurityOptions.Add("Password", User);
				break;
			case AuthenticationModel.AuthenticationType.ApiKey:
					connection.Authentication.SecurityOptions.Add("ApiKey", Bearer);
				break;
			case AuthenticationModel.AuthenticationType.Bearer:
					connection.Authentication.SecurityOptions.Add("Bearer", Bearer);
				break;
			case AuthenticationModel.AuthenticationType.Jwt:
			case AuthenticationModel.AuthenticationType.OAuth:
					connection.Authentication.SecurityOptions.Add("Url", UrlAuthorization);
					connection.Authentication.SecurityOptions.Add("User", User);
					connection.Authentication.SecurityOptions.Add("Password", Password);
				break;
		}
	}

	/// <summary>
	///		Obtiene el tipo seleccionado en el combo
	/// </summary>
	private AuthenticationModel.AuthenticationType GetSelectedType() => (AuthenticationModel.AuthenticationType) (ComboTypes?.SelectedId ?? 0);

	/// <summary>
	///		ViewModel de la conexión
	/// </summary>
	public ConnectionViewModel ConnectionViewModel { get; }

	/// <summary>
	///		Combo con los métodos
	/// </summary>
	public ComboViewModel? ComboTypes
	{
		get { return _comboTypes; }
		set { CheckObject(ref _comboTypes, value); }
	}

	/// <summary>
	///		Url de autorización
	/// </summary>
	public string UrlAuthorization
	{
		get { return _urlAuthorization; }
		set { CheckProperty(ref _urlAuthorization, value); }
	}

	/// <summary>
	///		Usuario
	/// </summary>
	public string User
	{
		get { return _user; }
		set { CheckProperty(ref _user, value); }
	}

	/// <summary>
	///		Contraseña
	/// </summary>
	public string Password
	{
		get { return _password; }
		set { CheckProperty(ref _password, value); }
	}

	/// <summary>
	///		Código bearer
	/// </summary>
	public string Bearer
	{
		get { return _bearer; }
		set { CheckProperty(ref _bearer, value); }
	}
}
