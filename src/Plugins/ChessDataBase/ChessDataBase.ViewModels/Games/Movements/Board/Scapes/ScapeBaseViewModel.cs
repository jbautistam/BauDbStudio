namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes;

/// <summary>
///		Base para los escaques del tablero
/// </summary>
public class ScapeBaseViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	public int _row, _column;
	public Models.Pieces.PieceBaseModel.PieceColor _color;

	public ScapeBaseViewModel(int row, int column, Models.Pieces.PieceBaseModel.PieceColor color)
	{
		Row = row;
		Column = column;
		Color = color;
	}

	/// <summary>
	///		Fila
	/// </summary>
	public int Row
	{
		get { return _row; }
		set { CheckProperty(ref _row, value); }
	}

	/// <summary>
	///		Columna
	/// </summary>
	public int Column
	{
		get { return _column; }
		set { CheckProperty(ref _column, value); }
	}

	/// <summary>
	///		Color
	/// </summary>
	public Models.Pieces.PieceBaseModel.PieceColor Color
	{
		get { return _color; }
		set { CheckProperty(ref _color, value); }
	}
}
