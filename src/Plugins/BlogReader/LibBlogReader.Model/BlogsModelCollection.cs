
namespace Bau.Libraries.LibBlogReader.Model;

/// <summary>
///		Colección de <see cref="BlogModel"/>
/// </summary>
public class BlogsModelCollection : LibDataStructures.Base.BaseExtendedModelCollection<BlogModel>
{
	/// <summary>
	///		Añade un <see cref="BlogModel"/> a la colección
	/// </summary>
	public BlogModel Add(string name, string description, string url)
	{
		BlogModel blog = new() 
							{ 
								Name = name, 
								Description = description, 
								URL = url 
							};

			// Añade el blog a la colección
			Add(blog);
			// Devuelve el blog
			return blog;
	}

	/// <summary>
	///		Cuenta los <see cref="BlogModel"/> activos
	/// </summary>
	public int CountEnabled()
	{
		int count = 0;

			// Cuenta el número de elementos
			foreach (BlogModel blog in this)
				if (blog.Enabled)
					count++;
			// Devuelve el número de elementos
			return count;
	}

	/// <summary>
	///		Obtiene el número de elementos no leídos
	/// </summary>
	public int GetNumberNotRead()
	{
		int number = 0;

			// Obtiene el número de elementos no leídos
			foreach (BlogModel blog in this)
				if (blog.Enabled)
					number += blog.NumberNotRead;
			// Devuelve el número de elementos no leídos
			return number;
	}
}
