namespace Bau.Libraries.DbStudio.ViewModels.Details.Connections;

/// <summary>
///		ViewModel de archivos de parámetros
/// </summary>
public class LastParameterFileViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas
	private string _path = string.Empty, _file = string.Empty;

	public LastParameterFileViewModel(string fileName) : base(fileName, fileName)
	{
		FileName = fileName;
		Path = System.IO.Path.GetDirectoryName(fileName) ?? string.Empty;
		File = System.IO.Path.GetFileName(fileName);
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; }

	/// <summary>
	///		Directorio
	/// </summary>
	public string Path
	{
		get { return _path; }
		set { CheckProperty(ref _path, value); }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string File
	{
		get { return _file; }
		set { CheckProperty(ref _file, value); }
	}
}
