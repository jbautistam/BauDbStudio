using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.LastFiles;

/// <summary>
///		ViewModel del último archivo abierto
/// </summary>
public class LastFileViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _header = string.Empty, _fileName = string.Empty, _icon = string.Empty;
	private int _index;

	public LastFileViewModel(PluginsStudioViewModel mainViewModel, string fileName, string icon, int index)
	{
		// Asigna las propiedades
		MainViewModel =	mainViewModel;
		FileName = fileName;
		Icon = icon;
		Index = index;
		// Asigna el comando
		OpenFileCommand = new BauMvvm.ViewModels.BaseCommand(_ => OpenFile(), _ => CanOpenFile());
	}

	/// <summary>
	///		Comprueba si se puede abrir un archivo
	/// </summary>
	private bool CanOpenFile() => !string.IsNullOrEmpty(FileName) && File.Exists(FileName);

	/// <summary>
	///		Abre el archivo
	/// </summary>
	private void OpenFile()
	{
		MainViewModel.MainController.HostPluginsController.OpenFile(FileName);
	}

	/// <summary>
	///		Asigna la cabecera
	/// </summary>
	private void AssignHeader()
	{
		if (!string.IsNullOrWhiteSpace(FileName))
			Header = Index + " " + WrapName(FileName);
	}

	/// <summary>
	///		Limita el nombre de archivo
	/// </summary>
	private string WrapName(string fileName)
	{
		string result = string.Empty;

			// El resultado son 5 directorios
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				string[] parts = fileName.Split('\\');

					if (parts.Length <= 5)
						result = fileName;
					else
					{
						int index = 0;

							// Añade los últimos cinco directorios
							foreach (string part in parts)
							{
								// Añade el directorio
								if (index == 0 || index >= parts.Length - 5)
									result = result.AddWithSeparator(part, "\\", false);
								// Añade el separador inicial
								if (index == 0)
									result += "\\...";
								// Incrementa el índice
								index++;
							}
					}
			}
			// Devuelve la cadena resultante
			return result;
	}

	/// <summary>
	///		ViewModel de la ventana principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

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
				AssignHeader();
		}
	}

	/// <summary>
	///		Icono
	/// </summary>
	public string Icon
	{
		get { return _icon; }
		set { CheckProperty(ref _icon, value); }
	}

	/// <summary>
	///		Indice del archivo en la lista
	/// </summary>
	public int Index
	{
		get { return _index; }
		set
		{
			if (CheckProperty(ref _index, value))
				AssignHeader();
		}
	}

	/// <summary>
	///		Comando para abrir el archivo
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand OpenFileCommand { get; }
}
