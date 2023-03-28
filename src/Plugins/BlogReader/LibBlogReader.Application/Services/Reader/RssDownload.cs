using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibFeeds.Syndication.Atom.Data;
using Bau.Libraries.LibFeeds.Process;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.Application.Services.Reader
{
	/// <summary>
	///		Clase de descarga de RSS
	/// </summary>
	public class RssDownload
	{ 
		// Eventos públicos
		public event EventHandler<EventArguments.DownloadEventArgs> Process;
		public event EventHandler<EventArguments.BlogEntryDownloadEventArgs> BlogEntryDownload;
		// Variables privadas
		private BlogReaderManager _blogManager;

		public RssDownload(BlogReaderManager blogManager)
		{
			_blogManager = blogManager;
		}

		/// <summary>
		///		Descarga un blog
		/// </summary>
		public void Download(bool includeDisabled, BlogModel blog)
		{
			Download(includeDisabled, new BlogsModelCollection { blog });
		}

		/// <summary>
		///		Descarga los blogs
		/// </summary>
		public void Download(bool includeDisabled, BlogsModelCollection blogs = null)
		{
			System.Threading.Tasks.Task.Run(async () => await DownloadProcessAsync(includeDisabled, blogs));
		}

		/// <summary>
		///		Descarg los blogs
		/// </summary>
		private async System.Threading.Tasks.Task DownloadProcessAsync(bool includeDisabled, BlogsModelCollection blogs = null)
		{
			FeedProcessor processor = new FeedProcessor();
			EntriesModelCollection entriesForDownload = new EntriesModelCollection();

				// Lanza el evento de inicio
				RaiseEvent(EventArguments.DownloadEventArgs.ActionType.StartDownload, "Start download process");
				// Crea la colección de blogs si estaba vacía
				if (blogs == null)
					blogs = _blogManager.File.GetBlogsRecursive();
				// Descarga los blogs
				foreach (BlogModel blog in blogs)
					if (blog.Enabled || (includeDisabled && !blog.Enabled))
					{
						AtomChannel atom;

							// Lanza el evento
							RaiseEvent(EventArguments.DownloadEventArgs.ActionType.StartDownloadBlog, $"Start download {blog.Name}");
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
										}
								}
								// Modifica la fecha de última descarga y el número de entradas no leidas
								blog.DateLastDownload = DateTime.Now;
								// Lanza el evento
								RaiseEvent(EventArguments.DownloadEventArgs.ActionType.EndDownloadBlog, $"End download {blog.Name}");
							}
							catch (Exception exception)
							{
								RaiseEvent(EventArguments.DownloadEventArgs.ActionType.ErrorDonwloadBlog, $"Error when download {blog.Name}. {exception.Message}");
							}
					}
				// Graba la estructura de los blogs
				_blogManager.Save();
				// Descarga los adjuntos
				if (entriesForDownload.Count > 0)
					await DownloadAttachmentsAsync(entriesForDownload);
				// Lanza el evento de fin
				RaiseEvent(EventArguments.DownloadEventArgs.ActionType.EndDownload, "End download blogs");
		}

		/// <summary>
		///		Añade los mensajes a las cuentas
		/// </summary>
		private EntriesModelCollection AddMessages(BlogModel blog, AtomChannel channel)
		{
			EntriesModelCollection existingEntries = _blogManager.LoadEntries(blog);
			EntriesModelCollection downloadedEntries = new();

				// Graba las entradas nuevas
				foreach (AtomEntry entry in channel.Entries)
					if (entry.Links != null && !existingEntries.ExistsURL(GetUrlAlternate(entry.Links)))
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
								blogEntry.Author = entry.Authors[0].Name;
							blogEntry.DatePublish = entry.DatePublished;
							blogEntry.Status = EntryModel.StatusEntry.NotRead;
							// Lanza el evento de descarga de una entrada
							RaiseEvent(blogEntry);
							// Añade la entrada al blog y a la lista de elementos descargados
							downloadedEntries.Add(blogEntry);
					}
				// Si se ha añadido algo, graba las entradas
				if (downloadedEntries.Count > 0)
				{
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
		private string GetUrlAttachment(AtomLinksCollection links)
		{
			return GetUrl(links, AtomLink.AtomLinkType.Enclosure);
		}

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
					fileName = System.IO.Path.Combine(blog.Path, System.IO.Path.GetFileName(url));
					// Ajusta el nombre de archivo
					fileName = LibHelper.Files.HelperFiles.GetConsecutiveFileName(blog.Path, System.IO.Path.GetFileName(fileName));
				}
				// Devuelve el nombre de archivo
				return fileName;
		}

		/// <summary>
		///		Descarga los adjuntos
		/// </summary>
		private async System.Threading.Tasks.Task DownloadAttachmentsAsync(EntriesModelCollection entries)
		{
			var webClient = new LibHelper.Communications.HttpWebClient();

				foreach (EntryModel entry in entries)
					if (!string.IsNullOrEmpty(entry.DownloadFileName))
					{
						string fileName = System.IO.Path.Combine(_blogManager.Configuration.PathBlogs, entry.DownloadFileName);

							// Descarga el archivo
							if (!System.IO.File.Exists(fileName))
							{
								RaiseEvent(EventArguments.DownloadEventArgs.ActionType.StartDownload, $"Descargando el archivo adjunto de {entry.Description}");
								await webClient.DownloadFileAsync(entry.UrlEnclosure, fileName);
							}
					}
		}

		/// <summary>
		///		Lanza el evento de descarga de una entrada de un blog
		/// </summary>
		private void RaiseEvent(EntryModel blogEntry)
		{
			BlogEntryDownload?.Invoke(this, new EventArguments.BlogEntryDownloadEventArgs(blogEntry));
		}

		/// <summary>
		///		Lanza un evento de proceso
		/// </summary>
		private void RaiseEvent(EventArguments.DownloadEventArgs.ActionType action, string description)
		{
			Process?.Invoke(this, new EventArguments.DownloadEventArgs(action, description));
		}
	}
}