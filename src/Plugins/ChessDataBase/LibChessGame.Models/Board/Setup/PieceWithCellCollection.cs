using System;
using System.Collections.Generic;

using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Models.Board.Setup
{
	/// <summary>
	///		Colección de <see cref="PieceWithCellModel"/>
	/// </summary>
	public class PieceWithCellCollection : List<PieceWithCellModel>
	{
		/// <summary>
		///		Añade una pieza
		/// </summary>
		public void Add(PieceBaseModel.PieceType piece, PieceBaseModel.PieceColor color, int row, int column)
		{
			Add(piece, color, new CellModel(row, column));
		}

		/// <summary>
		///		Añade una pieza
		/// </summary>
		public void Add(PieceBaseModel.PieceType piece, PieceBaseModel.PieceColor color, CellModel cell)
		{
			switch (piece)
			{
				case PieceBaseModel.PieceType.Pawn:
						Add(new PieceWithCellModel(new PawnModel(color), cell));
					break;
				case PieceBaseModel.PieceType.Rook:
						Add(new PieceWithCellModel(new RookModel(color), cell));
					break;
				case PieceBaseModel.PieceType.Knight:
						Add(new PieceWithCellModel(new KnightModel(color), cell));
					break;
				case PieceBaseModel.PieceType.Bishop:
						Add(new PieceWithCellModel(new BishopModel(color), cell));
					break;
				case PieceBaseModel.PieceType.Queen:
						Add(new PieceWithCellModel(new QueenModel(color), cell));
					break;
				case PieceBaseModel.PieceType.King:
						Add(new PieceWithCellModel(new KingModel(color), cell));
					break;
			}
		}

		/// <summary>
		///		Clona las piezas
		/// </summary>
		public PieceWithCellCollection Clone()
		{
			PieceWithCellCollection pieces = new PieceWithCellCollection();

				// Clona las piezas
				foreach (PieceWithCellModel piece in this)
					pieces.Add(piece.Piece.Type, piece.Piece.Color, piece.Cell);
				// Devuelve la colección
				return pieces;
		}

		/// <summary>
		///		Obtiene la pieza que está en una fila / columna
		/// </summary>
		public PieceBaseModel GetPiece(CellModel cell)
		{
			// Busca la pieza
			foreach (PieceWithCellModel piece in this)
				if (piece.Cell.Row == cell.Row && piece.Cell.Column == cell.Column)
					return piece.Piece;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Comprueba si está vacía una celda
		/// </summary>
		public bool IsEmpty(CellModel cell)
		{
			return GetPiece(cell) == null;
		}
	}
}
