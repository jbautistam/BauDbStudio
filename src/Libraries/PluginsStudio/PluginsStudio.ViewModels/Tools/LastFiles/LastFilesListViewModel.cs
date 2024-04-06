using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.LastFiles;

/// <summary>
///		Lista de últimos archivos abiertos
/// </summary>
public class LastFilesListViewModel : BaseObservableObject
{
	// Variables privadas
	private AsyncObservableCollection<LastFileViewModel> _files = default!;

	public LastFilesListViewModel(PluginsStudioViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
		Files = new AsyncObservableCollection<LastFileViewModel>();
	}

	/// <summary>
	///		Añade un archivo
	/// </summary>
	public void Add(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			string[] parts = fileName.Split(';', StringSplitOptions.TrimEntries);
			List<string> files = new();

				// Ordena los archivos al revés (porque queremos los más antiguos arriba porque se insertan siempre en la posición 0)
				for (int index = parts.Length - 1; index >= 0; index--)
					files.Add(parts[index]);
				// Añade los nombres de archivos al inicio
				foreach (string part in files)
					if (!string.IsNullOrWhiteSpace(part) && File.Exists(part))
					{
						LastFileViewModel? existing = Files.FirstOrDefault(item => item.FileName.Equals(part, StringComparison.CurrentCultureIgnoreCase));
						
							// Quita el archivo existente
							if (existing is not null)
								Files.Remove(existing);
							// Añade el archivo al principio
							Files.Insert(0, new LastFileViewModel(MainViewModel, part, 
																  MainViewModel.MainController.HostPluginsController.GetIcon(part), 
																  0)
										);
					}
				// Elimina los archivos sobrantes (deja 10 como máximo)
				while (Files.Count > 10)
					Files.RemoveAt(Files.Count - 1);
				// Asigna los índices
				for (int index = 0; index < Files.Count; index++)
					Files[index].Index = index + 1;
		}
	}

	/// <summary>
	///		Obtiene los archivos
	/// </summary>
	public string GetFiles()
	{
		string files = string.Empty;

			// Añade los archivos
			foreach (LastFileViewModel file in Files)
				files = files.AddWithSeparator(file.FileName, ";");
			// Devuelve la lista de archivos
			return files;
	}

	/// <summary>
	///		ViewModel de la ventana principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Archivos
	/// </summary>
	public AsyncObservableCollection<LastFileViewModel> Files
	{
		get { return _files; }
		set { CheckObject(ref _files, value); }
	}
}
