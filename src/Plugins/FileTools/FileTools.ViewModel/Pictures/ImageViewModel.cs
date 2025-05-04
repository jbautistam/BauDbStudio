using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Files;

namespace Bau.Libraries.FileTools.ViewModel.Pictures;

/// <summary>
///		ViewModel para un archivo de imagen
/// </summary>
public class ImageViewModel : BaseFileViewModel
{
	// Eventos públicos
	public event EventHandler? SaveImage;

	public ImageViewModel(FileToolsViewModel mainViewModel, string fileName) : base(mainViewModel.MainController.PluginController, fileName, string.Empty)
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
		// Añade el archivo a los últimos archivos abiertos
		MainViewModel.MainController.HostPluginsController.AddFileUsed(FileName);
		// Indica que no ha habido modificaciones
		IsUpdated = false;
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
	///		Ejecuta un comando
	/// </summary>
	public override void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
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
	public FileToolsViewModel MainViewModel { get; }
}
