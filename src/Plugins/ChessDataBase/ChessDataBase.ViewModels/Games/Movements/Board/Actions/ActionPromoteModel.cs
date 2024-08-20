using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;

/// <summary>
///		Acción de promocionar una pieza
/// </summary>
public class ActionPromoteModel : ActionBaseModel
{
	public ActionPromoteModel(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color, int toRow, int toColumn) : base(type, color)
	{
		ToRow = toRow;
		ToColumn = toColumn;
	}

	/// <summary>
	///		Fila donde se coloca la pieza promocionada
	/// </summary>
	public int ToRow { get; }

	/// <summary>
	///		Columna donde se coloca la pieza promocionada
	/// </summary>
	public int ToColumn { get; }
}
