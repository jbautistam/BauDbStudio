using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.Application
{
	/// <summary>
	///		Archivo de BlogReader
	/// </summary>
	public class BlogReaderManager
	{   
		// Variables privadas
		private Controllers.Configuration _configuration;

		public BlogReaderManager()
		{
			File = new FolderModel();
		}

		/// <summary>
		///		Carga los archivos
		/// </summary>
		public void Load()
		{
			if (!Configuration.PathBlogs.IsEmpty() && System.IO.Directory.Exists(Configuration.PathBlogs))
				File = new Repository.BlogsStructureRepository().Load(Configuration.PathBlogs);
		}

		/// <summary>
		///		Graba el árbol de blogs
		/// </summary>
		public void Save()
		{
			new Repository.BlogsStructureRepository().Save(File, Configuration.PathBlogs);
		}

		/// <summary>
		///		Carga un archivo OPML
		/// </summary>
		public void LoadOpml(string fileName)
		{
			new Services.Opml.OpmlServices().Load(this, fileName);
		}

		/// <summary>
		///		Graba un archivo OPML
		/// </summary>
		public void SaveOpml(string fileName)
		{
			new Services.Opml.OpmlServices().Save(this, fileName);
		}

		/// <summary>
		///		Carga las entradas de un blog
		/// </summary>
		public EntriesModelCollection LoadEntries(BlogModel blog)
		{
			return new Repository.EntryXmlRepository().Load(blog, Configuration.PathBlogs);
		}

		/// <summary>
		///		Guarda los datos de un blog
		/// </summary>
		public void SaveBlog(BlogModel blog, EntriesModelCollection entries)
		{
			new Repository.EntryXmlRepository().Save(blog, entries, Configuration.PathBlogs);
		}

		/// <summary>
		///		Archivo con los elementos del lector de blogs
		/// </summary>
		public FolderModel File { get; private set; }

		/// <summary>
		///		Configuración del procesador
		/// </summary>
		public Controllers.Configuration Configuration
		{
			get
			{ 
				// Crea el objeto de configuración si no estaba en memoria
				if (_configuration is null)
					_configuration = new Controllers.Configuration();
				// Devuelve el objeto de configuración
				return _configuration;
			}
		}
	}
}
