using Bau.Libraries.PasswordManager.Application.Models;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.OneTimePassword;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader;

/// <summary>
///		ViewModel para la página
/// </summary>
public class EntryViewModel : BaseObservableObject
{
	// Variables privadas
	private string _name = default!, _description = default!, _url = default!, _notes = default!, _user = default!, _password = default!, _repeatPassword = default!;
	private string? _key, _error;
	private string _hotpCode = default!, _totpCode = default!, _remainingTime = default!;
	private int _digits, _interval, _previousRemaining = -1;
	private long _counter;
	private ComboViewModel? _comboEncodings;
	private ComboViewModel? _comboShaAlgorithms;
	private System.Timers.Timer? _timer;
	private MvvmColor _remainingTimeColor = MvvmColor.Green;
	private bool _useOtp;

	public EntryViewModel(PasswordFileViewModel fileViewModel, EntryModel entry)
	{
		// Asigna las propiedades
		FileViewModel = fileViewModel;
		Entry = entry;
		// Inicializa el control
		InitControl();
		// Asigna el controlador de modificaciones
		PropertyChanged += (sender, args) => {
												if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
														args.PropertyName.Equals(nameof(IsUpdated), StringComparison.CurrentCultureIgnoreCase))
													FileViewModel.IsUpdated = true;
											 };
		// Asigna los comandos
		CopyPasswordCommand = new BaseCommand(_ => CopyPassword());
		CreatePasswordCommand = new BaseCommand(_ => CreatePassword());
		CopyHotpCommand = new BaseCommand(_ => CopyCode(true));
		CopyTotpCommand = new BaseCommand(_ => CopyCode(false));
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private void InitControl()
	{
		// Carga los combos
		LoadComboEncodings();
		LoadComboShaAlgorithms();
		// Genera una contraseña si no existía
		if (string.IsNullOrWhiteSpace(Entry.Password))
			Entry.Password = "P4ssw0rd";
		// Asigna las propiedades
		Name = Entry.Name;
		Description = Entry.Description;
		Url = Entry.Url;
		Notes = Entry.Notes;
		User = Entry.User;
		Password = Entry.Password;
		RepeatPassword = Entry.Password;
		// Inicializa los datos de OTP
		Key = Entry.AuthKey;
		Digits = Entry.Digits;
		Counter = Entry.Counter;
		Interval = Entry.Interval;
		if (ComboEncodings is not null)
		{
			ComboEncodings.SelectedId = (int) Entry.Encoding;
			ComboEncodings.PropertyChanged += (sender, args) => {
																	if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																			args.PropertyName.Equals(nameof(ComboEncodings.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																		UpdateCode();
																};
		}
		if (ComboShaAlgorithms is not null)
		{
			ComboShaAlgorithms.SelectedId = (int) Entry.HashAlgorithm;
			ComboShaAlgorithms.PropertyChanged += (sender, args) => {
																		if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																				args.PropertyName.Equals(nameof(ComboShaAlgorithms.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																			UpdateCode();
																	};
		}
		// Indica si se utiliza OTP
		UseOtp = !string.IsNullOrWhiteSpace(Key);
		// Inicializa el temporizador
		_timer = new System.Timers.Timer();
		_timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
		_timer.Elapsed += (sender, args) => UpdateCodeTimer();
		_timer.Start();
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Carga el combo de codificaciones
	/// </summary>
	private void LoadComboEncodings()
	{
		ComboEncodings = new ComboViewModel(this);
		ComboEncodings.AddItem((int) EntryModel.SecretEncoding.Plain, "Plain");
		ComboEncodings.AddItem((int) EntryModel.SecretEncoding.Base32, "Base 32");
		ComboEncodings.AddItem((int) EntryModel.SecretEncoding.Base64, "Base 64");
		ComboEncodings.SelectedIndex = 0;
	}

	/// <summary>
	///		Obtiene la codificación seleccionada
	/// </summary>
	protected EntryModel.SecretEncoding GetEncoding()
	{
		if (ComboEncodings is null)
			return EntryModel.SecretEncoding.Plain;
		else
			return (EntryModel.SecretEncoding) (ComboEncodings.SelectedId ?? (int) EntryModel.SecretEncoding.Plain);
	}

	/// <summary>
	///		Carga el combo de algoritmos SHA
	/// </summary>
	private void LoadComboShaAlgorithms()
	{
		ComboShaAlgorithms = new ComboViewModel(this);
		ComboShaAlgorithms.AddItem((int) EntryModel.HmacAlgorithm.Sha1, "SHA 1");
		ComboShaAlgorithms.AddItem((int) EntryModel.HmacAlgorithm.Sha256, "SHA 256");
		ComboShaAlgorithms.AddItem((int) EntryModel.HmacAlgorithm.Sha512, "SHA 512");
		ComboShaAlgorithms.SelectedIndex = 0;
	}

	/// <summary>
	///		Obtiene el algoritmo SHA1 seleccionado
	/// </summary>
	protected EntryModel.HmacAlgorithm GetShaAlgorithm()
	{
		if (ComboShaAlgorithms is null)
			return EntryModel.HmacAlgorithm.Sha1;
		else
			return (EntryModel.HmacAlgorithm) (ComboShaAlgorithms.SelectedId ?? (int) EntryModel.HmacAlgorithm.Sha1);
	}

	/// <summary>
	///		Actualiza el código
	/// </summary>
	protected void UpdateCode()
	{
		Error = string.Empty;
		if (string.IsNullOrWhiteSpace(Key))
		{
			HotpCode = string.Empty;
			TotpCode = string.Empty;
		}
		else
		{
			UpdateHotpCode(Key);
			UpdateTotpCode(Key);
		}
	}

	/// <summary>
	///		Calcula el código Hotp
	/// </summary>
	private void UpdateHotpCode(string key)
	{
		try
		{
			HotpGenerator hotpGenerator = new(key, ConvertEncoding(GetEncoding()), ConvertShaAlgorithm(GetShaAlgorithm()), Digits);

				HotpCode = hotpGenerator.Compute(Counter);
		}
		catch (Exception exception)
		{
			HotpCode = string.Empty;
			Error = $"Error ({exception.Message})";
		}
	}

	/// <summary>
	///		Calcula el código Totp
	/// </summary>
	private void UpdateTotpCode(string key)
	{
		try
		{
			TotpGenerator totpGenerator = new(key, ConvertEncoding(GetEncoding()), ConvertShaAlgorithm(GetShaAlgorithm()), Digits);

				TotpCode = totpGenerator.Compute();
		}
		catch (Exception exception)
		{
			TotpCode = string.Empty;
			Error = $"Error ({exception.Message})";
		}
	}

	/// <summary>
	///		Convierte la codificación
	/// </summary>
	private Secret.Encoding ConvertEncoding(EntryModel.SecretEncoding encoding)
	{
		return encoding switch
					{
						EntryModel.SecretEncoding.Base32 => Secret.Encoding.Base32,
						EntryModel.SecretEncoding.Base64 => Secret.Encoding.Base64,
						_ => Secret.Encoding.Plain
					};
	}

	/// <summary>
	///		Convierte el algoritmo Hash
	/// </summary>
	private BaseTokenGenerator.HashAlgorithm ConvertShaAlgorithm(EntryModel.HmacAlgorithm algorithm)
	{
		return algorithm switch
					{
						EntryModel.HmacAlgorithm.Sha256 => BaseTokenGenerator.HashAlgorithm.Sha256,
						EntryModel.HmacAlgorithm.Sha512 => BaseTokenGenerator.HashAlgorithm.Sha512,
						_ => BaseTokenGenerator.HashAlgorithm.Sha1
					};
	}

	/// <summary>
	///		Modifica el temporizador
	/// </summary>
	private void UpdateCodeTimer()
	{
		// Vacía el contador de tiempo restante
		RemainingTime = string.Empty;
		RemainingTimeColor = MvvmColor.Black;
		// Si se puede calcular un código, se obtiene el tiempo pendiente
		if (!string.IsNullOrWhiteSpace(Key) && string.IsNullOrWhiteSpace(Error))
		{
			OneTimePassword.TimeTools.TotpTimeManager timeManager = new()
																		{
																			IntervalSeconds = Interval
																		};
			int remaining = timeManager.GetRemainingSeconds();

				// Muestra el tiempo
				RemainingTime = $"{remaining:#,##0}";
				// Actualiza el código si cambia la ventana de tiempo
				if (remaining > _previousRemaining)
				{
					UpdateCode();
					_previousRemaining = -1;
				}
				else
					_previousRemaining = remaining;
				// Actualiza el color dependiendo del tiempo restante
				UpdateColorTimer(remaining);
		}
	}

	/// <summary>
	///		Modifica el color en el que se muestra el tiempo restante
	/// </summary>
	private void UpdateColorTimer(int remaining)
	{
		if (Interval != 0)
		{
			double percent = remaining * 100 / Interval;

				if (percent < 30)
					RemainingTimeColor = MvvmColor.Red;
				else if (percent < 60)
					RemainingTimeColor = MvvmColor.Green;
				else
					RemainingTimeColor = MvvmColor.Navy;
		}
		else
			RemainingTimeColor = MvvmColor.Black;
	}

	/// <summary>
	///		Comprueba si los datos introducidos son correctos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Introduzca el nombre de la entrada");
			else if (!CheckPassword(Password, RepeatPassword))
				FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Compruebe las contraseñas");
			else if (UseOtp)
			{
				if (!string.IsNullOrWhiteSpace(Error))
					FileViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Check the window errors");
				else if (string.IsNullOrWhiteSpace(Key))
					FileViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Enter the provider secret");
				else
					validated = true;
			}
			else 
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Comprueba las contraseñas
	/// </summary>
	private bool CheckPassword(string password, string repeatPassword)
	{
		if (string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(repeatPassword))
			return true;
		else if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(repeatPassword) || !Password.Equals(repeatPassword))
			return false;
		else
			return true;
	}

	/// <summary>
	///		Actualiza la entrada
	/// </summary>
	internal bool UpdateEntry()
	{
		bool validated = ValidateData();

			// Si los datos son correctos, se modifica la entrada
			if (validated)
			{
				// Cambia los datos
				Entry.Name = Name;
				Entry.Description = Description;
				Entry.Url = Url;
				Entry.Notes = Notes;
				Entry.User = User;
				Entry.Password = Password;
				// Cambia los datos de OTP
				if (!UseOtp)
				{
					Entry.AuthKey = string.Empty;
					Entry.Encoding = EntryModel.SecretEncoding.Plain;
					Entry.HashAlgorithm = EntryModel.HmacAlgorithm.Sha1;
					Entry.Digits = 6;
					Entry.Counter = 0;
					Entry.Interval = 30;
				}
				else
				{
					Entry.AuthKey = Key;
					Entry.Encoding = GetEncoding();
					Entry.HashAlgorithm = GetShaAlgorithm();
					Entry.Digits = Digits;
					Entry.Counter = Counter;
					Entry.Interval = Interval;
				}
				// Actualiza el árbol
				FileViewModel.Tree.Refresh();
			}
			// Devuelve el valor que indica si se ha actualizado
			return validated;
	}

	/// <summary>
	///		Copia la contraseña en el portapapeles
	/// </summary>
	private void CopyPassword()
	{
		if (!string.IsNullOrWhiteSpace(Password))
			FileViewModel.MainViewModel.ViewsController.PluginController.MainWindowController.CopyToClipboard(Password);
	}

	/// <summary>
	///		Crea la contraseña
	/// </summary>
	private void CreatePassword()
	{
		Generator.GeneratorViewModel viewModel = new(this);

			// Si se ha creado una contraseña, se copia en los cuadros de texto
			if (FileViewModel.MainViewModel.ViewsController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes &&
				!string.IsNullOrWhiteSpace(viewModel.Password))
			{
				Password = viewModel.Password;
				RepeatPassword = Password;
			}
	}

	/// <summary>
	///		Copia un código en el portapapeles
	/// </summary>
	private void CopyCode(bool copyHotp)
	{
		if (string.IsNullOrEmpty(Error))
		{
			if (copyHotp)
				FileViewModel.MainViewModel.ViewsController.MainWindowController.CopyToClipboard(HotpCode);
			else
				FileViewModel.MainViewModel.ViewsController.MainWindowController.CopyToClipboard(TotpCode);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PasswordFileViewModel FileViewModel { get; }

	/// <summary>
	///		Datos de la entrada
	/// </summary>
	public EntryModel Entry { get; }

	/// <summary>
	///		Nombre del elemento
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción del elemento
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Url del elemento
	/// </summary>
	public string Url
	{
		get { return _url; }
		set { CheckProperty(ref _url, value); }
	}

	/// <summary>
	///		Notas del elemento
	/// </summary>
	public string Notes
	{
		get { return _notes; }
		set { CheckProperty(ref _notes, value); }
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
	///		Contraseña (repetición)
	/// </summary>
	public string RepeatPassword
	{
		get { return _repeatPassword; }
		set { CheckProperty(ref _repeatPassword, value); }
	}

	/// <summary>
	///		Clave
	/// </summary>
	public string? Key
	{
		get { return _key; }
		set 
		{ 
			if (CheckProperty(ref _key, value))
				UpdateCode();
		}
	}

	/// <summary>
	///		Combo de codificación
	/// </summary>
	public ComboViewModel? ComboEncodings
	{
		get { return _comboEncodings; }
		set 
		{ 
			if (CheckObject(ref _comboEncodings, value))
				UpdateCode();
		}
	}

	/// <summary>
	///		Combo de algoritmos SHA
	/// </summary>
	public ComboViewModel? ComboShaAlgorithms
	{
		get { return _comboShaAlgorithms; }
		set 
		{ 
			if (CheckObject(ref _comboShaAlgorithms, value))
				UpdateCode(); 
		}
	}

	/// <summary>
	///		Dígitos de salida
	/// </summary>
	public int Digits
	{
		get { return _digits; }
		set 
		{ 
			if (CheckProperty(ref _digits, value))
				UpdateCode();
		}
	}

	/// <summary>
	///		Contador utilizado para generar la clave
	/// </summary>
	public long Counter
	{
		get { return _counter; }
		set 
		{ 
			if (CheckProperty(ref _counter, value))
				UpdateCode();
		}
	}

	/// <summary>
	///		Intervalo en segundos utilizado para generar la clave
	/// </summary>
	public int Interval
	{
		get { return _interval; }
		set 
		{ 
			if (CheckProperty(ref _interval, value))
				UpdateCode();
		}
	}

	/// <summary>
	///		Mensaje de error
	/// </summary>
	public string? Error
	{
		get { return _error; }
		set { CheckProperty(ref _error, value); }
	}

	/// <summary>
	///		Código generado por el algoritmo HOTP
	/// </summary>
	public string HotpCode
	{
		get { return _hotpCode; }
		set { CheckPropertyNoUpdate(ref _hotpCode, value); }
	}

	/// <summary>
	///		Código generado por el algoritmo TOTP
	/// </summary>
	public string TotpCode
	{
		get { return _totpCode; }
		set { CheckPropertyNoUpdate(ref _totpCode, value); }
	}

	/// <summary>
	///		Tiempo restante
	/// </summary>
	public string RemainingTime
	{
		get { return _remainingTime; }
		set { CheckPropertyNoUpdate(ref _remainingTime, value); }
	}

	/// <summary>
	///		Color asignado al temporizador
	/// </summary>
	public MvvmColor RemainingTimeColor
	{
		get { return _remainingTimeColor; }
		set { CheckObjectNoEvent(ref _remainingTimeColor!, value); }
	}

	/// <summary>
	///		Indica si se va a utilizar OTP
	/// </summary>
	public bool UseOtp
	{
		get { return _useOtp; }
		set { CheckProperty(ref _useOtp, value); }
	}

	/// <summary>
	///		Comando para copiar una contraseña al portapapeles
	/// </summary>
	public BaseCommand CopyPasswordCommand { get; }

	/// <summary>
	///		Comando para crear una contraseña
	/// </summary>
	public BaseCommand CreatePasswordCommand { get; }

	/// <summary>
	///		Copia el código Hotp generado
	/// </summary>
	public BaseCommand CopyHotpCommand { get; }

	/// <summary>
	///		Copia el código Totp generado
	/// </summary>
	public BaseCommand CopyTotpCommand { get; }
}