using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.JobsProcessor.ViewModel;

/// <summary>
///		ViewModel principal del procesador de trabajos
/// </summary>
public class JobsProcessorViewModel : BaseObservableObject
{
	public JobsProcessorViewModel(Controllers.IJobsProcessorController mainController)
	{
		// Asigna el controlador de vistas
		MainController = mainController;
		// Inicializa los comandos
		ExecuteFileCommand = new BaseCommand(ExecuteFolderFile);
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Carga el directorio
	/// </summary>
	public void Load(string path)
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Abre un archivo (no hace nada, sólo implementa la interface)
	/// </summary>
	public bool OpenFile(string fileName) => false;

	/// <summary>
	///		Ejecuta el script de un archivo o una carpeta
	/// </summary>
	private void ExecuteFolderFile(object? parameter)
	{
		if (parameter is string fileName && !string.IsNullOrWhiteSpace(fileName) &&
				fileName.EndsWith(".cmd.xml", StringComparison.CurrentCultureIgnoreCase))
			MainController.OpenWindow(new Processor.ExecuteConsoleViewModel(this, fileName));
	}

	/// <summary>
	///		Controlador de vistas principal
	/// </summary>
	public Controllers.IJobsProcessorController MainController { get; }

	/// <summary>
	///		Comando para ejecutar un archivo
	/// </summary>
	public BaseCommand ExecuteFileCommand { get; }
}
