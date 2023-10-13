using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;

/// <summary>
///		Factory para creación de una lista de movimientos
/// </summary>
public class ActionFactory
{
	/// <summary>
	///		Crea una lista de acciones basada en un movimiento
	/// </summary>
	public List<ActionBaseModel> Create(MovementFigureModel movement)
	{
		List<ActionBaseModel> actions = new();

			// Añade las acciones
			switch (movement.Type)
			{
				case MovementFigureModel.MovementType.CastleKingSide:
				case MovementFigureModel.MovementType.CastleQueenSide:
						actions.AddRange(CreateCastleKingActions(movement));
					break;
				default:
						// Crea el movimiento
						actions.Add(CreateMoveAction(movement.OriginPiece, movement.Color, movement.From, movement.To));
						// Añade la captura
						if (movement.Type == MovementFigureModel.MovementType.Capture)
						{
							if (movement.CapturedEnPassant != null)
								actions.Add(CreateCaptureAction(movement.Captured.Type, movement.Captured.Color, movement.CapturedEnPassant));
							else if (movement.Captured != null)
								actions.Add(CreateCaptureAction(movement.Captured.Type, movement.Captured.Color, movement.To));
							else // esto no debería pasar nunca
								actions.Add(CreateMoveAction(movement.OriginPiece, movement.Color, movement.From, movement.To));
						}
						// Crea la promoción
						if (movement.PromotedPiece != null)
						{
							// Elimina el peón
							actions.Add(CreateCaptureAction(PieceBaseModel.PieceType.Pawn, movement.Color, movement.To));
							// Crea la promoción
							actions.Add(CreatePromoteAction(movement.PromotedPiece ?? PieceBaseModel.PieceType.Queen, movement.Color, movement.To));
						}
					break;
			}
			// Devuelve las acciones
			return actions;
	}

	/// <summary>
	///		Crea una lista de acciones para deshacer un movimiento
	/// </summary>
	public List<ActionBaseModel> CreateUndo(MovementFigureModel movement)
	{
		List<ActionBaseModel> actions = new List<ActionBaseModel>();

			// Añade las acciones
			if (movement != null)
				switch (movement.Type)
				{
					case MovementFigureModel.MovementType.CastleKingSide:
					case MovementFigureModel.MovementType.CastleQueenSide:
							actions.AddRange(CreateUndoCastleKingActions(movement));
						break;
					default:
							actions.AddRange(CreateUndoMoveActions(movement));
						break;
				}
			// Devuelve las acciones
			return actions;
	}

	/// <summary>
	///		Crea las acciones de enroque
	/// </summary>
	private List<ActionBaseModel> CreateCastleKingActions(MovementFigureModel movement)
	{
		List<ActionBaseModel> actions = new List<ActionBaseModel>();
		int row = movement.Color == PieceBaseModel.PieceColor.White ? 7 : 0;

			// Enroque
			if (movement.Type == MovementFigureModel.MovementType.CastleKingSide) // enroque corto
			{
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.Rook, movement.Color, new CellModel(row, 7), new CellModel(row, 5)));
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.King, movement.Color, new CellModel(row, 4), new CellModel(row, 6)));
			}
			else // enroque largo
			{
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.Rook, movement.Color, new CellModel(row, 0), new CellModel(row, 3)));
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.King, movement.Color, new CellModel(row, 4), new CellModel(row, 2)));
			}
			// Devuelve las acciones
			return actions;
	}

	/// <summary>
	///		Crea las acciones de deshacer enroque
	/// </summary>
	private List<ActionBaseModel> CreateUndoCastleKingActions(MovementFigureModel movement)
	{
		List<ActionBaseModel> actions = new List<ActionBaseModel>();
		int row = movement.Color == PieceBaseModel.PieceColor.White ? 7 : 0;

			// Enroque
			if (movement.Type == MovementFigureModel.MovementType.CastleKingSide) // enroque corto
			{
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.Rook, movement.Color, new CellModel(row, 5), new CellModel(row, 7)));
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.King, movement.Color, new CellModel(row, 6), new CellModel(row, 4)));
			}
			else // enroque largo
			{
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.Rook, movement.Color, new CellModel(row, 3), new CellModel(row, 0)));
				actions.Add(CreateMoveAction(PieceBaseModel.PieceType.King, movement.Color, new CellModel(row, 2), new CellModel(row, 4)));
			}
			// Devuelve las acciones
			return actions;
	}

	/// <summary>
	///		Crea las acciones necesarias para deshacer un movimiento
	/// </summary>
	private List<ActionBaseModel> CreateUndoMoveActions(MovementFigureModel movement)
	{
		List<ActionBaseModel> actions = new List<ActionBaseModel>();

			// Deshace la promoción
			if (movement.PromotedPiece != null)
			{
				// Elimina la pieza promocionada
				actions.Add(CreateCaptureAction(movement.PromotedPiece ?? PieceBaseModel.PieceType.Queen, movement.Color, movement.To));
				// Crea de nuevo el peón en el mismo lugar que la pieza promocionada
				actions.Add(CreatePromoteAction(PieceBaseModel.PieceType.Pawn, movement.Color, movement.To));
			}
			// Deshace el movimiento
			actions.Add(CreateMoveAction(movement.OriginPiece, movement.Color, movement.To, movement.From));
			// Crea de nuevo la pieza capturada
			if (movement.Type == MovementFigureModel.MovementType.Capture)
			{
				if (movement.CapturedEnPassant != null)
					actions.Add(CreatePromoteAction(movement.Captured.Type, movement.Captured.Color, movement.CapturedEnPassant));
				else if (movement.Captured != null)
					actions.Add(CreatePromoteAction(movement.Captured.Type, movement.Captured.Color, movement.To));
				else // ... esto no debería pasar nunca
				{
					//actions.Add(CreateCaptureAction(movement.OriginPiece, movement.Color, movement.To));
					actions.Add(CreatePromoteAction(movement.OriginPiece, movement.Color, movement.To));
				}
			}
			// Devuelve las accioens
			return actions;
	}

	/// <summary>
	///		Crea un movimiento
	/// </summary>
	private ActionMoveModel CreateMoveAction(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color, CellModel from, CellModel to)
	{
		return new ActionMoveModel(type, color, from.Row, from.Column, to.Row, to.Column);
	}

	/// <summary>
	///		Crea una captura
	/// </summary>
	private ActionCaptureModel CreateCaptureAction(PieceBaseModel.PieceType pieceCaptured, PieceBaseModel.PieceColor colorCaptured, CellModel target)
	{
		return new ActionCaptureModel(pieceCaptured, colorCaptured, target.Row, target.Column);
	}

	/// <summary>
	///		Crea un promoción
	/// </summary>
	private ActionPromoteModel CreatePromoteAction(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color, CellModel target)
	{
		return new ActionPromoteModel(type, color, target.Row, target.Column);
	}
}