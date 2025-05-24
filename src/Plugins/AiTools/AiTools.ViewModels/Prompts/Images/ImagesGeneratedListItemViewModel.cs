using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.AiTools.ViewModels.Prompts.Images;

/// <summary>
///		Imagen de la lista de generaciones
/// </summary>
public class ImagesGeneratedListItemViewModel : BaseObservableObject
{
	// Variables privadas
	private string _fileName = default!, _shortFileName = default!;

	public ImagesGeneratedListItemViewModel(ImagesGeneratedListViewModel imagesGeneratedListViewModel, string fileName)
	{
		ImagesGeneratedListViewModel = imagesGeneratedListViewModel;
		FileName = fileName;
		OpenCommand = new BaseCommand(_ => OpenImage());
		DeleteCommand = new BaseCommand(_ => Delete());
	}

	/// <summary>
	///		Abre la imagen
	/// </summary>
	public void OpenImage()
	{
		ImagesGeneratedListViewModel.PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.MainViewModel
				.ViewsController.HostPluginsController.OpenFile(FileName);
	}

	/// <summary>
	///		Borra un archivo de imagen
	/// </summary>
	public void Delete()
	{
		if (ImagesGeneratedListViewModel.PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.MainViewModel.ViewsController
					.SystemController.ShowQuestion($"Do you want to delete the file '{Path.GetFileName(FileName)}'?", "Accept", "Cancel"))
		{
			// Borra el archivo
			LibHelper.Files.HelperFiles.KillFile(FileName, true);
			// Elimina la imagen de la lista
			ImagesGeneratedListViewModel.Items.Remove(this);
			// Actualiza el árbol de archivos
			ImagesGeneratedListViewModel.PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.RefreshFiles();
		}
	}

	/// <summary>
	///		Lista de imágenes a la que se asocia
	/// </summary>
	public ImagesGeneratedListViewModel ImagesGeneratedListViewModel { get; }

	/// <summary>
	///		Versión
	/// </summary>
	public string FileName
	{ 
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				ShortFileName = string.Empty;
				if (!string.IsNullOrWhiteSpace(value))
					ShortFileName = Path.GetFileName(value);
			}
		}
	}

	/// <summary>
	///		Nombre corto de archivo
	/// </summary>
	public string ShortFileName
	{
		get { return _shortFileName; }
		set { CheckProperty(ref _shortFileName, value); }
	}

	/// <summary>
	///		Comando para abrir la imagen
	/// </summary>
	public BaseCommand OpenCommand { get; }

	/// <summary>
	///		Comando para borrar la imagen
	/// </summary>
	public BaseCommand DeleteCommand { get; }
}
