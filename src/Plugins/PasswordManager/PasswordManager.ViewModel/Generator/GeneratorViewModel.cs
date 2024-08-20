namespace Bau.Libraries.PasswordManager.ViewModel.Generator;

/// <summary>
///		ViewModel del generador de contraseñas
/// </summary>
public class GeneratorViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private Random _rnd = new();
	private string _password = default!, _except = default!, _include = default!;
	private int  _minLength, _maxLength;
	private bool _withUpperCase, _withLowerCase, _withDigit, _withSpecial;

	public GeneratorViewModel(Reader.EntryViewModel entryViewModel)
	{
		// Inicializa las propiedades
		EntryViewModel = entryViewModel;
		// Inicializa el viewModel
		InitViewModel();
		// Inicializa los comandos
		GenerateCommand = new BauMvvm.ViewModels.BaseCommand(_ => GeneratePassword());
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		Password = string.Empty;
		Except = "\"'";
		MinLength = 7;
		MaxLength = 18;
		WithUpperCase = true;
		WithLowerCase = true;
		WithDigit = true;
		WithSpecial = true;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Genera la contraseña
	/// </summary>
	private void GeneratePassword()
	{
		if (MinLength == 0 || MaxLength == 0 || MinLength < 3 || MaxLength < 3)
			EntryViewModel.FileViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Introduzca una longitud mínima y máxima mayor que 3");
		else if (!WithUpperCase && !WithLowerCase && !WithDigit && !WithSpecial)
			EntryViewModel.FileViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Seleccione al menos un tipo de carácter");
		else
			Password = GeneratePassword(GetCharactersBase(), Except, GetLength(MinLength, MaxLength));
	}

	/// <summary>
	///		Genera la contraseña
	/// </summary>
	private string GeneratePassword(List<string> characters, string except, int length)
	{
		string password = string.Empty;

			// Genera la contraseña
			while (password.Length < length)
				for (int index = 0; index < characters.Count && password.Length < length; index++)
					password += GetPasswordChar(characters[index], except);
			// Devuelve la contraseña generada
			return Shuffle(password, 1_000);
	}

	/// <summary>
	///		Obtiene un carácter valido para la contraseña
	/// </summary>
	private char? GetPasswordChar(string characters, string except)
	{
		char passwordChar;
		int loops = 0;

			// Obtiene un carácter válido
			do
			{
				passwordChar = characters[_rnd.Next(0, characters.Length)];
			}
			while (!string.IsNullOrWhiteSpace(except) && except.IndexOf(passwordChar) >= 0 && ++loops < 2_000);
			// Si se ha pasado el número de bucles es porque el carácter no es válido
			if (loops >= 2_000)
				return null;
			else
				return passwordChar;
	}

	/// <summary>
	///		Desordena una cadena
	/// </summary>
	private string Shuffle(string value, int loops)
	{
		char[] characters = value.ToCharArray();
		string result = string.Empty;

			// Desordena el array de caracteres
			for (int index = 0; index < loops; index++)
			{
				int top = _rnd.Next(0, characters.Length);
				int bottom = _rnd.Next(0, characters.Length);

					// Intercambia los caracteres
					if (top != bottom)
					{
						char inter = characters[top];

							// Cambia los caracteres
							characters[top] = characters[bottom];
							characters[bottom] = inter;
					}
			}
			// Crea la cadena
			foreach (char character in characters)
				result += character;
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Obtiene la longitud de la cadena
	/// </summary>
	private int GetLength(int minLength, int maxLength)
	{
		if (minLength == maxLength)
			return minLength;
		else if (MinLength > MaxLength)
			return _rnd.Next(MaxLength, MinLength);
		else
			return _rnd.Next(MinLength, MaxLength);
	}

	/// <summary>
	///		Obtiene la lista de caracteres base con los que puede crear la contraseña
	/// </summary>
	private List<string> GetCharactersBase()
	{
		List<string> characters = new();

			// Añade los caracteres
			if (WithUpperCase)
				characters.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
			if (WithLowerCase)
				characters.Add("abcdefghijklmnopqrstuvwxyz");
			if (WithDigit)
				characters.Add("0123456789");
			if (WithSpecial)
				characters.Add("\"' ;,.#@!¿?=)(/&%");
			if (!string.IsNullOrWhiteSpace(Include))
			{
				if (characters.Count > 0)
					characters[0] = characters[0] + Include;
				else
					characters.Add(Include);
			}
			// Devuelve la lista de caracteres
			return characters;
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Password))
				EntryViewModel.FileViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Genere la contraseña");
			else
				validated = true;
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
			// Indica que ya no es nuevo y está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public Reader.EntryViewModel EntryViewModel { get; }

	/// <summary>
	///		Contraseña generada
	/// </summary>
	public string Password
	{
		get { return _password; }
		set { CheckProperty(ref _password, value); }
	}

	/// <summary>
	///		Cadena con caracteres que no se pueden introducir en la contraseña
	/// </summary>
	public string Except
	{
		get { return _except; }
		set { CheckProperty(ref _except, value); }
	}

	/// <summary>
	///		Cadena con caracteres que se deben introducir en la contraseña
	/// </summary>
	public string Include
	{
		get { return _include; }
		set { CheckProperty(ref _include, value); }
	}

	/// <summary>
	///		Longitud mínima
	/// </summary>
	public int MinLength
	{
		get { return _minLength; }
		set { CheckProperty(ref _minLength, value); }
	}

	/// <summary>
	///		Longitud máxima
	/// </summary>
	public int MaxLength
	{
		get { return _maxLength; }
		set { CheckProperty(ref _maxLength, value); }
	}

	/// <summary>
	///		Indica si la contraseña debe incluir letras mayúsculas
	/// </summary>
	public bool WithUpperCase
	{
		get { return _withUpperCase; }
		set { CheckProperty(ref _withUpperCase, value); }
	}

	/// <summary>
	///		Indica si la contraseña debe incluir letras minúsculas
	/// </summary>
	public bool WithLowerCase
	{
		get { return _withLowerCase; }
		set { CheckProperty(ref _withLowerCase, value); }
	}

	/// <summary>
	///		Indica si la contraseña debe incluir dígitos
	/// </summary>
	public bool WithDigit
	{
		get { return _withDigit; }
		set { CheckProperty(ref _withDigit, value); }
	}

	/// <summary>
	///		Indica si la contraseña debe incluir caracteres especiales
	/// </summary>
	public bool WithSpecial
	{
		get { return _withSpecial; }
		set { CheckProperty(ref _withSpecial, value); }
	}

	/// <summary>
	///		Comando de generación
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand GenerateCommand { get; }
}