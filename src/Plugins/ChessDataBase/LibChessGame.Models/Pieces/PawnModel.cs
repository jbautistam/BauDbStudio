using System;

using Bau.Libraries.ChessDataBase.Models.Board;

namespace Bau.Libraries.ChessDataBase.Models.Pieces
{
	/// <summary>
	///		Peón
	/// </summary>
	public class PawnModel : PieceBaseModel
	{
		public PawnModel(PieceColor color) : base(color, PieceType.Pawn) {}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public override PieceBaseModel Clone()
		{
			PawnModel target = new PawnModel(Color);

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
			int rowDiference = fromRow - toRow;
			int columnDiference = fromColumn - toColumn;
			bool can = false;

				// Comprueba si se puede mover
				if (Math.Abs(columnDiference) <= 1) // sólo se puede mover como máximo una columna a la izquierda
				{
					// Las blancas sólo se pueden mover hacia arriba y las negras sólo se pueden mover hacia abajo
					if ((Math.Sign(rowDiference) == 1 && Color == PieceColor.White) ||
							(Math.Sign(rowDiference) == -1 && Color == PieceColor.Black))
						{
							// Se puede mover dos filas: si no se ha movido y sólo ha ido en la misma columna o es una captura in Passant
							if (Math.Abs(rowDiference) == 2 && !IsMoved)
							{
								if (columnDiference == 0) // ... movimiento normal
									can = board.IsPathEmpty(fromRow, fromColumn, toRow, toColumn); // ... si están vacías las celdas entre una y otra
							}
							else if (Math.Abs(rowDiference) == 1)
							{
								if (columnDiference == 0)
									can = board.IsEmpty(toRow, toColumn);
								else
									can = board.CanCapture(this, toRow, toColumn);
							}
						}
					}
				// Devuelve el valor que indica si se puede mover
				return can;
		}
	}
}
