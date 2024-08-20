using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibBlogReader.Model;

/// <summary>
///		Clase con los datos de una carpeta
/// </summary>
public class FolderModel : LibDataStructures.Base.BaseExtendedModel
{
	/// <summary>
	///		Obtiene la carpeta principal
	/// </summary>
	public FolderModel GetRoot()
	{
		if (Parent == null)
			return this;
		else
			return Parent.GetRoot();
	}

	/// <summary>
	///		Obtiene recursivamente los blogs
	/// </summary>
	public BlogsModelCollection GetBlogsRecursive()
	{
		BlogsModelCollection blogs = [];

			// Añade los blogs de la carpeta
			foreach (BlogModel blog in Blogs)
				blogs.Add(blog);
			// Añade recursivamente los blogs de las carpetas
			foreach (FolderModel folder in Folders)
				blogs.AddRange(folder.GetBlogsRecursive());
			// Devuelve la colección de blogs
			return blogs;
	}

	/// <summary>
	///		Obtiene el número de elementos no leídos
	/// </summary>
	public int GetNumberNotRead()
	{ 
		// Calcula el número de elementos no leídos
		NumberNotRead = Blogs.GetNumberNotRead() + Folders.GetNumberNotRead();
		// Y devuelve el valor
		return NumberNotRead;
	}

	/// <summary>
	///		Borra una carpeta
	/// </summary>
	public void Delete(FolderModel folder)
	{
		if (Folders.Search(folder.GlobalId) != null)
			Folders.RemoveById(folder.GlobalId);
		else
			foreach (FolderModel childFolder in Folders)
				childFolder.Delete(folder);
	}

	/// <summary>
	///		Borra un blog
	/// </summary>
	public void Delete(BlogModel blog)
	{
		if (Blogs.Search(blog.GlobalId) != null)
			Blogs.RemoveById(blog.GlobalId);
		else
			foreach (FolderModel childFolder in Folders)
				childFolder.Delete(blog);
	}

	/// <summary>
	///		Borra un <see cref="HyperlinkModel"/>
	/// </summary>
	public void Delete(HyperlinkModel hyperlink)
	{
		if (Hyperlinks.Search(hyperlink.GlobalId) != null)
			Hyperlinks.RemoveById(hyperlink.GlobalId);
		else
			foreach (FolderModel childFolder in Folders)
				childFolder.Delete(hyperlink);
	}

	/// <summary>
	///		Actualiza el número de elementos no leidos de un blog
	/// </summary>
	public void UpdateNumberNotRead(BlogModel blog, int numberNotRead)
	{
		// Modifica los blogs de las carpetas
		foreach (FolderModel folder in Folders)
			folder.UpdateNumberNotRead(blog, numberNotRead);
		// Modifica los blogs hijo
		foreach (BlogModel child in Blogs)
			if (child.GlobalId.Equals(blog.GlobalId, StringComparison.CurrentCultureIgnoreCase))
				child.NumberNotRead = numberNotRead;
	}

	/// <summary>
	///		Número de elementos no leídos
	/// </summary>
	public int NumberNotRead { get; set; }

	/// <summary>
	///		Carpeta padre
	/// </summary>
	public FolderModel? Parent { get; set; }

	/// <summary>
	///		Carpetas contenidas en esta carpeta
	/// </summary>
	public FoldersModelCollection Folders { get; } = new();

	/// <summary>
	///		Blogs de esta carpeta
	/// </summary>
	public BlogsModelCollection Blogs { get; } = new();

	/// <summary>
	///		Hipervínculos de esta carpeta
	/// </summary>
	public HyperlinkModelCollection Hyperlinks { get; } = [];

	/// <summary>
	///		Nombre completo de la carpeta
	/// </summary>
	public string FullName
	{
		get
		{
			string fullName = Name;

				// Recoge el nombre de la carpeta padre
				if (Parent != null && !Parent.Name.IsEmpty())
					fullName = Parent.FullName + "\\" + fullName;
				// Devuelve el nombre completo
				return fullName;
		}
	}
}
