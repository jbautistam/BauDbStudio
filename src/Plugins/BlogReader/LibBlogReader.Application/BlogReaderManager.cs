using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.Application;

/// <summary>
///		Archivo de BlogReader
/// </summary>
public class BlogReaderManager
{   
	// Variables privadas
	private Controllers.Configuration? _configuration;

	public BlogReaderManager(ILogger logger)
	{
		File = new FolderModel();
		Logger = logger;
	}

	/// <summary>
	///		Carga los archivos
	/// </summary>
	public void Load()
	{
		if (!Configuration.PathBlogs.IsEmpty() && Directory.Exists(Configuration.PathBlogs))
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
	public EntriesModelCollection LoadEntries(BlogModel blog) => new Repository.EntryXmlRepository().Load(blog, Configuration.PathBlogs);

	/// <summary>
	///		Guarda los datos de un blog
	/// </summary>
	public void SaveBlog(BlogModel blog, EntriesModelCollection entries)
	{
		// Borra las entradas antiguas
		if (blog.DeleteOldEntries)
			DeleteOldEntries(blog, entries);
		// Graba las entradas
		new Repository.EntryXmlRepository().Save(blog, entries, Configuration.PathBlogs);
	}

	/// <summary>
	///		Borra las entradas antiguas
	/// </summary>
	private void DeleteOldEntries(BlogModel blog, EntriesModelCollection entries)
	{
		int entriesRead = entries.GetNumberReadAndDeleted();

			// Ordena las entradas por fecha
			entries.SortByDate();
			// Borra las últimas X entradas hasta tener sólo las necesarias
			for (int index = entries.Count - 1; index >= 0; index--)
				if ((entries[index].Status == EntryModel.StatusEntry.Read || entries[index].Status == EntryModel.StatusEntry.Deleted) &&
					entriesRead > blog.MaximumEntriesRead && entries[index].DatePublish < DateTime.Now.AddDays(-15))
				{
					// Borra la entrada
					entries.RemoveAt(index);
					// y marca el número de elementos que siguen en la colección
					entriesRead--;
				}
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

	/// <summary>
	///		Logger
	/// </summary>
	public ILogger Logger { get; }
}
