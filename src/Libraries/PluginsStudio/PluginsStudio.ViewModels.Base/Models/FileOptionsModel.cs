namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

/// <summary>
///		Opciones asociadas a un archivo
/// </summary>
public class FileOptionsModel
{
	/// <summary>
	///		Comprueba si esta opción se corresponde con una carpeta / archivo
	/// </summary>
	public bool Check(bool isFolder, string fileName)
	{
		// Comprueba si la carpeta / extensión coincide con las condiciones
		if (isFolder && ForFolder)
			return true;
		else if (!isFolder && !string.IsNullOrWhiteSpace(fileName))
			foreach (string extension in FileExtension)
				if (fileName.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
					return true;
		// Si ha llegado hasta aquí es porque no se corresponde
		return false;
	}

	/// <summary>
	///		Id principal
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Indica si es una opción para carpetas
	/// </summary>
	public bool ForFolder { get; set; }

	/// <summary>
	///		Extensiones de archivo a las que se aplica la opción
	/// </summary>
	public List<string> FileExtension { get; } = new();

	/// <summary>
	///		Opción de menú
	/// </summary>
	public MenuModel? Menu { get; set; }
}