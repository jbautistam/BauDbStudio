using Microsoft.Extensions.Logging;

using Bau.Libraries.LibFeeds.Syndication.Atom.Data;
using Bau.Libraries.LibFeeds.Process;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.Application.Services.Reader;

/// <summary>
///		Clase de descarga de RSS
/// </summary>
public class RssDownload
{ 
	// Variables privadas
	private BlogReaderManager _blogManager;

	public RssDownload(BlogReaderManager blogManager)
	{
		_blogManager = blogManager;
	}

	/// <summary>
	///		Descarga un blog
	/// </summary>
	public async Task<int> DownloadAsync(bool includeDisabled, BlogModel blog, CancellationToken cancellationToken)
	{
		return await DownloadAsync(includeDisabled, [ blog ], cancellationToken);
	}

	/// <summary>
	///		Descarga los blogs
	/// </summary>
	public async Task<int> DownloadAsync(bool includeDisabled, BlogsModelCollection? blogs, CancellationToken cancellationToken)
	{
		FeedProcessor processor = new();
		EntriesModelCollection entriesForDownload = new();
		int dowloadedItems = 0;

			// Log
			_blogManager.Logger.LogInformation("Start download process");
			// Crea la colección de blogs si estaba vacía
			if (blogs is null)
				blogs = _blogManager.File.GetBlogsRecursive();
			// Descarga los blogs
			foreach (BlogModel blog in blogs)
				if (blog.Enabled || (includeDisabled && !blog.Enabled) && !cancellationToken.IsCancellationRequested)
				{
					AtomChannel? atom;

						// Log
						_blogManager.Logger.LogInformation($"Start download {blog.Name}");
						// Descarga el archivo
						try
						{ 
							// Descarga el archivo Atom / Rss						
							atom = await processor.DownloadAsync(blog.URL);
							// Añade los mensajes
							if (atom is not null)
							{
								EntriesModelCollection downloaded = AddMessages(blog, atom);

									// Añade los elementos descargado a las entradas que pueden descargar adjuntos
									if (downloaded.Count > 0)
									{
										// Añade el número de entradas no leidas
										blog.NumberNotRead += downloaded.Count;
										// Añade las entradas a la descarga pendiente de adjuntos
										if (blog.DownloadPodcast)
											entriesForDownload.AddRange(downloaded);
										// Añade el número de elementos descargados
										dowloadedItems += downloaded.Count;
										// Obtiene la fecha de última entrada
										blog.DateLastEntry = entriesForDownload.GetDateLastEntry();
									}
							}
							// Log
							_blogManager.Logger.LogInformation($"End download {blog.Name}");
						}
						catch (Exception exception)
						{
							_blogManager.Logger.LogError(exception, $"Error when download {blog.Name}. {exception.Message}");
						}
				}
			// Graba la estructura de los blogs
			_blogManager.Save();
			// Descarga los adjuntos
			if (entriesForDownload.Count > 0)
				await DownloadAttachmentsAsync(entriesForDownload, cancellationToken);
			// Log
			_blogManager.Logger.LogInformation("End download blogs");
			// Devuelve el número de elementos descargados
			return dowloadedItems;
	}

	/// <summary>
	///		Añade los mensajes a las cuentas
	/// </summary>
	private EntriesModelCollection AddMessages(BlogModel blog, AtomChannel channel)
	{
		EntriesModelCollection existingEntries = _blogManager.LoadEntries(blog);
		EntriesModelCollection downloadedEntries = [];
		DateTime previousDate = existingEntries.GetDateLastEntry();

			// Graba las entradas nuevas
			foreach (AtomEntry entry in channel.Entries)
				if (entry.Links is not null && !existingEntries.ExistsURL(GetUrlAlternate(entry.Links)) && entry.GetFirstDate() >= previousDate)
				{
					EntryModel blogEntry = new();

						// Asigna el blog
						blogEntry.Blog = blog;
						// Asigna los datos a la entrada
						blogEntry.Name = entry.Title.Content;
						blogEntry.Content = entry.Content.Content;
						blogEntry.URL = GetUrlAlternate(entry.Links);
						blogEntry.UrlEnclosure = GetUrlAttachment(entry.Links);
						blogEntry.DownloadFileName = GetDownloadFileName(blog, blogEntry.UrlEnclosure);
						if (entry.Authors != null && entry.Authors.Count > 0)
							blogEntry.Author = entry.Authors[0].Name ?? string.Empty;
						blogEntry.DatePublish = entry.DatePublished;
						blogEntry.Status = EntryModel.StatusEntry.NotRead;
						// Añade la entrada al blog y a la lista de elementos descargados
						downloadedEntries.Add(blogEntry);
				}
			// Si se ha añadido algo, graba las entradas
			if (downloadedEntries.Count > 0)
			{
				// Modifica la fecha de última descarga
				blog.DateLastDownload = DateTime.UtcNow;
				// Añade las entradas descargadas a las existentes
				existingEntries.AddRange(downloadedEntries);
				// Graba las entradas
				_blogManager.SaveBlog(blog, existingEntries);
			}
			// Devuelve los elementos descargados
			return downloadedEntries;
	}

	/// <summary>
	///		Obtiene la URL alternativa
	/// </summary>
	private string GetUrlAlternate(AtomLinksCollection links)
	{
		string url = GetUrl(links, AtomLink.AtomLinkType.Alternate);

			// Si no ha encontrado una URL alternativa, recoge la primera
			if (string.IsNullOrWhiteSpace(url) && links.Count > 0)
				url = links[0].Href;
			// Devuelve la URL localizada
			return url;
	}

	/// <summary>
	///		Obtiene la URL del adjunto
	/// </summary>
	private string GetUrlAttachment(AtomLinksCollection links) => GetUrl(links, AtomLink.AtomLinkType.Enclosure);

	/// <summary>
	///		Obtiene la URL de un tipo determinado
	/// </summary>
	private string GetUrl(AtomLinksCollection links, AtomLink.AtomLinkType type)
	{
		// Busca la URL del adjunto
		foreach (AtomLink link in links)
			if (link.LinkType == type)
				return link.Href;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return string.Empty;
	}

	/// <summary>
	///		Obtiene el nombre del archivo de descarga
	/// </summary>
	private string GetDownloadFileName(BlogModel blog, string url)
	{
		string fileName = string.Empty;

			// Calcula el nombre del archivo
			if (!string.IsNullOrEmpty(url))
			{
				// Obtiene el nombre de archivo
				fileName = Path.Combine(blog.Path, Path.GetFileName(url));
				// Ajusta el nombre de archivo
				fileName = LibHelper.Files.HelperFiles.GetConsecutiveFileName(blog.Path, Path.GetFileName(fileName));
			}
			// Devuelve el nombre de archivo
			return fileName;
	}

	/// <summary>
	///		Descarga los adjuntos
	/// </summary>
	private async Task DownloadAttachmentsAsync(EntriesModelCollection entries, CancellationToken cancellationToken)
	{
		LibHelper.Communications.HttpWebClient webClient = new();

			foreach (EntryModel entry in entries)
				if (!string.IsNullOrEmpty(entry.DownloadFileName) && !cancellationToken.IsCancellationRequested)
				{
					string fileName = Path.Combine(_blogManager.Configuration.PathBlogs, entry.DownloadFileName);

						// Descarga el archivo
						if (!File.Exists(fileName) && !string.IsNullOrWhiteSpace(entry.UrlEnclosure))
						{
							_blogManager.Logger.LogInformation($"Donwloaded attachment file for {entry.Description}");
							await webClient.DownloadFileAsync(entry.UrlEnclosure, fileName);
						}
				}
	}
}