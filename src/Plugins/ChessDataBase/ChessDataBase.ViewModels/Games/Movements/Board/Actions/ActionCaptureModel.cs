using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;

/// <summary>
///		Acción para eliminar una pieza
/// </summary>
public class ActionCaptureModel : ActionBaseModel
{
	public ActionCaptureModel(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color, int targetRow, int targetColumn) : base(type, color)
	{
		TargetRow = targetRow;
		TargetColumn = targetColumn;
	}

	/// <summary>
	///		Fila en la que se encuentra el elemento a capturar
	/// </summary>
	public int TargetRow { get; }

	/// <summary>
	///		Columna en la que se encuentra el elemento a capturar
	/// </summary>
	public int TargetColumn { get; }
}
