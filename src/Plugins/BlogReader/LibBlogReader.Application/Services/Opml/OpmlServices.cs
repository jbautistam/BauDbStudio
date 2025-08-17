using Bau.Libraries.LibFeeds.Syndication.OPML.Data;
using Bau.Libraries.LibFeeds.Syndication.OPML.Transforms;

namespace Bau.Libraries.LibBlogReader.Application.Services.Opml;

/// <summary>
///		Servicios sobre OPML
/// </summary>
internal class OpmlServices
{
	/// <summary>
	///		Carga un archivo OPML sobre la carpeta actual
	/// </summary>
	internal void Load(BlogReaderManager manager, string fileName)
	{
		if (File.Exists(fileName))
			try
			{
				OPMLChannel? channel = new OPMLParser().Parse(fileName);

					if (channel is not null)
					{
						string? title = channel.Title;

							// Obtiene el título de la carpeta
							if (string.IsNullOrWhiteSpace(title))
								title = "Opml";
							// Añade las entradas
							if (channel.Entries.Count > 0)
								AddEntries(manager.File.Folders.Add(title), channel.Entries);
					}
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine("Exception when load file: " + exception.Message);
			}
	}

	/// <summary>
	///		Añade las entradas de un archivo OPML a una carpeta
	/// </summary>
	private void AddEntries(Model.FolderModel folder, OPMLEntriesCollection entries)
	{
		foreach (OPMLEntry entry in entries)
			if (entry.Entries.Count == 0 && !string.IsNullOrWhiteSpace(entry.URL))
				folder.Blogs.Add(entry.Text, entry.Title, entry.URL);
			else if (entry.Entries.Count > 0)
				AddEntries(folder.Folders.Add(entry.Text ?? "Folder"), entry.Entries);
	}

	/// <summary>
	///		Graba un archivo OPML con los datos actuales
	/// </summary>
	internal void Save(BlogReaderManager manager, string fileName)
	{
		OPMLChannel channel = new OPMLChannel();

			// Asigna las propiedades
			channel.Title = "Archivo creado con Bau Studio";
			// Añade las carpetas
			foreach (Model.FolderModel folder in manager.File.Folders)
				AddFolder(folder, channel.Entries);
			// Añade los blogs
			foreach (Model.BlogModel blog in manager.File.Blogs)
				AddBlog(blog, channel.Entries);
			// Graba el archivo
			LibHelper.Files.HelperFiles.MakePath(Path.GetDirectoryName(fileName)!);
			new OPMLWriter().Save(channel, fileName);
	}

	/// <summary>
	///		Añade una carpeta a la colección de entrada
	/// </summary>
	private void AddFolder(Model.FolderModel folder, OPMLEntriesCollection entries)
	{
		OPMLEntry entry = CreateEntry(folder.Name, "Folder", null, null);

			// Añade la entrada a la colección
			entries.Add(entry);
			// Añade las carpetas
			foreach (Model.FolderModel child in folder.Folders)
				AddFolder(child, entry.Entries);
			// Añade los blogs
			foreach (Model.BlogModel blog in folder.Blogs)
				AddBlog(blog, entry.Entries);
	}

	/// <summary>
	///		Añade un blog a la colección de entradas
	/// </summary>
	private void AddBlog(Model.BlogModel blog, OPMLEntriesCollection entries)
	{
		entries.Add(CreateEntry(blog.Name, "Blog", blog.Description, blog.URL));
	}

	/// <summary>
	///		Crea una entrada
	/// </summary>
	private OPMLEntry CreateEntry(string name, string? type, string? description, string? url)
	{
		return new OPMLEntry()
					{
						Text = name,
						Type = type,
						Title = description,
						URL = url
					};
	}
}
