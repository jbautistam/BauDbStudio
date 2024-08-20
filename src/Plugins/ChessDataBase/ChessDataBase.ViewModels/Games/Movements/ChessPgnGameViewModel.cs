namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements;

/// <summary>
///		ViewModel de la ventana principal
/// </summary>
public class ChessPgnGameViewModel : ChessGameBaseViewModel
{
	// Variables privadas
	private string _fileName = default!;
	private Pgn.PgnLibraryViewModel _pgnLibraryViewModel = default!;

	public ChessPgnGameViewModel(string fileName, string pathBoardImages, string pathPiecesImages, bool showAnimations) 
				: base(pathBoardImages, pathPiecesImages, showAnimations)
	{
		// Inicializa las propiedades
		FileName = fileName;
		// Inicializa los objetos
		PgnLibraryViewModel = new Pgn.PgnLibraryViewModel(GameBoardViewModel);
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	public async Task<(bool loaded, string error)> LoadFileAsync(CancellationToken cancellationToken) => await PgnLibraryViewModel.LoadAsync(FileName, cancellationToken);

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName 
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Librería de archivos PGN
	/// </summary>
	public Pgn.PgnLibraryViewModel PgnLibraryViewModel
	{
		get { return _pgnLibraryViewModel; }
		set { CheckObject(ref _pgnLibraryViewModel, value); }
	}
}
