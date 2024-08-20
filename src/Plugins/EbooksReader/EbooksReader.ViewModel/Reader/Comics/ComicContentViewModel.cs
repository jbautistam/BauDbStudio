using Bau.Libraries.LibCompressor;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ListView;

namespace Bau.Libraries.EbooksReader.ViewModel.Reader.Comics;

/// <summary>
///		ViewModel para ver el contenido de un <see cref="BookModel"/>
/// </summary>
public class ComicContentViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Eventos públícos
	public EventHandler<EventArguments.ZoomEventArgs>? UpdateZoom;
	// Variables privadas
	private ControlListViewModel _pages = default!;
	private string _tempPath = default!, _fileName = default!;
	private double _zoom;

	public ComicContentViewModel(EBookReaderViewModel mainViewModel, string fileName) : base(false)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		if (!string.IsNullOrWhiteSpace(fileName))
			Header = Path.GetFileName(fileName);
		else
			Header = "File";
		// Inicializa los objetos
		ComicPages = new ControlListViewModel();
		// Inicializa los comandos
		FirstPageCommand = new BaseCommand(_ => GoFirstPage(), _ => CanGoFirstPage())
									.AddListener(ComicPages, nameof(ComicPages.SelectedItem));
		LastPageCommand = new BaseCommand(_ => GoLastPage(), _ => CanGoLastPage())
									.AddListener(ComicPages, nameof(ComicPages.SelectedItem));
		NextPageCommand = new BaseCommand(_ => GoNextPage(), _ => CanGoNextPage())
									.AddListener(ComicPages, nameof(ComicPages.SelectedItem));
		PreviousPageCommand = new BaseCommand(_ => GoPreviousPage(), _ => CanGoPreviousPage())
									.AddListener(ComicPages, nameof(ComicPages.SelectedItem));
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	public async Task ParseAsync()
	{
		Compressor compressor = new();
		List<string> files = new();

			// Crea el directorio temporal
			_tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			LibHelper.Files.HelperFiles.MakePath(_tempPath);
			// Asigna el manejador de eventos
			compressor.Progress += (sender, args) => files.Add(args.FileName);
			compressor.End += (sender, args) => AddPages(files);
			// Descomprime los archivos
			try
			{
				await Task.Run(() => compressor.Uncompress(FileName, _tempPath));
			}
			catch (Exception exception)
			{
				MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load file '{FileName}'. {exception.Message}");
			}
			// Pasa a la primera página
			GoFirstPage();
	}

	/// <summary>
	///		Añade una página a la lista
	/// </summary>
	private void AddPages(List<string> files)
	{
		// Ordena los archivos
		files.Sort((first, second) => first.CompareTo(second));
		// Carga los archivos
		foreach (string file in files)
			if (LibHelper.Files.HelperFiles.CheckIsImage(file))
				ComicPages.Add(new BookPageViewModel(this, file, file, ComicPages.Items.Count + 1), false);
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
		GoPage(ComicPages.Items.Count - 1);
	}

	/// <summary>
	///		Comprueba si puede ir a la última página
	/// </summary>
	private bool CanGoLastPage() => CanGoPage(ComicPages.Items.Count - 1);

	/// <summary>
	///		Pasa a la primera página
	/// </summary>
	public void GoNextPage()
	{
		GoPage(GetActualPage() + 1);
	}

	/// <summary>
	///		Comprueba si puede ir a la primera página
	/// </summary>
	private bool CanGoNextPage() => CanGoPage(GetActualPage() + 1);

	/// <summary>
	///		Pasa a la primera página
	/// </summary>
	public void GoPreviousPage()
	{
		GoPage(GetActualPage() - 1);
	}

	/// <summary>
	///		Comprueba si puede ir a la primera página
	/// </summary>
	private bool CanGoPreviousPage() => CanGoPage(GetActualPage() - 1);

	/// <summary>
	///		Comprueba si se puede ir a una página
	/// </summary>
	private bool CanGoPage(int page)
	{
		return ComicPages.Items.Count > 0 && page >= 0  && page < ComicPages.Items.Count && (ComicPages.SelectedItem as BookPageViewModel)?.Page != page + 1;
	}

	/// <summary>
	///		Selecciona una página
	/// </summary>
	public void GoPage(int page)
	{
		if (CanGoPage(page))
		{
			double previousZoom = Zoom;

				// Cambia la página seleccionada
				ComicPages.SelectedItem = ComicPages.Items[page];
				// Lanza el evento para que recupere el zoom	
				UpdateZoom?.Invoke(this, new EventArguments.ZoomEventArgs(previousZoom));
		}
	}

	/// <summary>
	///		Obtiene la página actual
	/// </summary>
	private int GetActualPage()
	{
		if (ComicPages.SelectedItem is BookPageViewModel bookPage)
			return bookPage.Page - 1;
		else
			return 0;
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
		if (!string.IsNullOrWhiteSpace(_tempPath) && Directory.Exists(_tempPath))
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
	///		Páginas del cómic (no utiliza Pages en el View)
	/// </summary>
	public ControlListViewModel ComicPages
	{
		get { return _pages; }
		set { CheckObject(ref _pages, value); }
	}

	/// <summary>
	///		Zoom actual
	/// </summary>
	public double Zoom
	{
		get { return _zoom; }
		set { CheckProperty(ref _zoom, value); }
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
