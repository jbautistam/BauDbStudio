using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;

/// <summary>
///		Movimiento de una pieza
/// </summary>
public class ActionMoveModel : ActionBaseModel
{
	public ActionMoveModel(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color,
						   int fromRow, int fromColumn, int toRow, int toColumn) : base(type, color)
	{
		FromRow = fromRow;
		FromColumn = fromColumn;
		ToRow = toRow;
		ToColumn = toColumn;
	}

	/// <summary>
	///		Fila desde la que se mueve la pieza
	/// </summary>
	public int FromRow { get; }

	/// <summary>
	///		Columna desde la que se mueve la pieza
	/// </summary>
	public int FromColumn { get; }

	/// <summary>
	///		Fila donde se mueve la pieza
	/// </summary>
	public int ToRow { get; }

	/// <summary>
	///		Columna donde se mueve la pieza
	/// </summary>
	public int ToColumn { get; }

}
