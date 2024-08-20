using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ListView;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Search;

/// <summary>
///		ViewModel para búsqueda de archivos
/// </summary>
public class SearchFilesViewModel : BaseObservableObject
{
	// Variables privadas
	private string _textSearch = string.Empty;
	private bool _caseSensitive, _wholeWord, _useRegex;
	private ControlListViewModel _foldersViewModel = default!;
	private TreeSearchFilesResultViewModel _treeResultsViewModel = default!;

	public SearchFilesViewModel(PluginsStudioViewModel mainViewModel)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		FoldersViewModel = new ControlListViewModel();
		TreeResultsViewModel = new TreeSearchFilesResultViewModel(this);
		// Inicializa los comandos
		SearchCommand = new BaseCommand(async _ => await SearchFilesAsync(), _ => CanSearchFiles())
								.AddListener(this, nameof(TextSearch));
	}

	/// <summary>
	///		Carga los directorios de la solución
	/// </summary>
	public void LoadFolders()
	{
		// Limpia la lista de directorios
		FoldersViewModel.Items.Clear();
		// Añade los directorios de la solución
		if (MainViewModel.WorkspacesViewModel.SelectedItem != null)
			foreach (string folder in MainViewModel.WorkspacesViewModel.SelectedItem.Folders)
				if (Directory.Exists(folder))
				{
					ControlItemViewModel item = new ControlItemViewModel(Path.GetFileName(folder), folder);

						// Selecciona la carpeta
						item.IsChecked = true;
						// Añade el control
						FoldersViewModel.Add(item, false);
				}
	}

	/// <summary>
	///		Busca el texto en los archivos
	/// </summary>
	private async Task SearchFilesAsync()
	{
		await TreeResultsViewModel.SearchAsync(GetSelectedFolders(), GetMask(), TextSearch, CaseSensitive, WholeWord, UseRegex, new CancellationToken());
	}

	/// <summary>
	///		Obtiene las carpetas seleccionadas
	/// </summary>
	private List<string> GetSelectedFolders()
	{
		List<string> folders = new List<string>();

			// Obtiene las carpetas seleccionadas
			foreach (ControlItemViewModel item in FoldersViewModel.Items)
				if (item.IsChecked && item.Tag is string folder)
					folders.Add(folder);
			// Devuelve la lista de carpetas
			return folders;
	}

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
	///		Lista de directorios sobre los que se realiza la búsqueda
	/// </summary>
	public ControlListViewModel FoldersViewModel
	{
		get { return _foldersViewModel; }
		set { CheckObject(ref _foldersViewModel, value); }
	}

	/// <summary>
	///		Arbol de resultados de búsqueda
	/// </summary>
	public TreeSearchFilesResultViewModel TreeResultsViewModel
	{
		get { return _treeResultsViewModel; }
		set { CheckObject(ref _treeResultsViewModel, value); }
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