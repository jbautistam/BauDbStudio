using System;

using Bau.Libraries.ChessDataBase.Models.Board;

namespace Bau.Libraries.ChessDataBase.Models.Pieces
{
	/// <summary>
	///		Alfil
	/// </summary>
	public class BishopModel : PieceBaseModel
	{
		public BishopModel(PieceColor color) : base(color, PieceType.Bishop) {}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public override PieceBaseModel Clone()
		{
			BishopModel target = new BishopModel(Color);

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
			// Comprueba si puede mover
			if (IsDiagonalMovement(fromRow, fromColumn, toRow, toColumn, false))
				return board.IsPathEmpty(fromRow, fromColumn, toRow, toColumn) && board.IsLegalMoveTo(this, toRow, toColumn);
			// Devuelve el valor que indica si puede mover
			return false;
		}
	}
}
