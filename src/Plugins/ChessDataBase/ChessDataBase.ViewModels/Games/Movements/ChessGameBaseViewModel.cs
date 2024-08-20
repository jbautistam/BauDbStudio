namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements;

/// <summary>
///		Base para los viewModel de juegos de ajedrez
/// </summary>
public abstract class ChessGameBaseViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private Board.GameBoardViewModel _gameBoardViewModel = default!;
	private string _pathBoardImages = default!, _pathPiecesImages = default!;
	private bool _showAnimations;

	public ChessGameBaseViewModel(string pathBoardImages, string pathPiecesImages, bool showAnimations) : base(false)
	{
		// Inicializa las propiedades
		PathBoardImages = pathBoardImages;
		PathPiecesImages = pathPiecesImages;
		ShowAnimations = showAnimations;
		// Inicializa los objetos
		GameBoardViewModel = new Board.GameBoardViewModel(this);
	}

	/// <summary>
	///		ViewModel del tablero de juego
	/// </summary>
	public Board.GameBoardViewModel GameBoardViewModel
	{
		get { return _gameBoardViewModel; }
		set { CheckProperty(ref _gameBoardViewModel, value); }
	}

	/// <summary>
	///		Directorio de imágenes del tablero
	/// </summary>
	public string PathBoardImages
	{	
		get { return _pathBoardImages; }
		set { CheckProperty(ref _pathBoardImages, value); }
	}

	/// <summary>
	///		Directorio de imágenes de las piezas
	/// </summary>
	public string PathPiecesImages
	{	
		get { return _pathPiecesImages; }
		set { CheckProperty(ref _pathPiecesImages, value); }
	}

	/// <summary>
	///		Indica si se deben mostrar las animaciones
	/// </summary>
	public bool ShowAnimations
	{
		get { return _showAnimations; }
		set { CheckProperty(ref _showAnimations, value); }
	}
}
