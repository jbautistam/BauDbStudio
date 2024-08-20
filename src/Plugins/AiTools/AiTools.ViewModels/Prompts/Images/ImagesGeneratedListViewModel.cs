using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.AiTools.ViewModels.Prompts.Images;

/// <summary>
///		Lista de imágenes generadas
/// </summary>
public class ImagesGeneratedListViewModel : BaseObservableObject
{
	// Variables privadas
	private ImagesGeneratedListItemViewModel? _selectedItem;

	public ImagesGeneratedListViewModel(PromptVersionViewModel promptVersionViewModel)
	{
		PromptVersionViewModel = promptVersionViewModel;
		ContextUI = SynchronizationContext.Current ?? new SynchronizationContext();
	}

	/// <summary>
	///		Limpia los elementos
	/// </summary>
	internal void Clear()
	{
		Items.Clear();
		SelectedItem = null;
	}

	/// <summary>
	///		Carga las imágenes de una carpeta
	/// </summary>
	internal void Load()
	{
		object state = new object();

			// Carga las imágenes
			ContextUI.Send(_ => LoadImages(), state);
	}

	/// <summary>
	///		Carga las imágenes
	/// </summary>
	private void LoadImages()
	{
		string? folder = Path.GetDirectoryName(PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.PromptFile.FileName);

			// Limpia la lista
			Items.Clear();
			// Carga las imágenes
			if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
				foreach (string fileName in Directory.GetFiles(folder))
					if (IsImage(fileName) && IsVersionImage(fileName))
						Items.Add(new ImagesGeneratedListItemViewModel(this, fileName));

			// Comprueba si el nombre de archivo se corresponde con una imagen
			bool IsImage(string fileName)
			{
				return fileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase) || 
					   fileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) || 
					   fileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) || 
					   fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || 
					   fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase) || 
					   fileName.EndsWith(".tiff", StringComparison.CurrentCultureIgnoreCase) || 
					   fileName.EndsWith(".webp", StringComparison.CurrentCultureIgnoreCase);
			}
	}

	/// <summary>
	///		Comprueba si una imagen se corresponde con la versión
	/// </summary>
	private bool IsVersionImage(string fileName)
	{
		string? expectedFilePrefix = PromptVersionViewModel.GetFileImagePrefix();

			// Comprueba que el nombre de archivo se corresponda a un nombre de archivo adecuado para la versión
			if (!string.IsNullOrWhiteSpace(expectedFilePrefix))
				return Path.GetFileName(fileName).StartsWith(expectedFilePrefix, StringComparison.CurrentCultureIgnoreCase);
			// Si ha llegado hasta aquí es porque no es un archivo de esta versión
			return false;
	}

	/// <summary>
	///		ViewModel de la versión
	/// </summary>
	public PromptVersionViewModel PromptVersionViewModel { get; }

	/// <summary>
	///		Contexto de sincronización
	/// </summary>
	public SynchronizationContext ContextUI { get; }

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<ImagesGeneratedListItemViewModel> Items { get; } = new();

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public ImagesGeneratedListItemViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}
}
