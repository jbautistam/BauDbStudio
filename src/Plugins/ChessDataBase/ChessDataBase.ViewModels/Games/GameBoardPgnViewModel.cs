namespace Bau.Libraries.ChessDataBase.ViewModels.Games;

/// <summary>
///		ViewModel para mostrar un archivo PGN
/// </summary>
public class GameBoardPgnViewModel : BauMvvm.ViewModels.BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Eventos públicos
	public event EventHandler? Closed;
	// Variables privadas
	private Movements.ChessPgnGameViewModel _chessGameViewModel = default!;

	public GameBoardPgnViewModel(MainViewModel mainViewModel, string fileName) : base(false)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		Header = Path.GetFileName(fileName);
		ChessGameViewModel = new Movements.ChessPgnGameViewModel(fileName, MainViewModel.ConfigurationViewModel.PathBoardImages,
																 MainViewModel.ConfigurationViewModel.PathPiecesImages,
																 mainViewModel.ConfigurationViewModel.ShowAnimations);
		// Asigna los comandos
		OpenFileCommand = new BauMvvm.ViewModels.BaseCommand(_ => OpenFile());
	}

	/// <summary>
	///		Abre el archivo en modo de texto
	/// </summary>
	private void OpenFile()
	{
		MainViewModel.ViewsController.HostPluginsController.OpenTextEditor(ChessGameViewModel.FileName, "Pgn file (*.pgn)|*.pgn");
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Cierra el viewModel
	/// </summary>
	public void Close()
	{
		Closed?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Obtiene el mensaje de grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage() => string.Empty;

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// No hace nada aún
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public MainViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera de la ficha
	/// </summary>
	public string Header { get; }

	/// <summary>
	///		ViewModel del juego
	/// </summary>
	public Movements.ChessPgnGameViewModel ChessGameViewModel
	{
		get { return _chessGameViewModel; }
		set { CheckObject(ref _chessGameViewModel, value); }
	}

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => $"PgnView_{ChessGameViewModel.FileName}";

	/// <summary>
	///		Comando para abrir un archivo
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand OpenFileCommand { get; }
}
