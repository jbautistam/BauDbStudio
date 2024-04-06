using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Files;

/// <summary>
///		ViewModel base para los viewmodels de un archivo
/// </summary>
public abstract class BaseFileViewModel : BaseObservableObject, Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _header = string.Empty, _fileName = string.Empty, _mask = string.Empty;

	public BaseFileViewModel(Controllers.IPluginsController pluginsController, string fileName, string mask)
	{
		PluginsController = pluginsController;
		FileName = fileName;
		Mask = mask;
	}

	/// <summary>
	///		Carga el texto del archivo
	/// </summary>
	public abstract void Load();

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public abstract void SaveDetails(bool newName);

	/// <summary>
	///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "¿Desea grabar el archivo antes de continuar?";
		else
			return $"¿Desea grabar el archivo '{Path.GetFileName(FileName)}' antes de continuar?";
	}

	/// <summary>
	///		Actualiza el nombre de archivo
	/// </summary>
	protected void UpdateFileName(string oldTabId)
	{
		// Actualiza el árbol
		PluginsController.HostPluginsController.RefreshFiles();
		// Añade el archivo a los últimos archivos abiertos
		PluginsController.HostPluginsController.AddFileUsed(FileName);
		// Cambia la cabecera
		PluginsController.MainWindowController.UpdateTabId(oldTabId, TabId, Header);
	}

	/// <summary>
	///		Cierra el viewModel
	/// </summary>
	public abstract void Close();

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		Máscara de archivos
	/// </summary>
	public string Mask 
	{ 
		get 
		{
			const string maskAll = "All files (*.*)|*.*";

				// Añade el filtro predeterminado
				if (string.IsNullOrWhiteSpace(_mask))
					_mask = maskAll;
				_mask = _mask.Trim();
				if (!_mask.EndsWith("|*.*"))
					_mask += "|" + maskAll;
				// Devuelve la máscara
				return _mask;
		}
		protected set { _mask = value; }
	}

	/// <summary>
	///		Controlador de plugins
	/// </summary>
	public Controllers.IPluginsController PluginsController { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

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
				if (!string.IsNullOrWhiteSpace(value))
					Header = Path.GetFileName(value);
				else
					Header = "File";
			}
		}
	}
}
