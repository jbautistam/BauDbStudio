namespace Bau.Libraries.CloudStudio.Models;

/// <summary>
///		Clase con los datos de la solución
/// </summary>
public class SolutionModel : LibDataStructures.Base.BaseExtendedModel
{
	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; set; } = default!;

	/// <summary>
	///		Directorio base de la solución
	/// </summary>
	public string Path
	{
		get 
		{
			if (string.IsNullOrWhiteSpace(FileName))
				return string.Empty;
			else
				return System.IO.Path.GetDirectoryName(FileName) ?? string.Empty;
		}
	}

	/// <summary>
	///		Blob storage
	/// </summary>
	public Cloud.StorageModelCollection	Storages { get; } = new();

	/// <summary>
	///		Carpetas abiertas
	/// </summary>
	public List<string> Folders { get; } = new();
}
