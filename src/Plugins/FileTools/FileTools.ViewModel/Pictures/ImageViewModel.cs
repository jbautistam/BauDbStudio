using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.FileTools.ViewModel.Pictures;

/// <summary>
///		ViewModel para la edición de un archivo de imagen
/// </summary>
public class ImageViewModel : PluginsStudio.ViewModels.Base.Files.BaseFileViewModel
{
	/// <summary>
	///		Tipo de herramienta
	/// </summary>
	private enum ToolType
	{
		/// <summary>Desconocido, no se debería utilizar</summary>
		Unknown,
		/// <summary>División de archivos</summary>
		SplitFiles
	}
	// Eventos públicos
	public event EventHandler? SaveImage;
	// Variables privadas
	private ComboViewModel _comboTools = default!;

	public ImageViewModel(FileToolsViewModel mainViewModel, string fileName) : base(mainViewModel.MainController.PluginController, fileName, string.Empty)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		// Crea las listas
		ComboTools = new ComboViewModel(this);
		ComboTools.PropertyChanged += async (sender, args) => {
																if (args.PropertyName.EqualsIgnoreNull(nameof(ComboTools.SelectedItem)))
																	await ExecuteToolAsync(CancellationToken.None);
															  };
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Carga el archivo (en este caso se carga directamente en la vista)
	/// </summary>
	public override void Load()
	{
		// Inicializa los combos
		LoadComboTools();
		// Indica que no ha habido modificaciones
		IsUpdated = false;
		// Añade el archivo a los últimos archivos abiertos
		MainViewModel.MainController.HostPluginsController.AddFileUsed(FileName);
	}

	/// <summary>
	///		Carga el combo de herramientas
	/// </summary>
	private void LoadComboTools()
	{
		// Limpia el combo
		ComboTools.Items.Clear();
		// Añade los separadores básicos
		ComboTools.AddItem((int) ToolType.Unknown, "<Select a tool>");
		ComboTools.AddItem((int) ToolType.SplitFiles, "Split files");
		// Selecciona la herramienta
		ComboTools.SelectedIndex = 0;
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
	///		Ejecuta una herramienta
	/// </summary>
	private async Task ExecuteToolAsync(CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Ejecuta una herramienta
		switch ((ToolType) (ComboTools.SelectedId ?? 0))
		{
			case ToolType.SplitFiles:
					MainViewModel.MainController.OpenDialog(new Tools.SplitImagesViewModel(this));
				break;
		}
		// Selecciona el primer elemento
		ComboTools.SelectedId = (int) ToolType.Unknown;
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public override void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public FileToolsViewModel MainViewModel { get; }

	/// <summary>
	///		Heramientas
	/// </summary>
	public ComboViewModel ComboTools
	{
		get { return _comboTools; }
		set { CheckObject(ref _comboTools!, value); }
	}
}
