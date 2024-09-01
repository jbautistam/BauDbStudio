using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.EbooksReader.ViewModel.Reader.eBooks;

/// <summary>
///		ViewModel para ver el contenido de un <see cref="LibEBooks.Models.eBook.Book"/>
/// </summary>
public class EBookContentViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Eventos públicos
	public event EventHandler? ChangePage;
	// Variables privadas
	private string _fileName = default!, _tempPath = default!;
	private Explorer.TreeEbookViewModel? _treePages;
	private List<Explorer.EbookNodeViewModel> _bookNodePages = new();
	private int _actualPage, _pages;

	public EBookContentViewModel(EBookReaderViewModel mainViewModel, string fileName) : base(false)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		Header = Path.GetFileNameWithoutExtension(fileName);
		// Inicializa los objetos
		TreePages = new Explorer.TreeEbookViewModel(this);
		TreePages.PropertyChanged += (sender, args) => 
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName) && args.PropertyName.Equals(nameof(TreePages.SelectedNode)))
													RaiseEventChangePage();
											};
		// Inicializa los comandos
		FirstPageCommand = new BaseCommand(_ => GoFirstPage(), _ => CanGoFirstPage())
									.AddListener(TreePages, nameof(TreePages.SelectedNode));
		LastPageCommand = new BaseCommand(_ => GoLastPage(), _ => CanGoLastPage())
									.AddListener(TreePages, nameof(TreePages.SelectedNode));
		NextPageCommand = new BaseCommand(_ => GoNextPage(), _ => CanGoNextPage())
									.AddListener(TreePages, nameof(TreePages.SelectedNode));
		PreviousPageCommand = new BaseCommand(_ => GoPreviousPage(), _ => CanGoPreviousPage())
									.AddListener(TreePages, nameof(TreePages.SelectedNode));
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	public async Task ParseAsync()
	{
		// Evita el warning del async
		await Task.Delay(1);
		// Asigna el directorio temporal
		_tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		// Crea el directorio temporal
		LibHelper.Files.HelperFiles.MakePath(_tempPath);
		// Carga el libro
		try
		{
			LibEBooks.Services.EBooksManager manager = new();
			LibEBooks.Models.eBook.Book eBook = manager.Load(LibEBooks.Services.EBooksManager.BookType.ePub, FileName, _tempPath);

				if (TreePages is not null)
				{
					// Rellena el árbol de páginas
					_bookNodePages = TreePages.Load(eBook);
					// Guarda el número de páginas
					Pages = _bookNodePages.Count;
				}
		}
		catch (Exception exception)
		{
			MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load file '{FileName}'. {exception.Message}");
		}
		// Pasa a la primera página
		GoFirstPage();
	}

	/// <summary>
	///		Lanza el evento de cambio de página
	/// </summary>
	private void RaiseEventChangePage()
	{
		// Asigna la página actual
		if (TreePages?.SelectedNode is null)
			ActualPage = 0;
		else
		{
			string? nodeId = (TreePages.SelectedNode as Explorer.EbookNodeViewModel)?.Id;

				// Busca en la lista de páginas la página que se corresponde con ese nodo
				if (!string.IsNullOrWhiteSpace(nodeId))
					for (int index = 0; index < _bookNodePages.Count; index++)
					{
						string? pageId = _bookNodePages[index]?.Id;

							if (!string.IsNullOrWhiteSpace(pageId) && pageId.Equals(nodeId, StringComparison.CurrentCultureIgnoreCase))
								ActualPage = index;
					}
		}
		// Llama al evento para que se visualice la página
		ChangePage?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Pasa a la primera página
	/// </summary>
	private void GoFirstPage()
	{
		GoPage(0);
	}

	/// <summary>
	///		Comprueba si puede ir a la primera página
	/// </summary>
	private bool CanGoFirstPage() => CanGoPage(0);

	/// <summary>
	///		Pasa a la última página
	/// </summary>
	private void GoLastPage()
	{
		GoPage(_bookNodePages.Count - 1);
	}

	/// <summary>
	///		Comprueba si puede ir a la última página
	/// </summary>
	private bool CanGoLastPage() => CanGoPage(_bookNodePages.Count - 1);

	/// <summary>
	///		Pasa a la primera página
	/// </summary>
	private void GoNextPage()
	{
		GoPage(ActualPage + 1);
	}

	/// <summary>
	///		Comprueba si puede ir a la primera página
	/// </summary>
	private bool CanGoNextPage() => CanGoPage(ActualPage + 1);

	/// <summary>
	///		Pasa a la primera página
	/// </summary>
	private void GoPreviousPage()
	{
		GoPage(ActualPage - 1);
	}

	/// <summary>
	///		Comprueba si puede ir a la primera página
	/// </summary>
	private bool CanGoPreviousPage() => CanGoPage(ActualPage - 1);

	/// <summary>
	///		Comprueba si se puede ir a una página
	/// </summary>
	private bool CanGoPage(int page) => _bookNodePages.Count > 0 && page >= 0  && page < _bookNodePages.Count;

	/// <summary>
	///		Selecciona una página
	/// </summary>
	private void GoPage(int page)
	{
		if (_bookNodePages.Count > 0 && TreePages is not null)
		{
			// Cambia la página
			TreePages.SelectedNode = _bookNodePages[page];
			// Lanza el evento de cambio de página
			RaiseEventChangePage();
		}
	}

	/// <summary>
	///		Obtiene la URL de la página seleccionada
	/// </summary>
	public string GetUrlPage()
	{
		string? url = (TreePages?.SelectedNode as Explorer.EbookNodeViewModel)?.Url;

			// Convierte la URL en una URL de archivo
			if (!string.IsNullOrWhiteSpace(url))
				return "file://" + Path.Combine(_tempPath, url).Replace("\\", "/");
			else
				return string.Empty;
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage() => string.Empty;

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Cierra la ventana de detalles: elimina los archivos temporales
	/// </summary>
	public void Close()
	{
		if (!string.IsNullOrWhiteSpace(_tempPath))
			LibHelper.Files.HelperFiles.KillPath(_tempPath);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public EBookReaderViewModel MainViewModel { get; set; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				if (string.IsNullOrWhiteSpace(_fileName))
					Header = "New filename";
				else
					Header = Path.GetFileName(_fileName);
			}
		}
	}

	/// <summary>
	///		Arbol de páginas
	/// </summary>
	public Explorer.TreeEbookViewModel? TreePages
	{
		get { return _treePages; }
		set { CheckObject(ref _treePages, value); }
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
	///		Páginas del libro
	/// </summary>
	public int Pages
	{
		get { return _pages; }
		set { CheckProperty(ref _pages, value); }
	}

	/// <summary>
	///		Comando para ir a la primera página
	/// </summary>
	public BaseCommand FirstPageCommand { get; }

	/// <summary>
	///		Comando para ir a la página anterior
	/// </summary>
	public BaseCommand PreviousPageCommand { get; }

	/// <summary>
	///		Comando para ir a la siguiente página
	/// </summary>
	public BaseCommand NextPageCommand { get; }

	/// <summary>
	///		Comando para ir a la última página
	/// </summary>
	public BaseCommand LastPageCommand { get; }
}
