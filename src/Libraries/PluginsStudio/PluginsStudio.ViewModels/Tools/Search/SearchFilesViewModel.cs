using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Search;

/// <summary>
///		ViewModel para búsqueda de archivos
/// </summary>
public class SearchFilesViewModel : BaseObservableObject, Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _textSearch = default!, _header = default!, _folder = default!;
	private bool _caseSensitive, _wholeWord, _useRegex;
	private TreeSearchFilesResultViewModel _treeResultsViewModel = default!;

	public SearchFilesViewModel(PluginsStudioViewModel mainViewModel, string folder) : base(false)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		TreeResultsViewModel = new TreeSearchFilesResultViewModel(this);
		Header = "Search";
		Folder = folder;
		// Inicializa los comandos
		SearchCommand = new BaseCommand(async _ => await SearchFilesAsync(), _ => CanSearchFiles())
								.AddListener(this, nameof(TextSearch));
	}

	/// <summary>
	///		Busca el texto en los archivos
	/// </summary>
	private async Task SearchFilesAsync()
	{
		await TreeResultsViewModel.SearchAsync(Folder, GetMask(), TextSearch, CaseSensitive, WholeWord, UseRegex, new CancellationToken());
	}

	/// <summary>
	///		Ejecuta un comando externo
	/// </summary>
	public void Execute(Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Graba la ventana (en este caso no hace nada)
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// No hace nada
	}

	/// <summary>
	///		Obtiene el mensaje de grabación
	/// </summary>
	public string GetSaveAndCloseMessage() => $"¿Desea grabar las modificaciones?";

	/// <summary>
	///		Obtiene la máscara de búsqueda de archivos
	/// </summary>
	private string GetMask() => ".sql;.sqlx;.py;.md;.xml;.txt;.json";

	/// <summary>
	///		Indica si se pueden buscar archivos
	/// </summary>
	private bool CanSearchFiles() => !string.IsNullOrWhiteSpace(TextSearch);

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera de la ventana
	/// </summary>
	public string Header
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Id de la ficha en la aplicación
	/// </summary>
	public string TabId => GetType().ToString();

	/// <summary>
	///		Carpeta sobre la que se realiza la búsqueda
	/// </summary>
	public string Folder
	{
		get { return _folder; }
		set { CheckProperty(ref _folder, value); }
	}

	/// <summary>
	///		Arbol de resultados de búsqueda
	/// </summary>
	public TreeSearchFilesResultViewModel TreeResultsViewModel
	{
		get { return _treeResultsViewModel; }
		set { CheckObject(ref _treeResultsViewModel!, value); }
	}

	/// <summary>
	///		Texto a buscar
	/// </summary>
	public string TextSearch
	{
		get { return _textSearch; }
		set { CheckProperty(ref _textSearch, value); }
	}

	/// <summary>
	///		Tener en cuenta las mayúsculas
	/// </summary>
	public bool CaseSensitive
	{ 
		get { return _caseSensitive; }
		set { CheckProperty(ref _caseSensitive, value); }
	}

	/// <summary>
	///		Buscar la palabra completa
	/// </summary>
	public bool WholeWord
	{ 
		get { return _wholeWord; }
		set { CheckProperty(ref _wholeWord, value); }
	}

	/// <summary>
	///		Utiliza expresiones regulares en la búsqueda
	/// </summary>
	public bool UseRegex
	{ 
		get { return _useRegex; }
		set { CheckProperty(ref _useRegex, value); }
	}

	/// <summary>
	///		Comando de búsqueda
	/// </summary>
	public BaseCommand SearchCommand { get; }
}