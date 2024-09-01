using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PasswordManager.Application.Models;

namespace Bau.Libraries.PasswordManager.ViewModel.Reader;

/// <summary>
///		ViewModel para ver el contenido de un archivo de contraseñas
/// </summary>
public class PasswordFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName = default!, _password = default!;
	private bool _isSelectedEntry;
	private EntryViewModel? _selectedEntry;

	public PasswordFileViewModel(PasswordManagerViewModel mainViewModel, string fileName, string password) : base(false)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		Header = Path.GetFileNameWithoutExtension(fileName);
		_password = password;
		// Asigna los controles
		Tree = new Explorer.TreePasswordsViewModel(this);
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	internal bool LoadFile()
	{
		bool loaded = false;

			// Intenta cargar el archivo
			try
			{
				// Carga el archivo
				PasswordManager.Load(FileName, _password);
				// Carga el árbol
				Tree.Load();
				// Indica que se ha cargado el archivo
				loaded = true;
			}
			catch (Exception exception)
			{
				MainViewModel.ViewsController.Logger.LogError(exception, $"Error when load {FileName}. {exception.Message}");
				MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load {FileName}. {exception.Message}");
			}
			// Devuelve el valor que indica si ha podido cargar el archivo
			return loaded;
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "¿Desea grabar el archivo antes de continuar?";
		else
			return $"¿Desea grabar el archivo '{Path.GetFileName(FileName)}' antes de continuar?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// Graba el archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string? newFileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave(string.Empty, 
																								 "Archivo cifrado (*.bau.enc)|*.bau.enc|Todos los archivos (*.*)|*.*",
																								 FileName, ".bau.enc");

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Actualiza la entrada con los datos de la pantalla
			if (SelectedEntry is not null)
				SelectedEntry.UpdateEntry();
			// Graba el archivo
			PasswordManager.Save(FileName, _password);
			// Actualiza el árbol
			MainViewModel.ViewsController.HostPluginsController.RefreshFiles();
			// Añade el archivo a los últimos archivos abiertos
			MainViewModel.ViewsController.HostPluginsController.AddFileUsed(FileName);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Cierra la ventana de detalles: elimina los archivos temporales
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Comprueba si se puede modificar la entrada seleccionada
	/// </summary>
	internal bool CanUpdateSelectedEntry() => SelectedEntry is null || SelectedEntry.UpdateEntry();

	/// <summary>
	///		Descarga la entrada seleccionada (por ejemplo, si se han borrado los datos)
	/// </summary>
	internal void DiscardSelectedEntry()
	{
		SelectedEntry = null;
	}

	/// <summary>
	///		Actualiza la entrada seleccionada para que se visualice
	/// </summary>
	internal void UpdateSelectedEntry(EntryModel entry)
	{
		SelectedEntry = new EntryViewModel(this, entry);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PasswordManagerViewModel MainViewModel { get; set; }

	/// <summary>
	///		Manager de la aplicación de contraseñas
	/// </summary>
	public Application.PasswordManager PasswordManager { get; } = new();

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

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
	///		Arbol del explorador
	/// </summary>
	public Explorer.TreePasswordsViewModel Tree { get; }

	/// <summary>
	///		Entrada seleccionada
	/// </summary>
	public EntryViewModel? SelectedEntry 
	{ 
		get { return _selectedEntry; }
		private set
		{
			if (CheckObject(ref _selectedEntry, value))
				IsSelectedEntry = value is not null;
		}
	}

	/// <summary>
	///		Indica si hay alguna entrada seleccionada
	/// </summary>
	public bool IsSelectedEntry
	{
		get { return _isSelectedEntry; }
		set { CheckProperty(ref _isSelectedEntry, value); }
	}
}
