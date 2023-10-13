namespace Bau.Libraries.ChessDataBase.ViewModels.Games;

/// <summary>
///		ViewModel para mostrar un archivo PGN
/// </summary>
public class GameBoardViewModel : BauMvvm.ViewModels.BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Eventos públicos
	public event EventHandler? Closed;
	// Variables privadas
	private Movements.ChessGameViewModel _chessGameViewModel = default!;

	public GameBoardViewModel(MainViewModel mainViewModel) : base(false)
	{
		MainViewModel = mainViewModel;
		Header = "Game";
		ChessGameViewModel = new Movements.ChessGameViewModel(MainViewModel.ConfigurationViewModel.PathBoardImages,
															  MainViewModel.ConfigurationViewModel.PathPiecesImages,
															  mainViewModel.ConfigurationViewModel.ShowAnimations);
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
	public Movements.ChessGameViewModel ChessGameViewModel
	{
		get { return _chessGameViewModel; }
		set { CheckObject(ref _chessGameViewModel, value); }
	}

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => $"GameView_{Guid.NewGuid().ToString()}";
}
