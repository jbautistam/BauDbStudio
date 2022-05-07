using System;

using Bau.Libraries.ChessDataBase.Models.Board;

namespace Bau.Libraries.ChessDataBase.Models.Pieces
{
	/// <summary>
	///		Clase base con los datos de una pieza
	/// </summary>
	public abstract class PieceBaseModel
	{
		/// <summary>
		///		Tipo de pieza
		/// </summary>
		public enum PieceType
		{
			/// <summary>Desconocido. No debería ocurrir</summary>
			Unknown,
			/// <summary>Peón</summary>
			Pawn,
			/// <summary>Torre</summary>
			Rook,
			/// <summary>Alfil</summary>
			Bishop,
			/// <summary>Caballo</summary>
			Knight,
			/// <summary>Rey</summary>
			King,
			/// <summary>Dama</summary>
			Queen
		}

		/// <summary>
		///		Color de la pieza
		/// </summary>
		public enum PieceColor
		{
			/// <summary>Blanca</summary>
			White,
			/// <summary>Negra</summary>
			Black
		}

		protected PieceBaseModel(PieceColor color, PieceType type)
		{
			Color = color;
			Type = type;
			IsMoved = false;
		}

		/// <summary>
		///		Comprueba si la pieza se puede mover a una fila / columna
		/// </summary>
		internal abstract bool CanMoveTo(SquareModel board, int fromRow, int fromColumn, int toRow, int toColumn);

		/// <summary>
		///		Comprueba si es un movimiento horizontal
		/// </summary>
		protected bool IsVerticalHorizontalMovement(int fromRow, int fromColumn, int toRow, int toColumn, bool onlyOneCell)
		{
			int rowDifference = Math.Abs(fromRow - toRow);
			int columnDifference = Math.Abs(fromColumn - toColumn);

				// Comprueba si puede mover
				return ((rowDifference == 0 && columnDifference != 0) || (rowDifference != 0 && columnDifference == 0)) &&
						CheckOnlyOneCell(onlyOneCell, rowDifference, columnDifference);
		}

		/// <summary>
		///		Clona los datos de la figura
		/// </summary>
		public abstract PieceBaseModel Clone();

		/// <summary>
		///		Clona los datos internos
		/// </summary>
		protected void CloneInner(PieceBaseModel target)
		{
			target.IsMoved = IsMoved;
		}

		/// <summary>
		///		Comprueba si es un movimiento diagonal
		/// </summary>
		protected bool IsDiagonalMovement(int fromRow, int fromColumn, int toRow, int toColumn, bool onlyOneCell)
		{
			int rowDifference = Math.Abs(fromRow - toRow);
			int columnDifference = Math.Abs(fromColumn - toColumn);

				// Comprueba si es o no una sóla celda dependiendo de si se ha pedido o no mover sobre una sola celda
				return rowDifference != 0 && rowDifference == columnDifference && CheckOnlyOneCell(onlyOneCell, rowDifference, columnDifference);
		}

		/// <summary>
		///		Comprueba que sólo se ha movido una celda
		/// </summary>
		private bool CheckOnlyOneCell(bool onlyOneCell, int rowDiference, int columnDifference)
		{
			return !onlyOneCell || (onlyOneCell && Math.Max(rowDiference, columnDifference) == 1);
		}

		/// <summary>
		///		Tipo de pieza
		/// </summary>
		public PieceType Type { get; }

		/// <summary>
		///		Color de la pieza
		/// </summary>
		public PieceColor Color { get; }

		/// <summary>
		///		Indica si se ha movido
		/// </summary>
		public bool IsMoved { get; set; }
	}
}
