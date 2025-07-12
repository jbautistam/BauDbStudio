using Bau.Libraries.AiTools.Application.Models.Prompts;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.AiTools.ViewModels.Prompts;

/// <summary>
///		ViewModel del archivo del prompt
/// </summary>
public class PromptFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Constantes privadas
	private const string DefaultFileName = $"NewFile{AiToolsViewModel.ExtensionAiImageFile}";
	private const string Mask = $"Prompt image files (*{AiToolsViewModel.ExtensionAiImageFile})|*{AiToolsViewModel.ExtensionAiImageFile}|All files (*.*)|*.*";
	// Eventos públicos
	public event EventHandler<string>? CopiedText;
	// Variables privadas
	private string _fileName = default!, _name = default!, _description = default!;
	private PromptVersionListViewModel _versionsViewModel = default!;

	public PromptFileViewModel(AiToolsViewModel mainViewModel, string fileName)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		Header = Path.GetFileName(fileName);
		// Asigna los objetos
		PromptGenerator = new Application.PromptGenerator();
		VersionsViewModel = new PromptVersionListViewModel(this);
		TreeCategoriesViewModel = new Explorers.TreeCategoriesViewModel(this);
		// Asigna el manejador del evento de modificaciones
		VersionsViewModel.PropertyChanged += (sender, args) => {
																	if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																			args.PropertyName.Equals(nameof(IsUpdated)))
																		IsUpdated = true;
															   };
		// Asigna los comandos
		NewVersionCommand = new BaseCommand(_ => VersionsViewModel.Add());
		DeleteVersionCommand = new BaseCommand(_ => VersionsViewModel.Delete());
		CompileCommand = new BaseCommand(async _ => await VersionsViewModel.CompileAsync(CancellationToken.None));
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	public async Task LoadAsync(string categoriesFileName, CancellationToken cancellationToken)
	{
		// Inicializa el manager de imágenes
		await MainViewModel.AiImageGenerationManager.InitializeAsync(cancellationToken);
		// Carga el árbol de categorías
		TreeCategoriesViewModel.Load(categoriesFileName);
		// Carga el archivo
		PromptFile = PromptGenerator.LoadFile(FileName);
		if (PromptFile.Prompts.Count == 0)
			PromptFile.Prompts.Add(new PromptModel(string.Empty)
											{
												Version = 1
											}
								  );
		// Asigna las propiedades
		Name = PromptFile.Name;
		Description = PromptFile.Description;
		// Carga los prompts
		VersionsViewModel.Clear();
		foreach (PromptModel prompt in PromptFile.Prompts)
			VersionsViewModel.Add(prompt);
		VersionsViewModel.SelectLast();
		// Indica que no ha habido modificaciones
		IsUpdated = false;
		VersionsViewModel.IsUpdated = false;
	}

	/// <summary>
	///		Carga las imágenes
	/// </summary>
	internal void LoadImages()
	{
		VersionsViewModel.LoadImages();
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "Do you want to save the file before continuing?";
		else
			return $"Do you want to save the file '{Path.GetFileName(FileName)}' before continuing?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		string? fileName;

			// Si hay que cambiar el nombre de archivo
			if (PromptFile is null || string.IsNullOrWhiteSpace(PromptFile.FileName) || newName)
				fileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave(null, Mask, DefaultFileName, AiToolsViewModel.ExtensionAiImageFile);
			else
				fileName = PromptFile.FileName;
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(fileName))
				Save(fileName);
	}

	/// <summary>
	///		Graba un archivo
	/// </summary>
	private void Save(string fileName)
	{
		// Vuelca los datos
		PromptFile = new PromptFileModel(fileName);
		PromptFile.Name = Name;
		PromptFile.Description = Description;
		// Crea el item
		foreach (PromptVersionViewModel prompt in VersionsViewModel.Items)
			PromptFile.Prompts.Add(prompt.GetPrompt());
		// Graba el archivo
		PromptGenerator.SaveFile(fileName, PromptFile);
		// Indica que no ha habido modificaciones
		IsUpdated = false;
		VersionsViewModel.IsUpdated = false;
	}

	/// <summary>
	///		Actualiza el explorador de archivos
	/// </summary>
	internal void RefreshFiles()
	{
		MainViewModel.ViewsController.PluginController.HostPluginsController.RefreshFiles();
	}

	/// <summary>
	///		Cierra la ventana de detalles: limpia las versiones para que se descarguen las imágenes que pueda haber en memoria
	/// </summary>
	public void Close()
	{
		VersionsViewModel.Clear();
	}

	/// <summary>
	///		Lanza el evento de texto seleccionado en las categorías
	/// </summary>
	internal void RaiseCategoryTextEvent(string text)
	{
		CopiedText?.Invoke(this, text);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public AiToolsViewModel MainViewModel { get; }

	/// <summary>
	///		Generador de prompts
	/// </summary>
	public Application.PromptGenerator PromptGenerator { get; }

	/// <summary>
	///		Archivo de prompt cargado
	/// </summary>
	public PromptFileModel PromptFile { get; private set; } = default!;

	/// <summary>
	///		ViewModel del árbol de categorías
	/// </summary>
	public Explorers.TreeCategoriesViewModel TreeCategoriesViewModel { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Nombre del prompt
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción del prompt
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}
	
	/// <summary>
	///		Lista de versiones
	/// </summary>
	public PromptVersionListViewModel VersionsViewModel
	{
		get { return _versionsViewModel; }
		set { CheckObject(ref _versionsViewModel!, value); }
	}

	/// <summary>
	///		Comando para crear una nueva versión
	/// </summary>
	public BaseCommand NewVersionCommand { get; }

	/// <summary>
	///		Comando para borrar una versión
	/// </summary>
	public BaseCommand DeleteVersionCommand { get; }

	/// <summary>
	///		Comando de compilación
	/// </summary>
	public BaseCommand CompileCommand { get; }
}