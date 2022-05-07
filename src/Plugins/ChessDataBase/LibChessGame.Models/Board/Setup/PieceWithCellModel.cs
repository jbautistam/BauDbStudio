using System;

using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Models.Board.Setup
{
	/// <summary>
	///		Modelo con los datos de una pieza y su celda
	/// </summary>
	public class PieceWithCellModel
	{
		public PieceWithCellModel(PieceBaseModel piece, CellModel cell)
		{
			Piece = piece;
			Cell = cell;
		}

		/// <summary>
		///		Pieza
		/// </summary>
		public PieceBaseModel Piece { get; }

		/// <summary>
		///		Celda
		/// </summary>
		public CellModel Cell { get; }
	}
}
