using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;

/// <summary>
///		Clase con los datos de una acción
/// </summary>
public abstract class ActionBaseModel
{
	protected ActionBaseModel(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color)
	{
		Type = type;
		Color = color;
	}

	/// <summary>
	///		Tipo de pieza
	/// </summary>
	public PieceBaseModel.PieceType Type { get; }

	/// <summary>
	///		Color de la pieza
	/// </summary>
	public PieceBaseModel.PieceColor Color { get; }
}
