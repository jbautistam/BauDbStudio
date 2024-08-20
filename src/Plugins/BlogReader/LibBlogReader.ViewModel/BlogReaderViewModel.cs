namespace Bau.Libraries.LibBlogReader.ViewModel;

/// <summary>
///		ViewModel principal del lector de blogs
/// </summary>
public class BlogReaderViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private Blogs.TreeBlogs.TreeBlogsViewModel _treeBlogsViewModel = default!;

	public BlogReaderViewModel(Controllers.IBlogReaderController mainController)
	{
		// Asigna el manager de blogs y el controlador de vistas
		BlogManager = new Application.BlogReaderManager(mainController.Logger);
		ViewsController = mainController;
		// Asigna los objetos
		ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
		TreeBlogs = new Blogs.TreeBlogs.TreeBlogsViewModel(this);
		BlogDownloadProcesor = new Controllers.Process.BlogDownloadProcessor(this);
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		// Carga la configuración
		ConfigurationViewModel.Load();
		// Arranca el procesador de descargas
		BlogDownloadProcesor.Start();
	}

	/// <summary>
	///		Carga el directorio (en este caso, siempre es el mismo directorio, por eso no lo cambia)
	/// </summary>
	public void Load(string path)
	{
		// Configura el directorio
		if (string.IsNullOrWhiteSpace(ConfigurationViewModel.PathBlogs) && !string.IsNullOrWhiteSpace(path))
			ConfigurationViewModel.PathBlogs = path;
		BlogManager.Configuration.PathBlogs = ConfigurationViewModel.PathBlogs;
		// Carga la lista de blogs
		try
		{
			BlogManager.Load();
		}
		catch (Exception exception)
		{
			ViewsController.HostController.SystemController.ShowMessage($"Error when load blogs file at '{path}'. {exception.Message}");
		}
		// Carga los exploradores
		TreeBlogs.Load();
	}

	/// <summary>
	///		Manager de blogs
	/// </summary>
	public Application.BlogReaderManager BlogManager { get; }

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IBlogReaderController ViewsController { get; }

	/// <summary>
	///		ViewModel de configuración
	/// </summary>
	public Configuration.ConfigurationViewModel ConfigurationViewModel { get; }

	/// <summary>
	///		Arbol de blogs
	/// </summary>
	public Blogs.TreeBlogs.TreeBlogsViewModel TreeBlogs
	{
		get { return _treeBlogsViewModel; }
		set { CheckObject(ref _treeBlogsViewModel!, value); }
	}

	/// <summary>
	///		Proceso de descarga de blogs
	/// </summary>
	internal Controllers.Process.BlogDownloadProcessor BlogDownloadProcesor { get; }
}
