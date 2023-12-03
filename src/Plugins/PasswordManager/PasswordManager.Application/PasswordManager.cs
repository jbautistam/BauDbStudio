namespace Bau.Libraries.PasswordManager.Application;

/// <summary>
///		Clase principal del manager de contraseñas
/// </summary>
public class PasswordManager
{
	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	public void Load(string fileName, string password)
	{
		File = new Repository.PasswordRepository().Load(fileName, password);
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void Save(string fileName, string password)
	{
		new Repository.PasswordRepository().Save(fileName, File, password);
	}

	/// <summary>
	///		Archivo con los elementos del lector de contraseñas
	/// </summary>
	public Models.FileModel File { get; private set; } = new();
}