namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements;

/// <summary>
///		ViewModel de la ventana principal
/// </summary>
public class ChessGameViewModel : ChessGameBaseViewModel
{
	public ChessGameViewModel(string pathBoardImages, string pathPiecesImages, bool showAnimations) : base(pathBoardImages, pathPiecesImages, showAnimations)
	{
	}

	/// <summary>
	///		Inicializa el tablero
	/// </summary>
	public void Init()
	{
		GameBoardViewModel.LoadMovements(null);
	}
}
