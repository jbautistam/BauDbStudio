using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Files;

/// <summary>
///		ViewModel base para los viewmodels de un archivo
/// </summary>
public abstract class BaseFileViewModel : BaseObservableObject, Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _header, _fileName;

	public BaseFileViewModel(string fileName)
	{
		FileName = fileName;
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
	///		Cierra el viewModel
	/// </summary>
	public abstract void Close();

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

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
					Header = "Archivo";
			}
		}
	}
}
