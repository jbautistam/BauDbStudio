namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;

/// <summary>
///		Acción para resetear el tablero
/// </summary>
public class ActionResetBoardModel : ActionBaseModel
{
	public ActionResetBoardModel() : base(Models.Pieces.PieceBaseModel.PieceType.Pawn, Models.Pieces.PieceBaseModel.PieceColor.White) {}
}
