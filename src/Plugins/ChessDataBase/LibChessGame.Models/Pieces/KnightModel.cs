using System;

using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;

namespace Bau.Libraries.ChessDataBase.Models.Pieces
{
	/// <summary>
	///		Caballo
	/// </summary>
	public class KnightModel : PieceBaseModel
	{
		public KnightModel(PieceColor color) : base(color, PieceType.Knight) {}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public override PieceBaseModel Clone()
		{
			KnightModel target = new KnightModel(Color);

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
			int rowDifference = Math.Abs(fromRow - toRow);
			int columnDifference = Math.Abs(fromColumn - toColumn);

				// Puede moverse si se desplaza dos celdas en una dirección y una en otra
				if ((rowDifference == 2 && columnDifference == 1) ||
						(rowDifference == 1 && columnDifference == 2))
					return board.IsLegalMoveTo(this, toRow, toColumn);
				// Devuelve el valor que indica que no se puede
				return false;
		}
	}
}
