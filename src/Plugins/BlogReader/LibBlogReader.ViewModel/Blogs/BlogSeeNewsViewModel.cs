using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.LibBlogReader.ViewModel.Blogs;

/// <summary>
///		ViewModel para ver las noticias de <see cref="BlogModel"/>
/// </summary>
public class BlogSeeNewsViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Enumerados privadas
	/// <summary>
	///		Acciones que se pueden realizar con una entrada
	/// </summary>
	private enum EntryAction
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Marcar la entrada como leida / no leida</summary>
		MarkRead,
		/// <summary>Marcar la entrada como interesante</summary>
		MarkInteresting,
		/// <summary>Borrar la entrada</summary>
		Delete,
		/// <summary>Reproducir el adjunto</summary>
		Play
	}
	// Eventos públicos
	public event EventHandler? Closed;
	// Variables privadas
	private EntriesModelCollection _blogEntries = new();
	private BlogEntriesCollectionViewModel _entries = default!, _entriesForView = default!;
	private BlogEntryViewModel? _selectedEntry;
	private bool _mustSeeInteresting, _mustSeeNotRead, _mustSeeRead, _isDirty;
	private int _entriesNumber, _recordsPerPage, _actualPage, _pageIndex;
	private string _htmlNews = default!, _tabId = default!;

	public BlogSeeNewsViewModel(BlogReaderViewModel mainViewModel, BlogsModelCollection blogs) : base(false)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		Blogs = blogs;
		// Inicializa el ViewModel
		InitViewModel();
		// Inicializa los comandos de página
		FirstPageCommand = CreateCommandForPage(nameof(FirstPageCommand));
		NextPageCommand = CreateCommandForPage(nameof(NextPageCommand));
		PreviousPageCommand = CreateCommandForPage(nameof(PreviousPageCommand));
		LastPageCommand = CreateCommandForPage(nameof(LastPageCommand));
		// Inicializa los comandos del menú
		MarkAsReadCommand = new BaseCommand(_ => ExecuteAction(nameof(MarkAsReadCommand)),
											_ => CanExecuteAction(nameof(MarkAsReadCommand)));
		MarkAsNotReadCommand = new BaseCommand(_ => ExecuteAction(nameof(MarkAsNotReadCommand)),
											   _ => CanExecuteAction(nameof(MarkAsNotReadCommand)));
		MarkAsInterestingCommand = new BaseCommand(_ => ExecuteAction(nameof(MarkAsInterestingCommand)),
												   _ => CanExecuteAction(nameof(MarkAsInterestingCommand)));
		ExportEntriesCommand = new BaseCommand(_ => ExportEntries());
		RefreshCommand = new BaseCommand(_ => Refresh());
		PlayCommand = new BaseCommand(_ => PlayEntry())
								.AddListener(this, nameof(SelectedEntry));
		DeleteCommand = new BaseCommand(_ => ExecuteAction(nameof(DeleteCommand)),
										_ => CanExecuteAction(nameof(DeleteCommand)))
								.AddListener(this, nameof(SelectedEntry));
		// Inicializa las propiedades
		PropertyChanged += (sender, args) =>
								{
									if (args is not null && args.PropertyName!.EqualsIgnoreCase(nameof(SelectedEntry)))
										DeleteCommand.OnCanExecuteChanged();
									IsUpdated = false;
								};
	}

	/// <summary>
	///		Inicializa el ViewModel: propiedades, menús y comandos
	/// </summary>
	private void InitViewModel()
	{
		// Guarda el Id del documento
		TabId = $"{GetType().ToString()}_{GetBlogSeeNewsTabId()}";
		// Obtiene los datos de configuración (asigna las variables, no las propiedades porque las propiedades lanzan TreatSelectedEntriesChanged)
		_mustSeeRead = MainViewModel.ConfigurationViewModel.SeeEntriesRead;
		_mustSeeNotRead = MainViewModel.ConfigurationViewModel.SeeEntriesNotRead;
		_mustSeeInteresting = MainViewModel.ConfigurationViewModel.SeeEntriesInteresting;
		// Carga las entradas de los blogs
		LoadBlogsEntries();
		// Carga las noticias
		TreatSelectedEntriesChanged();
	}

	/// <summary>
	///		Carga todos los blogs
	/// </summary>
	private void LoadBlogsEntries()
	{
		// Limpia la lista de entrada
		_blogEntries.Clear();
		// Carga las entradas de los blogs seleccionados
		foreach (BlogModel blog in Blogs)
			if (blog.Enabled || MainViewModel.ConfigurationViewModel.ShowNewsDisabledBlogs)
				try
				{
					_blogEntries.AddRange(MainViewModel.BlogManager.LoadEntries(blog));
				}
				catch (Exception exception)
				{
					MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load blog {blog.Name}. {exception.Message}");
				}
		// Ordena las entradas
		_blogEntries.Sort((first, second) => {
												if (first.Blog.Name.Equals(second.Blog.Name, StringComparison.CurrentCultureIgnoreCase))
													return -1 * first.DatePublish.CompareTo(second.DatePublish);
												else
													return first.Blog.Name.CompareTo(second.Blog.Name);
											 }
						 );
		// -1 * first.DatePublish.CompareTo(second.DatePublish));
		// Carga las entradas en las entradas de la lista
		EntriesList = new BlogEntriesCollectionViewModel();
		foreach (EntryModel entry in _blogEntries)
			if (entry.Status != EntryModel.StatusEntry.Deleted)
				EntriesList.Add(new BlogEntryViewModel(entry.Blog.Name, entry));

	}

	/// <summary>
	///		Obtiene la etiqueta para la colección de blogs
	/// </summary>
	private string GetBlogSeeNewsTabId()
	{
		string tabId = string.Empty;

			// Crea la Id de la ventana
			foreach (BlogModel blog in Blogs)
				tabId += blog.URL + ";";
			// Devuelve el Id
			return tabId;
	}

	/// <summary>
	///		Crea un comando asociado a la página HTML
	/// </summary>
	private BaseCommand CreateCommandForPage(string action)
	{
		return new BaseCommand(_ => ExecuteAction(action), _ => CanExecuteAction(action))
						.AddListener(this, nameof(Pages)).AddListener(this, nameof(ActualPage)).AddListener(this, nameof(RecordsPerPage));
	}

	/// <summary>
	///		Ejecuta una acción
	/// </summary>
	private void ExecuteAction(string action)
	{
		switch (action)
		{
			case nameof(MarkAsReadCommand):
					SetStatusEntriesSelected(EntryModel.StatusEntry.Read);
				break;
			case nameof(MarkAsNotReadCommand):
					SetStatusEntriesSelected(EntryModel.StatusEntry.NotRead);
				break;
			case nameof(MarkAsInterestingCommand):
					SetStatusEntriesSelected(EntryModel.StatusEntry.Interesting);
				break;
			case nameof(FirstPageCommand):
					ActualPage = 1;
					TreatSelectedEntriesChanged();
				break;
			case nameof(PreviousPageCommand):
					ActualPage--;
					TreatSelectedEntriesChanged();
				break;
			case nameof(LastPageCommand):
					ActualPage = Pages;
					TreatSelectedEntriesChanged();
				break;
			case nameof(NextPageCommand):
					ActualPage++;
					TreatSelectedEntriesChanged();
				break;
			case nameof(RefreshCommand):
					Refresh();
				break;
			case nameof(DeleteCommand):
					DeleteSelectedEntries();
				break;
		}
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	private bool CanExecuteAction(string action)
	{
		switch (action)
		{
			case nameof(FirstPageCommand):
			case nameof(PreviousPageCommand):
				return ActualPage > 1;
			case nameof(LastPageCommand):
			case nameof(NextPageCommand):
				return ActualPage < Pages;
			case nameof(DeleteCommand):
				return SelectedEntry != null || (SelectedEntry == null && Blogs.GetNumberNotRead() == 0 && EntriesList.Count > 0);
			case nameof(MarkAsReadCommand):
			case nameof(MarkAsNotReadCommand):
			case nameof(MarkAsInterestingCommand):
				return SelectedEntry != null;
			default:
				return false;
		}
	}

	/// <summary>
	///		Actualiza la vista
	/// </summary>
	private void Refresh()
	{
		SelectedEntry = null;
		MarkAsDirty();
	}

	/// <summary>
	///		Obtiene las entradas de los blogs
	/// </summary>
	private BlogEntriesCollectionViewModel GetEntries()
	{
		BlogEntriesCollectionViewModel entriesViewModel = new BlogEntriesCollectionViewModel();

			// Ordena los blogs
			Blogs.SortByName();
			// Asigna las entradas
			foreach (BlogModel blog in Blogs)
			{ 
				EntriesModelCollection entries = MainViewModel.BlogManager.LoadEntries(blog);

					// Ordena las entradas por fecha
					entries.SortByDate(false);
					// Añade las entradas a la vista (no hace un foreach porque puede que se cambie la colección en las descargas)
					for (int index = 0; index < entries.Count; index++)
						if (entries[index].Status != EntryModel.StatusEntry.Deleted)
							entriesViewModel.Add(new BlogEntryViewModel(blog.Name, entries[index]));
			}
			// Devuelve las entradas
			return entriesViewModel;
	}

	/// <summary>
	///		Obtiene las entradas seleccionadas
	/// </summary>
	private BlogEntriesCollectionViewModel GetEntriesSelected()
	{
		if (EntriesList.CountSelected > 0)
			return EntriesList.GetSelected();
		else
			return EntriesList.GetByParameters(SeeNotRead, SeeRead, SeeInteresting);
	}

	/// <summary>
	///		Borra las entradas seleccionadas
	/// </summary>
	private void DeleteSelectedEntries()
	{
		BlogEntriesCollectionViewModel entriesForDelete = EntriesList.GetSelected();

			// Si no se ha seleccionado ninguna, obtiene las no leídas para borrarlas
			if (entriesForDelete.Count == 0)
				entriesForDelete = EntriesList.GetRead();
			// Si hay algo que se pueda borrar ...
			if (entriesForDelete.Count > 0 &&
					MainViewModel.ViewsController.SystemController.ShowQuestion("¿Realmente desea eliminar las entradas seleccionadas?"))
				DeleteEntries(entriesForDelete);
	}

	/// <summary>
	///		Borra una serie de entradas
	/// </summary>
	private void DeleteEntries(BlogEntriesCollectionViewModel entriesForDelete)
	{
		if (entriesForDelete.Count > 0)
		{
			BlogsModelCollection blogsUpdated = new BlogsModelCollection();
			bool deleted = false;

				// Borra las entradas
				foreach (BlogEntryViewModel entryViewModel in entriesForDelete)
				{ 
					// Añade el blog de la entrada
					if (!blogsUpdated.Exists(entryViewModel.Entry.Blog.GlobalId))
						blogsUpdated.Add(entryViewModel.Entry.Blog);
					// Marca como borrado la entrada del blog
					_blogEntries.UpdateStatus(entryViewModel.Entry.Blog, entryViewModel.Entry.GlobalId, EntryModel.StatusEntry.Deleted);
					// Elimina los archivos asociados
					if (!string.IsNullOrEmpty(entryViewModel.Entry.DownloadFileName) &&
							System.IO.File.Exists(entryViewModel.Entry.DownloadFileName))
						LibHelper.Files.HelperFiles.KillFile(System.IO.Path.Combine(MainViewModel.ConfigurationViewModel.PathBlogs, entryViewModel.Entry.DownloadFileName));
					// Indica que algo se ha borrado
					deleted = true;
				}
				// Si realmente hay algo que borrar
				if (deleted)
				{
					// Graba los archivos
					TreatUpdateEntries(blogsUpdated);
					// Elimina las entradas de la lista
					foreach (BlogEntryViewModel entryViewModel in entriesForDelete)
						EntriesList.Remove(entryViewModel);
				}
		}
	}

	/// <summary>
	///		Obtiene el HTML de las entradas seleccionadas para mostrarlas en el explorador
	/// </summary>
	private string GetHtmlNews()
	{
		BlogEntriesCollectionViewModel entries = LastEntriesSelectedForView;
		System.Text.StringBuilder sbHtml = new System.Text.StringBuilder();
		string lastBlog = null;
		int startIndex, endIndex;
		bool addedEntry = false;

			// Actualiza el número de entradas y de páginas
			EntriesNumber = entries.Count;
			if (RecordsPerPage == 0)
				RecordsPerPage = 25;
			Pages = EntriesNumber / RecordsPerPage;
			if (EntriesNumber % RecordsPerPage != 0)
				Pages++;
			// Normaliza la página actual
			if (ActualPage > Pages)
				ActualPage = Pages;
			else if (ActualPage < 1)
				ActualPage = 1;
			// Asigna el índice de inicio y fin
			startIndex = (ActualPage - 1) * RecordsPerPage;
			endIndex = startIndex + RecordsPerPage;
			// Cabecera HTML
			sbHtml.AppendLine("<html><head>");
			sbHtml.AppendLine("<meta content='text/html; charset=utf-8' http-equiv='Content-Type'>");
			sbHtml.AppendLine("<meta name = 'Content-Type' content = 'Content-Type: text/html; charset=utf-8'>");
			sbHtml.AppendLine("<meta name='lang' content='es'>");
			sbHtml.AppendLine("</head><body>");
			// Obtiene el HTML
			for (int index = startIndex; index < endIndex; index++)
				if (index >= 0 && index < entries.Count)
				{
					BlogEntryViewModel entry = entries[index];

						// Título del blog
						if (!lastBlog.EqualsIgnoreCase(entry.BlogName))
						{ 
							// Cabecera de blog
							sbHtml.AppendLine($"<h1 style='clear:both;'>{entry.BlogName}</h1>");
							// Guarda la última cabecera de blog
							lastBlog = entry.BlogName;
						}
						// Cabecera de entrada
						sbHtml.AppendLine($"<h2 style='clear:both;'><a href='{entry.URL}'>{entry.Title}</a></h2>");
						// Comandos sobre la entrada
						sbHtml.AppendLine("<p>");
						if (entry.Entry.Status == EntryModel.StatusEntry.NotRead)
							sbHtml.AppendLine(GetJavaScriptExternalFuction(entry, "Marcar leído", EntryAction.MarkRead));
						if (entry.Entry.Status == EntryModel.StatusEntry.Read)
							sbHtml.AppendLine(GetJavaScriptExternalFuction(entry, "Marcar no leído", EntryAction.MarkRead));
						if (entry.Entry.Status == EntryModel.StatusEntry.Read || entry.Entry.Status == EntryModel.StatusEntry.NotRead)
							sbHtml.AppendLine(GetJavaScriptExternalFuction(entry, "Marcar interesante", EntryAction.MarkInteresting));
						sbHtml.AppendLine(GetJavaScriptExternalFuction(entry, "Borrar", EntryAction.Delete));
						sbHtml.AppendLine("</p>");
						// Cuerpo
						sbHtml.AppendLine(RemoveIframe(entry.Content) + "<br>");
						// Cierra las etiquetas que se hayan podido quedar abiertas y provocan que el siguiente artículo se lea en negrita o cursiva
						for (int closeTags = 0; closeTags < 10; closeTags++)
							sbHtml.AppendLine("</b></strong></i></em></h1></h2></h3></h4></h5></img></a></span></div></font></li></ul></ol></blockquote></script>");
						// Adjunto
						if (!string.IsNullOrEmpty(entry.UrlEnclosure))
							sbHtml.AppendLine($"<p><strong>Adjunto: </strong>{GetJavaScriptExternalFuction(entry, System.IO.Path.GetFileName(entry.UrlEnclosure), EntryAction.Play)}</p><br>");
						// Autor - Fecha
						sbHtml.AppendLine($@"<p style='text-align:right;'>
													<strong>Autor: </strong>{entry.Author} - 
													<strong>Fecha: </strong>{entry.DatePublish:dd-MM-yyyy HH:mm}
												</p>");
						// Separador
						sbHtml.AppendLine("<hr>");
						// Indica que algo se ha añadido
						addedEntry = true;
				}
			// Si no se ha añadido nada, se muestra un mensaje
			if (!addedEntry)
				sbHtml.AppendLine("<p>No entries</p>");
			// Cierre HTML
			sbHtml.AppendLine("</body></html>");
			// Devuelve el HTML
			return sbHtml.ToString();
	}

	/// <summary>
	///		Elimina el contenido de las etiquetas iFrame que pueden bloquear el explorador
	/// </summary>
	private string RemoveIframe(string text)
	{
		string result = text;
		int loops = 0;

			// Quita la etiqueta "iframe"
			while (!string.IsNullOrEmpty(result) && result.Contains("<iframe") && result.Contains("</iframe>") && loops++ < 10)
				result = System.Text.RegularExpressions.Regex.Replace(result, "<iframe(.|\n)*?</iframe>", string.Empty, 
																	  System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
																		System.Text.RegularExpressions.RegexOptions.Multiline |
																		System.Text.RegularExpressions.RegexOptions.Compiled);
			// Elimina los iframe que hayan podido quedarse descolgados
			if (!string.IsNullOrWhiteSpace(result))
			{
				result = result.Replace("<iframe", "<span");
				result = result.Replace("</iframe", "</span>");
			}
			// Devuelve el resultado
			return result;
	}


	/// <summary>
	///		Obtiene un vínculo a una llamada de función externa en JavaScript
	/// </summary>
	private string GetJavaScriptExternalFuction(BlogEntryViewModel entry, string title, EntryAction action)
	{
		return $"<a href='' onclick=\"window.chrome.webview.postMessage('{action.ToString()}|{entry.Entry.URL}');return false;\">{title}</a>";
	}

	/// <summary>
	///		Ejecuta las funciones llamadas desde el explorador
	/// </summary>
	public void ExecuteFromExplorer(string arguments)
	{
		if (!arguments.IsEmpty())
		{
			string[] parameters = arguments.Split('|');

				if (parameters.Length == 2)
				{
					BlogEntryViewModel entry = EntriesList.Search(parameters[1]);

						if (entry != null)
						{
							BlogEntriesCollectionViewModel entries = new BlogEntriesCollectionViewModel { entry };

								switch (parameters[0].GetEnum(EntryAction.Unknown))
								{
									case EntryAction.MarkRead:
											if (entry.Entry.Status == EntryModel.StatusEntry.Read)
												SetStatusEntries(entries, EntryModel.StatusEntry.NotRead);
											else
												SetStatusEntries(entries, EntryModel.StatusEntry.Read);
										break;
									case EntryAction.MarkInteresting:
											if (entry.Entry.Status == EntryModel.StatusEntry.Interesting)
												SetStatusEntries(entries, EntryModel.StatusEntry.Read);
											else
												SetStatusEntries(entries, EntryModel.StatusEntry.Interesting);
										break;
									case EntryAction.Delete:
											DeleteEntries(entries);
										break;
									case EntryAction.Play:
											PlayEntry();
										break;
								}
						}
				}
		}
	}

	/// <summary>
	///		Marca los elementos seleccionados como leídos
	/// </summary>
	public void MarkRead()
	{
		SetStatusEntriesSelected(EntryModel.StatusEntry.Read);
	}

	/// <summary>
	///		Modifica el estado de las entradas seleccionadas
	/// </summary>
	private void SetStatusEntriesSelected(EntryModel.StatusEntry idNewStatus)
	{
		SetStatusEntries(LastEntriesSelectedForView, idNewStatus);
	}

	/// <summary>
	///		Modifica el estado de las entradas seleccionadas
	/// </summary>
	private void SetStatusEntries(BlogEntriesCollectionViewModel entries, EntryModel.StatusEntry newStatus)
	{
		BlogsModelCollection blogsUpdated = new BlogsModelCollection();
		bool updated = false;

			// Marca los elementos seleccionados como leídos
			foreach (BlogEntryViewModel entry in entries)
				if (CanChangeStatus(entry, newStatus))
				{ 
					// Cambia el estado
					entry.Status = newStatus;
					// Añade el blog
					if (!blogsUpdated.Exists(entry.Entry.Blog.GlobalId))
						blogsUpdated.Add(entry.Entry.Blog);
					// Modifica la entrada
					_blogEntries.UpdateStatus(entry.Entry.Blog, entry.Entry.GlobalId, newStatus);
					// Indica que se ha modificado
					updated = true;
				}
			// Si realmente se ha marcado alguno ... 
			if (updated)
				TreatUpdateEntries(blogsUpdated);
	}

	/// <summary>
	///		Trata la modificación de las entradas las entradas
	/// </summary>
	private void TreatUpdateEntries(BlogsModelCollection blogsUpdated)
	{
		// Graba los archivos de los blogs modificados
		foreach (BlogModel blog in blogsUpdated)
		{
			EntriesModelCollection entries = _blogEntries.GetFrom(blog);

				// Graba las entradas del blog
				MainViewModel.BlogManager.SaveBlog(blog, entries);
				// Graba el blog
				blog.NumberNotRead = entries.GetNumberNotRead();
				// Actualiza el número de elementos no leidos del manager
				MainViewModel.BlogManager.File.UpdateNumberNotRead(blog, blog.NumberNotRead);
		}
		// Graba el archivo con los blogs y las carpetas
		MainViewModel.BlogManager.Save();
		// Cambia el estado del comando borrar
		DeleteCommand.OnCanExecuteChanged();
		// Actualiza el árbol
		MainViewModel.Load(string.Empty);
	}

	/// <summary>
	///		Comprueba si se puede cambiar el estado de una entrada
	/// </summary>
	private bool CanChangeStatus(BlogEntryViewModel entry, EntryModel.StatusEntry newStatus)
	{
		bool canChange = false;

			// Comprueba si se puede cambiar el estado de una entrada
			if (entry.Status != newStatus)
			{
				// Indica que por ahora se puede cambiar
				canChange = true;
				// Comprueba el nuevo valor
				if (newStatus == EntryModel.StatusEntry.Read && entry.Status == EntryModel.StatusEntry.Interesting)
					canChange = false;
			}
			// Devuelve el valor que indica si se puede cambiar el estado
			return canChange;
	}

	/// <summary>
	///		Marca el modelo como sucio para que se recargue el explorador
	/// </summary>
	public void MarkAsDirty()
	{
		_isDirty = true;
		TreatSelectedEntriesChanged();
	}

	/// <summary>
	///		Trata la modificación en los parámetros de lectura de la lista
	/// </summary>
	private void TreatSelectedEntriesChanged()
	{
		// Se no se ha seleccionado nada, se muestran al menos la no leidas
		if (!SeeRead && !SeeNotRead && !SeeInteresting)
			SeeNotRead = true;
		// Asigna los datos a la configuración
		MainViewModel.ConfigurationViewModel.SeeEntriesRead = SeeRead;
		MainViewModel.ConfigurationViewModel.SeeEntriesNotRead = SeeNotRead;
		MainViewModel.ConfigurationViewModel.SeeEntriesInteresting = SeeInteresting;
		// Graba la configuración
		MainViewModel.ConfigurationViewModel.Save();
		// Lanza el evento de modificación de entradas seleccionadas
		HtmlNews = GetHtmlNews();
	}

	/// <summary>
	///		Exporta las entradas
	/// </summary>
	private void ExportEntries()
	{
		string? fileName = MainViewModel.ViewsController.DialogsController
								.OpenDialogSave(null, "Archivos XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*", "ExportEntries.xml", ".xml");

			if (!string.IsNullOrWhiteSpace(fileName))
			{
				BlogEntriesCollectionViewModel entriesViewModel = LastEntriesSelectedForView;
				BlogModel blog = new();
				EntriesModelCollection entries = new();

					// Crea los datos del blog
					blog.Name = "Export";
					// Añade las entradas
					foreach (BlogEntryViewModel entryViewModel in entriesViewModel)
						entries.Add(new EntryModel
												{
													Name = entryViewModel.Title,
													Content = entryViewModel.Content,
													URL = entryViewModel.URL
												}
										);
					// Graba los datos del blog
					MainViewModel.BlogManager.SaveBlog(blog, entries);
			}
	}

	/// <summary>
	///		Obtiene el nombre de adjunto de la entrada seleccionada
	/// </summary>
	private KeyValuePair<string, string> GetAttachment(EntryModel entry)
	{
		KeyValuePair<string, string> attachment = new KeyValuePair<string, string>();

			// Obtiene el nombre de archivo o la URL del adjunto
			if (entry != null && !string.IsNullOrWhiteSpace(entry.UrlEnclosure))
			{
				// Obtiene el nombre de archivo
				attachment = new KeyValuePair<string, string>(entry.Name, System.IO.Path.Combine(MainViewModel.ConfigurationViewModel.PathBlogs, entry.DownloadFileName));
				// Si no existe el archivo, obtiene la Url
				if (!System.IO.File.Exists(attachment.Value))
					attachment = new KeyValuePair<string, string>(entry.Name, entry.UrlEnclosure);
			}
			// Devuelve el nombre del adjunto
			return attachment;
	}

	/// <summary>
	///		Reproduce el adjunto de una entrada
	/// </summary>
	private void PlayEntry()
	{
		BlogEntriesCollectionViewModel entries = LastEntriesSelectedForView;
		Dictionary<string, List<KeyValuePair<string, string>>> attachments = new();

			// Obtiene los nombres de los archivos adjuntos
			foreach (BlogEntryViewModel entry in entries)
			{
				KeyValuePair<string, string> attachment = GetAttachment(entry?.Entry);

					if (!string.IsNullOrEmpty(attachment.Value))
					{
						if (attachments.TryGetValue(entry.BlogName, out var dictionaryEntry))
							dictionaryEntry.Add(attachment);
						else
							attachments.Add(entry.BlogName, new List<KeyValuePair<string, string>> { attachment });
					}
			}
			// Ejecuta los archivos
			if (attachments.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine("Aquí faltan cosas");
				//foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> attachment in attachments)
				//	BlogReaderViewModel.Instance.HostController.Messenger.SendFilesToMediaPlayer(BlogReaderViewModel.Instance.ModuleName, attachment.Key, attachment.Value);
			}
			else
				MainViewModel.ViewsController.SystemController.ShowMessage("No se ha seleccionado ningún elemento con adjuntos");
	}

	/// <summary>
	///		Obtiene el mensaje de grabación
	/// </summary>
	public string GetSaveAndCloseMessage() => "¿Desea grabar las noticias?";

	/// <summary>
	///		Graba los detalles
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// No hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		// Limpia los datos
		EntriesList.Clear();
		LastEntriesSelectedForView.Clear();
		HtmlNews = string.Empty;
		// Cierra la ventana
		Closed?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		Blogs
	/// </summary>
	private BlogsModelCollection Blogs { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header => "News";

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId 
	{
		get { return _tabId; }
		private set { CheckProperty(ref _tabId, value); }
	}

	/// <summary>
	///		Entradas
	/// </summary>
	public BlogEntriesCollectionViewModel EntriesList
	{
		get
		{ 
			// Si no está cargado, obtiene las entradas
			if (_entries == null)
				_entries = GetEntries();
			// Devuelve las entradas
			return _entries;
		}
		set { CheckObject(ref _entries, value); }
	}

	/// <summary>
	///		Entradas seleccionadoas para verlas y tratarlas desde el explorador
	/// </summary>
	private BlogEntriesCollectionViewModel LastEntriesSelectedForView
	{
		get
		{
			// Obtiene las entradas que se deben mostrar en el explorador
			if (_entriesForView == null || _isDirty)
			{ 
				// Recarga la lista
				_entriesForView = GetEntriesSelected();
				// Indica que se han leído los últimos datos
				_isDirty = false;
			}
			// Devuelve las entradas de la vista
			return _entriesForView;
		}
	}

	/// <summary>
	///		Entrada seleccionada
	/// </summary>
	public BlogEntryViewModel? SelectedEntry
	{
		get { return _selectedEntry; }
		set { CheckObject(ref _selectedEntry, value); }
	}

	/// <summary>
	///		Indica si se deben ver o no las entradas interesantes
	/// </summary>
	public bool SeeInteresting
	{
		get { return _mustSeeInteresting; }
		set
		{
			if (CheckProperty(ref _mustSeeInteresting, value))
				MarkAsDirty();
		}
	}

	/// <summary>
	///		Indica si se deben ver o no las entradas no leídas
	/// </summary>
	public bool SeeNotRead
	{
		get { return _mustSeeNotRead; }
		set
		{
			if (CheckProperty(ref _mustSeeNotRead, value))
				MarkAsDirty();
		}
	}

	/// <summary>
	///		Indica si se deben ver o no las entradas leídas
	/// </summary>
	public bool SeeRead
	{
		get { return _mustSeeRead; }
		set
		{
			if (CheckProperty(ref _mustSeeRead, value))
				MarkAsDirty();
		}
	}

	/// <summary>
	///		Número de entradas seleccionadas
	/// </summary>
	public int EntriesNumber
	{
		get { return _entriesNumber; }
		set { CheckProperty(ref _entriesNumber, value); }
	}

	/// <summary>
	///		Registros por página
	/// </summary>
	public int RecordsPerPage
	{
		get { return _recordsPerPage; }
		set { CheckProperty(ref _recordsPerPage, value); }
	}

	/// <summary>
	///		Página actual
	/// </summary>
	public int ActualPage
	{
		get { return _actualPage; }
		set { CheckProperty(ref _actualPage, value); }
	}

	/// <summary>
	///		Número de páginas
	/// </summary>
	public int Pages
	{
		get { return _pageIndex; }
		set { CheckProperty(ref _pageIndex, value); }
	}

	/// <summary>
	///		Texto Html de las noticias
	/// </summary>
	public string HtmlNews
	{
		get { return _htmlNews; }
		set { CheckProperty(ref _htmlNews, value); }
	}

	/// <summary>
	///		Comando para actualizar
	/// </summary>
	public BaseCommand RefreshCommand { get; }

	/// <summary>
	///		Comando para mostrar la primera página
	/// </summary>
	public BaseCommand FirstPageCommand { get; }

	/// <summary>
	///		Comando para mostrar la siguiente página
	/// </summary>
	public BaseCommand NextPageCommand { get; }

	/// <summary>
	///		Comando para mostrar la página anterior
	/// </summary>
	public BaseCommand PreviousPageCommand { get; }

	/// <summary>
	///		Comando para mostrar la última página
	/// </summary>
	public BaseCommand LastPageCommand { get; }

	/// <summary>
	///		Comando para marcar un elemento como leído
	/// </summary>
	public BaseCommand MarkAsReadCommand { get; }

	/// <summary>
	///		Comando para marcar un elemento como no leído
	/// </summary>
	public BaseCommand MarkAsNotReadCommand { get; }

	/// <summary>
	///		Comando para marcar un elemento como interesante
	/// </summary>
	public BaseCommand MarkAsInterestingCommand { get; }

	/// <summary>
	///		Comando para exportar entradas
	/// </summary>
	public BaseCommand ExportEntriesCommand { get; }

	/// <summary>
	///		Comando para reproducir archivos
	/// </summary>
	public BaseCommand PlayCommand { get; }

	/// <summary>
	///		Comando para borrar las entradas
	/// </summary>
	public BaseCommand DeleteCommand { get; }
}