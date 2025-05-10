using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Search;

/// <summary>
///		ViewModel para los resultados de la búsqueda
/// </summary>
public class TreeSearchFilesResultViewModel : BaseObservableObject
{
	// Variables privadas
	private ObservableCollection<TreeResultNodeViewModel> _children = new();
	private TreeResultNodeViewModel? _selectedNode;
	private string _fileSearched = string.Empty;

	public TreeSearchFilesResultViewModel(SearchFilesViewModel searchFilesViewModel)
	{
		SearchFilesViewModel = searchFilesViewModel;
		OpenCommand = new BaseCommand(_ => OpenFile(), _ => SelectedNode != null)
								.AddListener(this, nameof(SelectedNode));
	}

	/// <summary>
	///		Busca texto en los archivos
	/// </summary>
	internal async Task SearchAsync(string folder, string mask, string textSearch, bool caseSensitive, bool wholeWords, bool useRegex, CancellationToken cancellationToken)
	{
		// Inicializa la lista
		Children = new ObservableCollection<TreeResultNodeViewModel>();
		// Busca el texto en los archivos
		if (!cancellationToken.IsCancellationRequested)
			await SearchFolderAsync(folder, mask.Split(';'), textSearch, caseSensitive, wholeWords, useRegex, cancellationToken);
		// Vacía el nombre de archivo que se está buscando
		FileSearched = string.Empty;
	}

	/// <summary>
	///		Busca texto en los archivos de una carpeta
	/// </summary>
	private async Task SearchFolderAsync(string folder, string[] mask, string textSearch, bool caseSensitive, bool wholeWords, bool useRegex, 
										 CancellationToken cancellationToken)
	{
		if (Directory.Exists(folder))
		{
			// Busca los archivos
			foreach (string fileName in Directory.GetFiles(folder))
				if (!cancellationToken.IsCancellationRequested && CheckExtension(fileName, mask))
				{
					List<(string text, int line, string textFound)> lines = await SearchFileAsync(fileName, textSearch, caseSensitive, wholeWords, 
																								  useRegex, cancellationToken);

						// Añade los nodos si es necesario
						if (lines.Count > 0)
							AddNodes(fileName, lines);
				}
			// Busca los directorios
			foreach (string path in Directory.GetDirectories(folder))
				if (!cancellationToken.IsCancellationRequested)
					await SearchFolderAsync(path, mask, textSearch, caseSensitive, wholeWords, useRegex, cancellationToken);
		}
	}

	/// <summary>
	///		Comprueba la extensión del archivo
	/// </summary>
	private bool CheckExtension(string fileName, string[] masks)
	{
		string extension = Path.GetExtension(fileName);

			// Comprueba si la extensión está en la máscara
			foreach (string mask in masks)
				if (extension.Equals(mask, StringComparison.CurrentCultureIgnoreCase))
					return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
	}

	/// <summary>
	///		Busca el texto de un archivo
	/// </summary>
	private async Task<List<(string text, int line, string textFound)>> SearchFileAsync(string fileName, string textSearch, bool caseSensitive, bool wholeWords, 
																						bool useRegex, CancellationToken cancellationToken)
	{
		List<(string text, int line, string textFound)> texts = new List<(string text, int line, string textFound)>();
		string [] lines = await File.ReadAllLinesAsync(fileName, cancellationToken);
		int actual = 1;
		Regex regex = GetRegEx(textSearch, caseSensitive, wholeWords, useRegex);

			// Indica el archivo buscado
			FileSearched = Path.GetFileName(fileName);
			// Busca el texto en la línea
			foreach (string line in lines)
			{
				// Busca el texto
				if (!string.IsNullOrWhiteSpace(line))
				{
					Match match = regex.Match(line);

						// Si el texto está en la línea, lo añade
						if (match.Success)
							texts.Add((line.Trim(), actual, match.Value));
				}
				// Incrementa el número de línea
				actual++;
			}
			// Devuelve las líneas de texto
			return texts;
	}

	/// <summary>
	///		Obtiene la expresión regular
	/// </summary>
        private Regex GetRegEx(string textToFind, bool caseSensitive, bool wholeWord, bool useRegex)
        {
            RegexOptions options = RegexOptions.None;

			// Añade las opciones de búsqueda
			if (!caseSensitive)
				options |= RegexOptions.IgnoreCase;
			// Obtiene el patrón
			if (useRegex)
				return new Regex(textToFind, options);
			else
			{
				string pattern = Regex.Escape(textToFind);

					// Sustituye los comodines por caracteres de expresiones regulares
					pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
					// Añade los patrones de búsqueda por palabra completa
					if (wholeWord)
						pattern = "\\b" + pattern + "\\b";
					// Devuelve la expresión regular
					return new Regex(pattern, options);
			}
        }

	/// <summary>
	///		Añade los nodos
	/// </summary>
	private void AddNodes(string fileName, List<(string text, int line, string textFound)> texts)
	{
		TreeResultNodeViewModel fileNode = new(this, null, fileName, fileName, -1, string.Empty, true, BauMvvm.ViewModels.Media.MvvmColor.Navy);

			// Añade los hijos
			foreach ((string text, int line, string textFound) in texts)
				fileNode.Children.Add(new TreeResultNodeViewModel(this, fileNode, text, fileName, line, textFound, false, BauMvvm.ViewModels.Media.MvvmColor.Black));
			// Añade el nodo
			Children.Add(fileNode);
	}

	/// <summary>
	///		Abre el archivo seleccionado
	/// </summary>
	private void OpenFile()
	{
		if (SelectedNode != null && !string.IsNullOrWhiteSpace(SelectedNode.FileName) && File.Exists(SelectedNode.FileName))
		{
			// Abre la ventana del archivo
			SearchFilesViewModel.MainViewModel.MainController.PluginsController.HostPluginsController.OpenFile(SelectedNode.FileName);
			//// Va a la línea adecuada (si estamos en el editor del archivo adecuado)
			if (SelectedNode.Line >= 0 &&
					SearchFilesViewModel.MainViewModel.MainController.PluginsController.HostPluginsController.SelectedDetailsViewModel is Base.Files.BaseTextFileViewModel fileViewModel && 
					SelectedNode.FileName.Equals(fileViewModel.FileName, StringComparison.CurrentCultureIgnoreCase))
				fileViewModel.GoToLine(SelectedNode.TextFound, SelectedNode.Line);
		}
	}

	/// <summary>
	///		ViewModel de búsqueda
	/// </summary>
	public SearchFilesViewModel SearchFilesViewModel { get; }

	/// <summary>
	///		Nodos
	/// </summary>
	public ObservableCollection<TreeResultNodeViewModel> Children 
	{ 
		get { return _children; }
		set { CheckObject(ref _children!, value); }
	}

	/// <summary>
	///		Nodo seleccionado
	/// </summary>
	public TreeResultNodeViewModel? SelectedNode
	{	
		get { return _selectedNode; }
		set { CheckObject(ref _selectedNode, value); }
	}

	/// <summary>
	///		Archivo en el que se está buscando
	/// </summary>
	public string FileSearched
	{
		get { return _fileSearched; }
		set { CheckProperty(ref _fileSearched, value); }
	}

	/// <summary>
	///		Comando para abrir un archivo
	/// </summary>
	public BaseCommand OpenCommand { get; }
}
