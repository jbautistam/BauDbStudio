using System;

using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Models.Board.Setup
{
    /// <summary>
    ///		Configuración del tablero, se utiliza cuando la partida no está completa si no que se ha creado a partir de una posición inicial
    /// </summary>
    public class BoardSetup
    {
		/// <summary>
		///		Obtiene el texto del tablero
		/// </summary>
		public string GetText()
		{
			string board = "  0 1 2 3 4 5 6 7" + Environment.NewLine + 
						   "  A B C D E F G H" + Environment.NewLine;

				// Obtiene el texto
				for (int row = 0; row < 8; row++)
				{
					// Añade el carácter con la fila
					board += $"{8 - row} ";
					for (int column = 0; column < 8; column++)
					{
						PieceBaseModel piece = Pieces.GetPiece(new CellModel(row, column));

							if (piece == null)
								board += ".";
							else
								switch (piece.Type)
								{
									case PieceBaseModel.PieceType.Pawn:
											board += piece.Color == PieceBaseModel.PieceColor.White ? "P" : "p";
										break;
									case PieceBaseModel.PieceType.Bishop:
											board += piece.Color == PieceBaseModel.PieceColor.White ? "B" : "b";
										break;
									case PieceBaseModel.PieceType.Knight:
											board += piece.Color == PieceBaseModel.PieceColor.White ? "N" : "n";
										break;
									case PieceBaseModel.PieceType.Rook:
											board += piece.Color == PieceBaseModel.PieceColor.White ? "R" : "r";
										break;
									case PieceBaseModel.PieceType.King:
											board += piece.Color == PieceBaseModel.PieceColor.White ? "K" : "k";
										break;
									case PieceBaseModel.PieceType.Queen:
											board += piece.Color == PieceBaseModel.PieceColor.White ? "Q" : "q";
										break;
								}
							board += " ";
					}
					// Añade el número de fila y un salto de línea
					board += $" {8 - row} {row}" + Environment.NewLine;
				}
				// Añade el número de columna
				board += "  A B C D E F G H" + Environment.NewLine +
						 "  0 1 2 3 4 5 6 7";
				// Devuelve el texto
				return board;
		}

		/// <summary>
		///		Indica si se ha configurado el tablero
		/// </summary>
		public bool HasSetup 
		{ 
			get { return Pieces.Count > 0; }
		}

		/// <summary>
		///		Parámetros del tablero
		/// </summary>
		public SquareParametersModel Parameters { get; } = new SquareParametersModel();

		/// <summary>
		///		Posición de las piezas
		/// </summary>
		public PieceWithCellCollection Pieces { get; } = new PieceWithCellCollection();
    }
}
