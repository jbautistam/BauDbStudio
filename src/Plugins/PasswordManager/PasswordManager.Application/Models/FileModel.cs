namespace Bau.Libraries.PasswordManager.Application.Models;

/// <summary>
///		Clase con los datos de un archivo de contraseñas
/// </summary>
public class FileModel
{
	/// <summary>
	///		Clave de salto
	/// </summary>
	public string Salt { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Carpeta raíz con las carpetas contenidas en el archivo
	/// </summary>
	public FolderModel Root { get; } = new();
}
