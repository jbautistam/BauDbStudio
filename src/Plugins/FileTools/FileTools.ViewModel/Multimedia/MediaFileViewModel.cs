using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.FileTools.ViewModel.Multimedia;

/// <summary>
///		ViewModel para ver el contenido de un archivo multimedia
/// </summary>
public class MediaFileViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Eventos públícos
	public EventHandler<EventArgs>? PlayFile;
	public EventHandler<EventArgs>? PauseFile;
	public EventHandler<EventArgs>? StopFile;
	// Variables privadas
	private string _fileName = string.Empty, _shortFileName = string.Empty;
	private bool _isAudio, _isPlaying, _overAllWindows;

	public MediaFileViewModel(FileToolsViewModel mainViewModel, string fileName, bool isAudio)
	{ 
		// Asigna los objetos
		MainViewModel = mainViewModel;
		// Asigna las propiedades
		FileName = fileName;
		if (!string.IsNullOrWhiteSpace(FileName))
			ShortFileName = Path.GetFileName(fileName);
		else
			ShortFileName = "No file";
		IsAudio = isAudio;
		IsPlaying = false;
		OverAllWindows = true;
		// Asigna los comandos
		PlayCommand = new BaseCommand(_ => Play(), _ => CanPlay())
								.AddListener(this, nameof(IsPlaying));
		PauseCommand = new BaseCommand(_ => Pause(), _ => CanStop())
								.AddListener(this, nameof(IsPlaying));
		StopCommand = new BaseCommand(_ => Stop(), _ => CanStop())
								.AddListener(this, nameof(IsPlaying));
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Indica si se puede activar la visualización
	/// </summary>
	private bool CanPlay() => !string.IsNullOrWhiteSpace(FileName) && File.Exists(FileName) && !IsPlaying;

	/// <summary>
	///		Indica si se puede detener la visualización
	/// </summary>
	private bool CanStop() => IsPlaying;

	/// <summary>
	///		Comienza la ejecución del archivo multimedia
	/// </summary>
	public void Play()
	{
		// Indica que se está ejecutando
		IsPlaying = true;
		// Lanza el evento
		PlayFile?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Comienza la ejecución del archivo multimedia
	/// </summary>
	public void Pause()
	{
		// Indica que ya no se está ejecutando
		IsPlaying = false;
		// Lanza el evento
		PauseFile?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Detiene la ejecución del archivo multimedia
	/// </summary>
	public void Stop()
	{
		// Indica que ya no se está ejecutando
		//? Tiene que ir antes de llamar al evento stop
		IsPlaying = false;
		// Lanza el evento
		StopFile?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public FileToolsViewModel MainViewModel { get; set; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string ShortFileName
	{
		get { return _shortFileName; }
		set { CheckProperty(ref _shortFileName, value); }
	}

	/// <summary>
	///		Indica si se está visualizando el archivo
	/// </summary>
	public bool IsPlaying
	{
		get { return _isPlaying; }
		set { CheckProperty(ref _isPlaying, value); }
	}

	/// <summary>
	///		Indica si es un archivo de audio
	/// </summary>
	public bool IsAudio
	{
		get { return _isAudio; }
		set { CheckProperty(ref _isAudio, value); }
	}

	/// <summary>
	///		Indica si se debe mostrar sobre todas las ventanas
	/// </summary>
	public bool OverAllWindows
	{
		get { return _overAllWindows; }
		set { CheckObject(ref _overAllWindows, value); }
	}

	/// <summary>
	///		Comando para activar el archivo multimedia
	/// </summary>
	public BaseCommand PlayCommand { get; }

	/// <summary>
	///		Comando para pausar el archivo multimedia
	/// </summary>
	public BaseCommand PauseCommand { get; }

	/// <summary>
	///		Comando para detener el archivo multimedia
	/// </summary>
	public BaseCommand StopCommand { get; }
}
