using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibBlogReader.ViewModel.Configuration;

/// <summary>
///		ViewModel para la configuración
/// </summary>
public class ConfigurationViewModel : BauMvvm.ViewModels.BaseObservableObject
{   
	// Constants privadas
	private const string ApplicationName = "BlogReader";
	// Variables privadas
	private string _pathBlogs = default!;
	private int _minutesBetweenDownload, _recordsPerPage;
	private bool _downloadEnabled, _seeEntriesRead, _seeEntriesNotRead, _seeEntriesInteresting, _showDisabledBlogs, _downloadDisabledBlogs;

	public ConfigurationViewModel(BlogReaderViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Carga los datos de configuración
	/// </summary>
	internal void Load()
	{
		PathBlogs = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(PathBlogs));
		MinutesBetweenDownload = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(MinutesBetweenDownload)).GetInt(60);
		RecordsPerPage = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(RecordsPerPage)).GetInt(25);
		DownloadEnabled = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(DownloadEnabled)).GetBool();
		SeeEntriesRead = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(SeeEntriesRead)).GetBool();
		SeeEntriesNotRead = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(SeeEntriesNotRead)).GetBool();
		SeeEntriesInteresting = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(SeeEntriesInteresting)).GetBool();
		ShowNewsDisabledBlogs = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(ShowNewsDisabledBlogs)).GetBool();
		DownloadDisabledBlogs = MainViewModel.ViewsController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(DownloadDisabledBlogs)).GetBool();
	}

	/// <summary>
	///		Comprueba si los datos son correctos
	/// </summary>
	public bool ValidateData(out string error)
	{ 
		// Inicializa los argumentos de salida
		error = string.Empty;
		// Comprueba los datos
		if (string.IsNullOrWhiteSpace(PathBlogs) || !Directory.Exists(PathBlogs))
			error = "Enter a valid path";
		// Devuelve el valor que indica si los datos son correctos
		return error.IsEmpty();
	}

	/// <summary>
	///		Graba los datos (y actualiza el árbol)
	/// </summary>
	public void Save()
	{
		// Grava la configuración
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(PathBlogs), 
																								PathBlogs);
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(MinutesBetweenDownload), 
																								MinutesBetweenDownload.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(RecordsPerPage), 
																								RecordsPerPage.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(DownloadEnabled), 
																								DownloadEnabled.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(SeeEntriesRead), 
																								SeeEntriesRead.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(SeeEntriesNotRead), 
																								SeeEntriesNotRead.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(SeeEntriesInteresting), 
																								SeeEntriesInteresting.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(ShowNewsDisabledBlogs), 
																								ShowNewsDisabledBlogs.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(DownloadDisabledBlogs), 
																								DownloadDisabledBlogs.ToString());
		// Actualiza el árbol
		MainViewModel.Load(PathBlogs);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public BlogReaderViewModel MainViewModel { get; }

	/// <summary>
	///		Directorio a partir del que se encuentran los datos de los blogs
	/// </summary>
	public string PathBlogs
	{
		get { return _pathBlogs; }
		set { CheckProperty(ref _pathBlogs, value); }
	}

	/// <summary>
	///		Minutos entre descargas
	/// </summary>
	public int MinutesBetweenDownload
	{
		get { return _minutesBetweenDownload; }
		set { CheckProperty(ref _minutesBetweenDownload, value); }
	}

	/// <summary>
	///		Indica si la descarga automática está activa
	/// </summary>
	public bool DownloadEnabled
	{
		get { return _downloadEnabled; }
		set { CheckProperty(ref _downloadEnabled, value); }
	}

	/// <summary>
	///		Registros que se muestran al visualizar los blogs
	/// </summary>
	public int RecordsPerPage
	{
		get { return _recordsPerPage; }
		set { CheckProperty(ref _recordsPerPage, value); }
	}

	/// <summary>
	///		Muestra las entradas leidas
	/// </summary>
	public bool SeeEntriesRead
	{
		get { return _seeEntriesRead; }
		set { CheckProperty(ref _seeEntriesRead, value); }
	}

	/// <summary>
	///		Muestra las entradas no leidas
	/// </summary>
	public bool SeeEntriesNotRead
	{
		get { return _seeEntriesNotRead; }
		set { CheckProperty(ref _seeEntriesNotRead, value); }
	}

	/// <summary>
	///		Muestra las entradas interesantes
	/// </summary>
	public bool SeeEntriesInteresting
	{
		get { return _seeEntriesInteresting; }
		set { CheckProperty(ref _seeEntriesInteresting, value); }
	}

	/// <summary>
	///		Indica si se deben mostrar las noticias de los blogs inactivos
	/// </summary>
	public bool ShowNewsDisabledBlogs
	{
		get { return _showDisabledBlogs; }
		set { CheckProperty(ref _showDisabledBlogs, value); }
	}

	/// <summary>
	///		Indica si se deben descargar los blogs inactivos
	/// </summary>
	public bool DownloadDisabledBlogs
	{
		get { return _downloadDisabledBlogs; }
		set { CheckProperty(ref _downloadDisabledBlogs, value); }
	}
}
