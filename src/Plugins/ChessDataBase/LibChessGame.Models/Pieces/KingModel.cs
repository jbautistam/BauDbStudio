using System;

using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;

namespace Bau.Libraries.ChessDataBase.Models.Pieces
{
	/// <summary>
	///		Rey
	/// </summary>
	public class KingModel : PieceBaseModel
	{
		public KingModel(PieceColor color) : base(color, PieceType.King) {}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public override PieceBaseModel Clone()
		{
			KingModel target = new KingModel(Color);

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
			// Comprueba si es un movimiento horizontal / diagonal de un solo recuadro
			if (IsVerticalHorizontalMovement(fromRow, fromColumn, toRow, toColumn, true) || 
					IsDiagonalMovement(fromRow, fromColumn, toRow, toColumn, true))
				return board.IsLegalMoveTo(this, toRow, toColumn);
			// Si ha llegado hasta aquí es porque el movimiento no era legal
			return false;
		}
	}
}
