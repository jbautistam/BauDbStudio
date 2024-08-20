using System;

using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Setup;
using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.LibPgn.Conversor.Parsers
{
	/// <summary>
	///		Intérprete de una cadena FEN - Notación Forsyth–Edwards
	/// </summary>
	/// <remarks>
	///		FEN "2R5/1Q6/3PN3/2N1P2p/p1k3qR/2p2p2/8/b4Kn1 w - - 0 1"
	///		http://www.edicionesma40.com/blog/la-notacion-fen.htm
	/// </remarks>
	internal class FenParser
	{
		/// <summary>
		///		Interpreta una cadena FEN
		/// </summary>
		internal BoardSetup Parse(string fen)
		{
			BoardSetup board = new BoardSetup();

				// Interpreta la cadena
				if (!string.IsNullOrEmpty(fen))
				{
					string [] parts = fen.Trim().Split(' ');

						if (parts.Length == 6)
						{
							// Obtiene las piezas del tablero
							board.Pieces.AddRange(ParseBoardPieces(parts[0]));
							// Obtiene si tienen que jugar blancas o negras
							// “w” significa que mueven las blancas y “b” las negras
							board.Parameters.IsWhiteMove = parts[1].Equals("W", StringComparison.CurrentCultureIgnoreCase);
							// Obtiene la posibilidad de enroques
							// Si el enroque ya no es posible para ninguna de las partes, se coloca el carácter “-“. 
							// De lo contrario, irán una o más letras: “K” (es posible el enroque en el flanco del rey blanco), 
							// “Q” (es posible el enroque en el flanco de la dama blanca), “k” (es posible el enroque en el flanco 
							// del rey negro), y/o “q” (es posible el enroque en el flanco de la dama negra).
							board.Parameters.CanWhiteCastleKingSide = parts[2].IndexOf("K") >= 0;
							board.Parameters.CanWhiteCastleQueenSide = parts[2].IndexOf("Q") >= 0;
							board.Parameters.CanBlackCastleKingSide = parts[2].IndexOf("k") >= 0;
							board.Parameters.CanBlackCastleQueenSide = parts[2].IndexOf("q") >= 0;
							// Captura al paso
							//  Posibilidad de captura al paso de un peón en notación algebraica. Si no hay posibilidad de 
							// captura al paso de ningún peón, se colocará el carácter “-“. Si un peón acaba de hacer una 
							// jugada de 2 cuadros, se pondrá la casilla que está “detrás” del peón. Ésta se deberá poner 
							// independientemente de si hay o no algún peón en condiciones de realizar una captura al paso.
							if (parts[3] != "-")
								board.Parameters.EnPassantCellTarget = new CellModel(parts[3]);
							// Número de medios movimientos
							// Número de medios movimientos desde la última captura o avance de peón. Esto se utiliza para determinar si unas tablas 
							// pueden ser reclamadas en virtud de la regla de los cincuenta movimientos.
							board.Parameters.HalfMoveClock = GetInt(parts[4]);
							// Número total de movimientos de la partida. 
							// Tiene el valor de 1 para la posición inicial y se va incrementando con cada movimiento de las negras.
							board.Parameters.FullMoveCount = GetInt(parts[5]);
						}
				}
				// Devuelve los datos del tablero
				return board;
		}

		/// <summary>
		///		Interpreta las piezas del tablero: rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R
		/// </summary>
		private PieceWithCellCollection ParseBoardPieces(string boardPieces)
		{
			PieceWithCellCollection pieces = new PieceWithCellCollection();
			int row = 0;

				// Carga las piezas
				if (!string.IsNullOrEmpty(boardPieces))
					foreach (string rowPart in boardPieces.Split('/'))
						if (!string.IsNullOrWhiteSpace(rowPart) && row >= 0)
						{
							int column = 0;

								// Asigna las piezas
								foreach (char data in rowPart)
									if (char.IsDigit(data))
										column += data - '0';
									else if (column < 8)
									{
										string piece = data.ToString();

											// Convierte la pieza
											switch (piece.ToUpper())
											{
												case "P":
														pieces.Add(PieceBaseModel.PieceType.Pawn, GetColor(piece), row, column);
													break;
												case "R":
														pieces.Add(PieceBaseModel.PieceType.Rook, GetColor(piece), row, column);
													break;
												case "N":
														pieces.Add(PieceBaseModel.PieceType.Knight, GetColor(piece), row, column);
													break;
												case "B":
														pieces.Add(PieceBaseModel.PieceType.Bishop, GetColor(piece), row, column);
													break;
												case "K":
														pieces.Add(PieceBaseModel.PieceType.King, GetColor(piece), row, column);
													break;
												case "Q":
														pieces.Add(PieceBaseModel.PieceType.Queen, GetColor(piece), row, column);
													break;
											}
											// Pasa a la siguiente columna
											column++;
									}
								// Incrementa la fila
								row++;
						}
				// Devuelve la colección de piezas
				return pieces;
		}

		/// <summary>
		///		Obtiene el color asociado a una letra
		/// </summary>
		private PieceBaseModel.PieceColor GetColor(string letter)
		{
			if (letter == letter.ToUpper())
				return PieceBaseModel.PieceColor.White;
			else
				return PieceBaseModel.PieceColor.Black;
		}

		/// <summary>
		///		Convierte una cadena a entero
		/// </summary>
		private int GetInt(string value)
		{
			if (int.TryParse(value, out int result))
				return result;
			else
				return 0;
		}
	}
}
