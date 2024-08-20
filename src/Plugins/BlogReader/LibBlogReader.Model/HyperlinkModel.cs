namespace Bau.Libraries.LibBlogReader.Model;

/// <summary>
///		Clase con los datos de un hipervínculo
/// </summary>
public class HyperlinkModel : LibDataStructures.Base.BaseExtendedModel
{ 
	/// <summary>
	///		URL donde apunta el hipervínculo
	/// </summary>
	public string Url { get; set; } = default!;

	/// <summary>
	///		Carpeta a la que pertenece el hipervínculo
	/// </summary>
	public FolderModel? Folder { get; set; }
}
