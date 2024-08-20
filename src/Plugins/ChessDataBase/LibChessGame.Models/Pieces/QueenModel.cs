using System;

using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;

namespace Bau.Libraries.ChessDataBase.Models.Pieces
{
	/// <summary>
	///		Reina
	/// </summary>
	public class QueenModel : PieceBaseModel
	{
		public QueenModel(PieceColor color) : base(color, PieceType.Queen) {}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public override PieceBaseModel Clone()
		{
			QueenModel target = new QueenModel(Color);

				// Asigna los datos privados
				CloneInner(target);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Comprueba si la pieza se puede mover a una fila / columna
		/// </summary>
		internal override bool CanMoveTo(SquareModel board, int fromRow, int fromColumn, int toRow, int toColumn)
		{
			// Comprueba si es un movimiento horizontal / diagonal
			if (IsVerticalHorizontalMovement(fromRow, fromColumn, toRow, toColumn, false) || 
					IsDiagonalMovement(fromRow, fromColumn, toRow, toColumn, false))
				return board.IsPathEmpty(fromRow, fromColumn, toRow, toColumn) && board.IsLegalMoveTo(this, toRow, toColumn);
			// Si ha llegado hasta aquí es porque el movimiento no era legal
			return false;
		}
	}
}
