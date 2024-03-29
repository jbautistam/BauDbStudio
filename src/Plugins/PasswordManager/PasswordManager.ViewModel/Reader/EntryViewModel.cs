﻿using Bau.Libraries.PasswordManager.Application.Models;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader;

/// <summary>
///		ViewModel para la página
/// </summary>
public class EntryViewModel : BaseObservableObject
{
	// Variables privadas
	private string _name = default!, _description = default!, _url = default!, _notes = default!, _user = default!, _password = default!, _repeatPassword = default!;

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
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private void InitControl()
	{
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
		// Indica que no ha habido modificaciones
		IsUpdated = false;
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
	///		ViewModel principal
	/// </summary>
	public PasswordFileViewModel FileViewModel { get; }

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
	///		Datos de la entrada
	/// </summary>
	public EntryModel Entry { get; }

	/// <summary>
	///		Comando para copiar una contraseña al portapapeles
	/// </summary>
	public BaseCommand CopyPasswordCommand { get; }

	/// <summary>
	///		Comando para crear una contraseña
	/// </summary>
	public BaseCommand CreatePasswordCommand { get; }
}