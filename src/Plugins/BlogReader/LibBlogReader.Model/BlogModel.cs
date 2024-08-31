namespace Bau.Libraries.LibBlogReader.Model;

/// <summary>
///		Clase con los datos de un blog
/// </summary>
public class BlogModel : LibDataStructures.Base.BaseExtendedModel
{ 
	// Variables privadas
	private string path = string.Empty;

	/// <summary>
	///		Directorio para grabación de las entradas
	/// </summary>
	public string Path
	{
		get
		{
			// Obtiene el nombre de archivo
			if (string.IsNullOrEmpty(path))
				path = LibHelper.Files.HelperFiles.Normalize(URL, false);
			// Devuelve el nombre de archivo
			return path;
		}
		set { path = value; }
	}

	/// <summary>
	///		URL donde se encuentra el archivo RSS del blog
	/// </summary>
	public string URL { get; set; } = default!;

	/// <summary>
	///		Indica si se deben descargar automáticamente los postcasts
	/// </summary>
	public bool DownloadPodcast { get; set; }

	/// <summary>
	///		Indica si el blog está habilitado para descarga
	/// </summary>
	public bool Enabled { get; set; } = true;

	/// <summary>
	///		Indica si se deben eliminar las entradas leidas
	/// </summary>
	public bool DeleteOldEntries { get; set; }

	/// <summary>
	///		Fecha de última descarga
	/// </summary>
	public DateTime? DateLastDownload { get; set; }

	/// <summary>
	///		Fecha de la última entrada descargada
	/// </summary>
	public DateTime? DateLastEntry { get; set; }

	/// <summary>
	///		Número de elementos no leídos
	/// </summary>
	public int NumberNotRead { get; set; }

	/// <summary>
	///		Carpeta a la que pertenece el blog
	/// </summary>
	public FolderModel? Folder { get; set; }
}
