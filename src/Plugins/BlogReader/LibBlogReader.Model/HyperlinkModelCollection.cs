namespace Bau.Libraries.LibBlogReader.Model;

/// <summary>
///		Colección de <see cref="HyperlinkModel"/>
/// </summary>
public class HyperlinkModelCollection : LibDataStructures.Base.BaseExtendedModelCollection<HyperlinkModel>
{
	/// <summary>
	///		Añade un <see cref="HyperlinkModel"/> a la colección
	/// </summary>
	public HyperlinkModel Add(string name, string description, string url)
	{
		HyperlinkModel newUrl = new() 
							{ 
								Name = name, 
								Description = description, 
								Url = url
							};

			// Añade la URL a la colección
			Add(newUrl);
			// Devuelve la URL añadida
			return newUrl;
	}
}
