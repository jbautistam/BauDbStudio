using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.PluginsStudio.ViewModels.Files;

/// <summary>
///		ViewModel para un archivo de imagen
/// </summary>
public class ImageViewModel : Base.Files.BaseFileViewModel
{
	// Eventos públicos
	public event EventHandler? SaveImage;

	public ImageViewModel(PluginsStudioViewModel mainViewModel, string fileName) : base(mainViewModel.MainController.PluginsController, fileName, string.Empty)
	{
		MainViewModel = mainViewModel;
		FileName = fileName;
		IsUpdated = false;
	}

	/// <summary>
	///		Carga el archivo (en este caso se carga directamente en la vista)
	/// </summary>
	public override void Load()
	{
		// Indica que no ha habido modificaciones
		IsUpdated = false;
		// Añade el archivo a los últimos archivos abiertos
		MainViewModel.MainController.HostPluginsController.AddFileUsed(FileName);
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public override void SaveDetails(bool newName)
	{
		string oldTabId = TabId;
		string? fileName = MainViewModel.MainController.MainWindowController.DialogsController
									.OpenDialogSave(Path.GetDirectoryName(FileName),
													GetMask(), Path.GetFileName(FileName),
													Path.GetExtension(FileName));

			// Cambia el nombre de archivo
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				// Cambia el nombre de archivo
				FileName = fileName;
				SaveImage?.Invoke(this, EventArgs.Empty);
				// Actualiza la ventana
				UpdateFileName(oldTabId);
			}
			// Indica que no ha habido modificaciones
			IsUpdated = false;

		// Obtiene la máscara de archivos
		string GetMask()
		{
			string mask = string.Empty;

				// Crea la cadena de máscara
				foreach ((string file, string extension) in MainViewModel.ImageTypeFiles)
					mask = mask.AddWithSeparator($"{file} (*{extension})|*{extension}", "|", false);
				// Devuelve la máscara
				return mask;
		}
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public override void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Solución
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }
}
