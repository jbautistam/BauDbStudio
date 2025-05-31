using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Connections;

/// <summary>
///		ViewModel de datos de conexión
/// </summary>
public class ConnectionViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = string.Empty, _description = string.Empty, _connectionString = string.Empty;
	private string _server = string.Empty, _user = string.Empty, _password = string.Empty, _database = string.Empty, _fileName = string.Empty;
	private string _additionalProperties = string.Empty;
	private bool _useIntegratedSecurity, _multipleActiveResultSets, _isServerConnection, _isOdbcConnection, _hasFileName, _isNew, _showAdditionalProperties;
	private bool _hasInMemory, _inMemory, _hasPassword;
	private int _timeoutExecuteScriptMinutes, _port;
	private ComboViewModel _comboTypes = default!;

	public ConnectionViewModel(DbStudioViewModel solutionViewModel, ConnectionModel? connection)
	{
		// Inicializa las propiedades
		SolutionViewModel = solutionViewModel;
		IsNew = connection is null;
		Connection = connection ?? new ConnectionModel(solutionViewModel.Solution);
		// Inicializa el viewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Carga el combo de tipos
		LoadComboTypes();
		// Asigna las propiedades
		Name = Connection.Name;
		if (string.IsNullOrWhiteSpace(Name))
			Name = "Nueva conexión";
		Description = Connection.Description;
		ComboTypes.SelectedId = (int) Connection.Type;
		Server = Connection.Parameters[nameof(Server)];
		Port = Connection.Parameters[nameof(Port)].GetInt(1433);
		User = Connection.Parameters[nameof(User)];
		Password = Decrypt(Connection.Parameters[nameof(Password)]);
		Database = Connection.Parameters[nameof(Database)];
		InMemory = Connection.Parameters[nameof(InMemory)].GetBool();
		UseIntegratedSecurity = Connection.Parameters[nameof(UseIntegratedSecurity)].GetBool();
		if (IsNew)
			MultipleActiveResultSets = true;
		else
			MultipleActiveResultSets = Connection.Parameters[nameof(MultipleActiveResultSets)].GetBool();
		ConnectionString = Connection.Parameters[nameof(ConnectionString)];
		AdditionalProperties = GetAdditionalProperties(Connection.Parameters);
		FileName = Connection.Parameters[nameof(FileName)];
		TimeoutExecuteScriptMinutes = (int) Connection.TimeoutExecuteScript.TotalMinutes;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Carga el combo de tipos
	/// </summary>
	private void LoadComboTypes()
	{
		// Crea el combo
		ComboTypes = new ComboViewModel(this);
		// Añade los elementos
		ComboTypes.AddItem(-1, "<Connection type>");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.Spark, "Spark");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.SqlServer, "Sql server");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.Odbc, "Odbc");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.SqLite, "SqLite");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.MySql, "MySql");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.PostgreSql, "Postgresql");
		ComboTypes.AddItem((int) ConnectionModel.ConnectionType.DuckDb, "DuckDb");
		// Selecciona el primer elemento
		ComboTypes.SelectedItem = ComboTypes.Items[0];
		// Asigna el manejador de eventos
		ComboTypes.PropertyChanged += (sender, args) =>
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName) && args.PropertyName.Equals(nameof(ComboTypes.SelectedItem)))
												{
													ConnectionModel.ConnectionType type = (ConnectionModel.ConnectionType) (ComboTypes.SelectedId ?? 0);

														// Cambia las propiedades
														IsServerConnection = type == ConnectionModel.ConnectionType.SqlServer ||
																			 type == ConnectionModel.ConnectionType.PostgreSql ||
																			 type == ConnectionModel.ConnectionType.MySql;
														HasFileName = type == ConnectionModel.ConnectionType.SqLite || type == ConnectionModel.ConnectionType.DuckDb;
														HasPassword = type == ConnectionModel.ConnectionType.SqLite;
														HasInMemory = type == ConnectionModel.ConnectionType.DuckDb;
														IsOdbcConnection = type == ConnectionModel.ConnectionType.Odbc || type == ConnectionModel.ConnectionType.Spark;
														ShowAdditionalProperties = !IsOdbcConnection && type != ConnectionModel.ConnectionType.DuckDb;
														// Muestra el puerto predeterminado
														if (IsNew)
															Port = GetDefaultPort(type);
												}
											};

		// Obtiene el puerto predeterminado
		int GetDefaultPort(ConnectionModel.ConnectionType type)
		{
			return type switch
					{
						ConnectionModel.ConnectionType.SqlServer => 1433,
						ConnectionModel.ConnectionType.MySql => 3306,
						ConnectionModel.ConnectionType.PostgreSql => 5432,
						_ => 0
					};
		}
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de la conexión");
			else if (ComboTypes.SelectedId == null)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione un tipo");
			else if (ValidateConnection())
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Comprueba la conexión
	/// </summary>
	private bool ValidateConnection()
	{
		bool validated = false;

			// Comprueba los datos
			if (IsServerConnection)
			{
				if (string.IsNullOrWhiteSpace(Server))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca la dirección del servidor");
				else if (!UseIntegratedSecurity && string.IsNullOrWhiteSpace(User))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el código de usuario");
				else if (!UseIntegratedSecurity && string.IsNullOrWhiteSpace(Password))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca la contraseña");
				else if (string.IsNullOrWhiteSpace(Database))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de la base de datos");
				else
					validated = true;
			}
			else if (HasFileName)
			{
				if (string.IsNullOrWhiteSpace(FileName) && !InMemory)
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca el nombre de archivo");
				else
					validated = true;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(ConnectionString))
					SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca la cadena de conexión");
				else
					validated = true;
			}
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
			// Asigna los datos al proyecto
			Connection.Name = Name;
			Connection.Description = Description;
			Connection.Type = (ConnectionModel.ConnectionType) (ComboTypes.SelectedId ?? 0);
			// Asigna los parámetros
			Connection.Parameters.Clear();
			Connection.Parameters[nameof(Server)] = Server;
			Connection.Parameters[nameof(Port)] = Port.ToString();
			Connection.Parameters[nameof(User)] = User;
			Connection.Parameters[nameof(Password)] = Encrypt(Password);
			Connection.Parameters[nameof(Database)] = Database;
			Connection.Parameters[nameof(UseIntegratedSecurity)] = UseIntegratedSecurity.ToString();
			Connection.Parameters[nameof(MultipleActiveResultSets)] = MultipleActiveResultSets.ToString();
			if (IsOdbcConnection)
				Connection.Parameters[nameof(ConnectionString)] = ConnectionString;
			if (HasFileName)
				Connection.Parameters[nameof(FileName)] = FileName;
			if (ShowAdditionalProperties)
				Connection.Parameters[nameof(AdditionalProperties)] = AdditionalProperties;
			if (HasInMemory)
				Connection.Parameters[nameof(InMemory)] = InMemory.ToString();
			Connection.TimeoutExecuteScript = TimeSpan.FromMinutes(TimeoutExecuteScriptMinutes);
			// Añade la conexión a la solución si es necesario
			if (IsNew)
				SolutionViewModel.Solution.Connections.Add(Connection);
			// Graba la solución
			SolutionViewModel.SaveSolution();
			// Actualiza el combo de conexiones
			SolutionViewModel.ConnectionExecutionViewModel.Load();
			// Indica que ya no es nuevo y está grabado
			IsNew = false;
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		Obtiene las propiedades adicionales
	/// </summary>
	private string GetAdditionalProperties(NormalizedDictionary<string> parameters)
	{
		// Añade las propiedades que no estén en el diccionario
		foreach ((string key, string value) in parameters.Enumerate())
			if (key.Equals(nameof(AdditionalProperties), StringComparison.CurrentCultureIgnoreCase))
				return value;
		// Si ha llegado hasta aquí es porque no se ha definido ninguna propiedad adicional
		return string.Empty;
	}

	/// <summary>
	///		Encripta una cadena
	/// </summary>
	private string Encrypt(string text)
	{
		return text;

		// => GetCryptographyController().EncryptToBase64(text);
	}

	/// <summary>
	///		Desencripta una cadena
	/// </summary>
	private string Decrypt(string text)
	{
		return text;
		//// Desencripta una cadena
		//if (!string.IsNullOrWhiteSpace(text))
		//	try
		//	{
		//		return GetCryptographyController().DecryptFromBase64(text);
		//	}
		//	catch (Exception exception)
		//	{
		//		SolutionViewModel.MainController.HostController.SystemController.ShowMessage($"Error when decrypt password. {exception.Message}");
		//	}
		//// Si ha llegado hasta aquí es porque no ha podido desencriptar la cadena
		//return string.Empty;
	}

	///// <summary>
	/////		Obtiene el controlador criptográfico
	///// </summary>
	//private LibCryptography.Cryptography.CryptographyController GetCryptographyController()
	//{
	//	return new LibCryptography.Cryptography.CryptographyController(LibCryptography.Cryptography.CryptographyController.CryptographyProviders.TripleDES, "key");
	//}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Datos de conexión
	/// </summary>
	public ConnectionModel Connection { get; }

	/// <summary>
	///		Indica si es nuevo
	/// </summary>
	public bool IsNew
	{
		get { return _isNew; }
		set { CheckProperty(ref _isNew, value); }
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
	///		Descripción
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Tipos de conexión
	/// </summary>
	public ComboViewModel ComboTypes
	{
		get { return _comboTypes; }
		set { CheckObject(ref _comboTypes!, value); }
	}

	/// <summary>
	///		Indica si es un tipo de conexión a servidor
	/// </summary>
	public bool IsServerConnection
	{
		get { return _isServerConnection; }
		set { CheckProperty(ref _isServerConnection, value); }
	}

	/// <summary>
	///		Indica si es un tipo de conexión ODBC
	/// </summary>
	public bool IsOdbcConnection
	{
		get { return _isOdbcConnection; }
		set { CheckProperty(ref _isOdbcConnection, value); }
	}

	/// <summary>
	///		Indica si es un tipo de conexión que necesita un nombre de archivo
	/// </summary>
	public bool HasFileName
	{
		get { return _hasFileName; }
		set { CheckProperty(ref _hasFileName, value); }
	}

	/// <summary>
	///		Indica si es un tipo de conexión que puede estar en memoria
	/// </summary>
	public bool HasInMemory
	{
		get { return _hasInMemory; }
		set { CheckProperty(ref _hasInMemory, value); }
	}

	/// <summary>
	///		Indica si es un tipo de conexión que puede tener contraseña
	/// </summary>
	public bool HasPassword
	{
		get { return _hasPassword; }
		set { CheckProperty(ref _hasPassword, value); }
	}

	/// <summary>
	///		Indica si se deben mostrar las propiedades adicionales
	/// </summary>
	public bool ShowAdditionalProperties
	{
		get { return _showAdditionalProperties; }
		set { CheckProperty(ref _showAdditionalProperties, value); }
	}

	/// <summary>
	///		Servidor
	/// </summary>
	public string Server
	{
		get { return _server; }
		set { CheckProperty(ref _server, value); }
	}

	/// <summary>
	///		Puerto
	/// </summary>
	public int Port
	{
		get { return _port; }
		set { CheckProperty(ref _port, value); }
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
	///		Base de datos
	/// </summary>
	public string Database
	{
		get { return _database; }
		set { CheckProperty(ref _database, value); }
	}

	/// <summary>
	///		Indica si se debe utilizar seguridad integrada
	/// </summary>
	public bool UseIntegratedSecurity
	{
		get { return _useIntegratedSecurity; }
		set { CheckProperty(ref _useIntegratedSecurity, value); }
	}

	/// <summary>
	///		Indica si se pueden utilizar varios conjuntos de resultados en las consultas
	/// </summary>
	public bool MultipleActiveResultSets
	{
		get { return _multipleActiveResultSets; }
		set { CheckProperty(ref _multipleActiveResultSets, value); }
	}

	/// <summary>
	///		Cadena de conexión
	/// </summary>
	public string ConnectionString
	{
		get { return _connectionString; }
		set { CheckProperty(ref _connectionString, value); }
	}

	/// <summary>
	///		Minutos de timeout para la ejecución de scripts
	/// </summary>
	public int TimeoutExecuteScriptMinutes
	{
		get { return _timeoutExecuteScriptMinutes; }
		set { CheckProperty(ref _timeoutExecuteScriptMinutes, value); }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Indica si la conexión es en memoria
	/// </summary>
	public bool InMemory
	{
		get { return _inMemory; }
		set { CheckProperty(ref _inMemory, value); }
	}

	/// <summary>
	///		Propiedades adicionales
	/// </summary>
	public string AdditionalProperties
	{
		get { return _additionalProperties; }
		set { CheckProperty(ref _additionalProperties, value); }
	}
}