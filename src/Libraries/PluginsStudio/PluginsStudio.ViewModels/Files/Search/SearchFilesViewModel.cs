using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

namespace Bau.Libraries.PluginsStudio.ViewModels.Files.Search;

/// <summary>
///		ViewModel para búsqueda de archivos
/// </summary>
public class SearchFilesViewModel : BaseObservableObject, Base.Interfaces.IDetailViewModel
{
	// Constantes privadas
	private const string ConfigurationMaskKey = "SearchMask";
	// Variables privadas
	private string _textSearch = default!, _header = default!, _folder = default!, _mask = default!;
	private bool _caseSensitive, _wholeWord, _useRegex;
	private TreeSearchFilesResultViewModel _treeResultsViewModel = default!;
	private ControlItemCollectionViewModel<SearchFileTypeItemViewModel> _fileTypes = default!;

	public SearchFilesViewModel(PluginsStudioViewModel mainViewModel, string folder, List<FileAssignedModel> filesAssigned) : base(false)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		TreeResultsViewModel = new TreeSearchFilesResultViewModel(this);
		Header = $"Search {Path.GetFileName(folder)}";
		Folder = folder;
		FileTypes = CreateFileTypes(filesAssigned);
		// Actualiza la máscara
		UpdateMask();
		// Inicializa los comandos
		SearchCommand = new BaseCommand(async _ => await SearchFilesAsync(), _ => CanSearchFiles())
								.AddListener(this, nameof(TextSearch));
		SelectAllCommand = new BaseCommand(_ => SelectAllFileTypes(true));
		UnSelectAllCommand = new BaseCommand(_ => SelectAllFileTypes(false));
	}

	/// <summary>
	///		Crea los tipos de archivos a partir de la lista de archivos asignados de los plugins
	/// </summary>
	private ControlItemCollectionViewModel<SearchFileTypeItemViewModel> CreateFileTypes(List<FileAssignedModel> filesAssigned)
	{
		List<string> lastExtensions = GetExtensionsSelected();
		ControlItemCollectionViewModel<SearchFileTypeItemViewModel> items = [];

			// Añade los archivos asignados
			filesAssigned.Sort((first, second) => first.Name.CompareTo(second.Name));
			foreach (FileAssignedModel fileAssigned in filesAssigned)
				if (fileAssigned.CanSearch)
					items.Add(new SearchFileTypeItemViewModel(this, fileAssigned.Name, fileAssigned.FileExtension, fileAssigned.Icon, 
															  IsExtensionAtMask(lastExtensions, fileAssigned.FileExtension)));
			// Devuelve la colección de elementos
			return items;

		// Obtiene las últimas extensiones seleccionadas para búsquedas
		List<string> GetExtensionsSelected()
		{
			List<string> extensions = [];
			string lastMasks = MainViewModel.MainController.PluginsController.ConfigurationController.GetConfiguration(PluginsStudioViewModel.PluginName, ConfigurationMaskKey);

				// Obtiene las máscaras seleccionadas previamente
				if (!string.IsNullOrWhiteSpace(lastMasks))
					foreach (string lastMask in lastMasks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
						extensions.Add(lastMask);
				// Devuelve las extensions
				return extensions;
		}

		// Comprueba si una extensión está entre las últimas seleccionadas
		bool IsExtensionAtMask(List<string> lastExtensions, string extension)
		{
			if (lastExtensions.Count == 0)
				return true;
			else
			{
				// Busca la extensión en la lista
				foreach (string lastExtension in lastExtensions)
					if (lastExtension.Equals(extension, StringComparison.CurrentCultureIgnoreCase) ||
							$"*.{lastExtension}".Equals(extension, StringComparison.CurrentCultureIgnoreCase))
						return true;
				// Si ha llegado hasta aquí es porque no ha encontrado nada
				return false;
			}
		}
	}

	/// <summary>
	///		Actualiza la máscara con los archivos seleccionados
	/// </summary>
	internal void UpdateMask()
	{
		string mask = string.Empty;

			// Añade las extensiones a la máscara
			if (FileTypes is not null)
				foreach (SearchFileTypeItemViewModel fileType in FileTypes)
					if (fileType.IsChecked)
						mask = mask.AddWithSeparator(fileType.Extension, ";");
			// Guarda la máscara en la configuración
			MainViewModel.MainController.PluginsController.ConfigurationController.SetConfiguration(PluginsStudioViewModel.PluginName, ConfigurationMaskKey, mask);
			// Asigna la máscara
			Mask = mask;
	}

	/// <summary>
	///		Busca el texto en los archivos
	/// </summary>
	private async Task SearchFilesAsync()
	{
		await TreeResultsViewModel.SearchAsync(Folder, Mask, TextSearch, CaseSensitive, WholeWord, UseRegex, new CancellationToken());
	}

	/// <summary>
	///		Selecciona / deselecciona los tipos de archivos
	/// </summary>
	private void SelectAllFileTypes(bool select)
	{
		foreach (SearchFileTypeItemViewModel fileType in FileTypes)
			fileType.IsChecked = select;
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
	public string TabId => $"{GetType().ToString()}_{Path.GetFileName(Folder)}";

	/// <summary>
	///		Carpeta sobre la que se realiza la búsqueda
	/// </summary>
	public string Folder
	{
		get { return _folder; }
		set { CheckProperty(ref _folder, value); }
	}

	/// <summary>
	///		Máscara de búsqueda de archivos
	/// </summary>
	public string Mask
	{
		get { return _mask; }
		set { CheckProperty(ref _mask, value); }
	}

	/// <summary>
	///		Lista de tipos de archivos
	/// </summary>
	public ControlItemCollectionViewModel<SearchFileTypeItemViewModel> FileTypes
	{
		get { return _fileTypes; }
		set { CheckObject(ref _fileTypes!, value); }
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

	/// <summary>
	///		Comando para seleccionar todos los tipos de archivos
	/// </summary>
	public BaseCommand SelectAllCommand { get; }

	/// <summary>
	///		Comando para deseleccionar todos los tipos de archivos
	/// </summary>
	public BaseCommand UnSelectAllCommand { get; }
}