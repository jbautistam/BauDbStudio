using System;
using System.Collections.Generic;

using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Pieces;
using Bau.Libraries.LibPgnReader.Models.Movements;

namespace Bau.Libraries.LibPgn.Conversor.Parsers
{
	/// <summary>
	///		Conversor de movimientos
	/// </summary>
	internal class MovementConversor
	{
		internal MovementConversor(SquareModel board)
		{
			Board = board;
		}

		/// <summary>
		///		Convierte los movimientos
		/// </summary>
		internal List<MovementBaseModel> Convert(List<BaseMovementModel> movements)
		{
			List<MovementBaseModel> movementsConverted = new List<MovementBaseModel>();

				// Convierte los movimientos
				foreach (BaseMovementModel movement in movements)
					movementsConverted.Add(ConvertMovement(movement));
				// Devuelve la lista de movimientos
				return movementsConverted;
		}

		/// <summary>
		///		Convierte un movimiento
		/// </summary>
		private MovementBaseModel ConvertMovement(BaseMovementModel movementPgn)
		{
			switch (movementPgn)
			{
				case PieceMovementModel movement:
					return ConvertPieceMovement(movement);
				case ResultMovementModel movement:
					return ConvertResultMovement(movement);
				default:
					throw new ChessDataBase.Models.Exceptions.GameReaderException("Unknown movement type");
			}
		}

		/// <summary>
		///		Convierte un movimiento de pieza
		/// </summary>
		private MovementFigureModel ConvertPieceMovement(PieceMovementModel movement)
		{
			MovementFigureModel movementFigure = new MovementFigureModel(ConvertTurn(movement.Turn), movement.Content);

				// Convierte los datos básicos
				ConvertMovementBase(movement, movementFigure);
				// Añade las variaciones (antes de mover real)
				foreach (VariationModel variation in movement.Variations)
					movementFigure.Variations.Add(ConvertVariation(variation, movementFigure));
				// Convierte el color de la figura
				if (movement.Turn.Type == TurnModel.TurnType.White)
					movementFigure.Color = PieceBaseModel.PieceColor.White;
				else
					movementFigure.Color = PieceBaseModel.PieceColor.Black;
				// Convierte los datos de la notación SAN
				ConvertSan(movementFigure, movement.Content);
				// Devuelve el movimiento
				return movementFigure;
		}

		/// <summary>
		///		Convierte los datos básicos del movimiento
		/// </summary>
		private void ConvertMovementBase(BaseMovementModel movementPgn, MovementBaseModel movement)
		{
			// Asigna los comentarios
			foreach (string comment in movementPgn.Comments)
				movement.Comments.Add(comment);
			// Asigna la información
			foreach (InfoMovementModel info in movementPgn.Info)
				movement.Info.Add(new MovementInfoModel(info.Content));
		}

		/// <summary>
		///		Obtiene los datos de la notación SAN
		/// </summary>
		private void ConvertSan(MovementFigureModel movement, string content)
		{
			//// Debug
			//#if DEBUG
			//	System.Diagnostics.Debug.WriteLine($"Start convert SAN: {content}");
			//#endif
			// Tipo de movimiento
			movement.Type = MovementFigureModel.MovementType.Normal;
			// Obtiene la información básica del movimiento
			if (content == "0-0" || content == "O-O")
			{
				// Datos de la pieza
				movement.OriginPiece = PieceBaseModel.PieceType.King;
				movement.Type = MovementFigureModel.MovementType.CastleKingSide;
				// Quita los datos del contenido
				if (content.Length == 3)
					content = string.Empty;
				else
					content = content.Substring(3);
			}
			else if (content == "0-0-0" || content == "O-O-O")
			{
				// Datos de la pieza
				movement.OriginPiece = PieceBaseModel.PieceType.King;
				movement.Type = MovementFigureModel.MovementType.CastleQueenSide;
				// Quita los datos del contenido
				if (content.Length == 5)
					content = string.Empty;
				else
					content = content.Substring(5);
			}
			else
			{
				// Obtiene la pieza
				movement.OriginPiece = ExtractPiece(ref content);
				// Obtiene la información de las celdas origen y destino
				ExtractCellsInfo(movement, ref content);
			}
			// Obtiene la información adicional
			if (!string.IsNullOrWhiteSpace(content))
			{
				// Promociones, jaque, jaque mate
				if (content.StartsWith("=")) // promoción
				{
					content = content.Substring(1);
					movement.PromotedPiece = ExtractPiece(ref content);
				}
				else if (content.StartsWith("(=)")) // el jugador ofrece tablas (algunos archivos)
				{
					content = content.Substring(3);
					movement.IsDrawOffered = true;
				}
				else if (content.StartsWith("++")) // jaque mate en algunos archivos
				{
					content = content.Substring(2);
					movement.IsCheckMate = true;
				}
				else if (content.StartsWith("+")) // jaque
				{
					content = content.Substring(1);
					movement.IsCheck = true;
				}
				else if (content.StartsWith("#")) // jaque mate
				{
					content = content.Substring(1);
					movement.IsCheckMate = true;
				}
				// Anotaciones
				if (!string.IsNullOrWhiteSpace(content))
					movement.Info.Add(new MovementInfoModel(content));
			}
			// Y por último realiza el movimiento
			MakeMove(movement);
		}

		/// <summary>
		///		Extrae la celda origen del movimiento
		/// </summary>
		/// <remarks>
		/// Disambiguation
		/// In the case of ambiguities (multiple pieces of the same type moving to the same square), the first appropriate 
		/// disambiguating step of the three following steps is taken:
		/// First, if the moving pieces can be distinguished by their originating files, the originating file letter of the 
		/// moving piece is inserted immediately after the moving piece letter.
		/// Second (when the first step fails), if the moving pieces can be distinguished by their originating ranks, the 
		/// originating rank digit of the moving piece is inserted immediately after the moving piece letter.
		/// Third (when both the first and the second steps fail), the two character square coordinate of the originating 
		/// square of the moving piece is inserted immediately after the moving piece letter.
		/// </remarks>
		/// <example>
		/// cxb5 axb5 dxe5 Bxe7 Qxe7 Nxc4 Bxc4 Nb6 Rxe1+ 
		/// </example>
		private void ExtractCellsInfo(MovementFigureModel movement, ref string content)
		{
			string cellStart = string.Empty, cellEnd = string.Empty;

				// Captura
				if (content.StartsWith("x", StringComparison.CurrentCultureIgnoreCase))
				{
					movement.Type = MovementFigureModel.MovementType.Capture;
					content = content.Substring(1);
				}
				// Obtiene los datos de la celda de inicio
				cellStart = ExtractCell(ref content);
				// Captura
				if (content.StartsWith("x", StringComparison.CurrentCultureIgnoreCase))
				{
					// Indica que es una captura (y ya ha habido información de celda)
					movement.Type = MovementFigureModel.MovementType.Capture;
					content = content.Substring(1);
					// Obtiene los datos de la celda de fin
					if (!string.IsNullOrWhiteSpace(content))
						cellEnd = ExtractCell(ref content);
				}
				// Interpreta el movimiento
				if (string.IsNullOrWhiteSpace(cellStart) && string.IsNullOrWhiteSpace(cellEnd))
					throw new ChessDataBase.Models.Exceptions.GameReaderException($"Cant get movement from {movement.Content}");
				else
				{
					(CellModel from, CellModel to) = GetChoords(cellStart, cellEnd);
					(int row, int column, PieceBaseModel piece) pieceCell = Board.SearchMoveTo(movement.OriginPiece, movement.Color, from.Row, 
																							   from.Column, to.Row, to.Column);

						// Asigna los datos del movimiento
						if (pieceCell.piece == null)
							throw new ChessDataBase.Models.Exceptions.GameReaderException($"Cant find who moves to {movement.Content}");
						else
						{
							// Asigna el movimiento de inicio y fin
							movement.From = new CellModel(pieceCell.row, pieceCell.column);
							movement.To = to;
							// Asigna la pieza capturada
							if (movement.Type == MovementFigureModel.MovementType.Capture)
							{
								if (Board.IsCaptureEnPassant(Board[movement.From], movement.To.Row, movement.To.Column, Board[movement.To]))
								{
									movement.Captured = new PawnModel(Board.Parameters.EnPassantColor);
									movement.CapturedEnPassant = new CellModel(Board.Parameters.EnPassantCell);
								}
								else
									movement.Captured = Board[movement.To];
							}
						}
				}
		}

		/// <summary>
		///		Ajusta las coordenadas del movimiento
		/// </summary>
		private (CellModel from, CellModel to) GetChoords(string cellStart, string cellEnd)
		{
			CellModel from = new CellModel(-1, -1);
			CellModel to = new CellModel(-1, -1);

				if (!string.IsNullOrWhiteSpace(cellStart) && string.IsNullOrWhiteSpace(cellEnd))
				{
					// Obtiene los datos de desambiguacion
					if (cellStart.Length <= 2)
						to = new CellModel(cellStart);
					else
					{
						// Obtiene la celda de inicio
						from = new CellModel(cellStart.Substring(0, cellStart.Length - 2));
						// Obtiene el dato final
						to = new CellModel(cellStart.Substring(cellStart.Length - 2, 2));
					}
				}
				else if (!string.IsNullOrWhiteSpace(cellStart) && !string.IsNullOrWhiteSpace(cellEnd))
				{
					from = new CellModel(cellStart);
					to = new CellModel(cellEnd);
				}
				// Devuelve el movimiento
				return (from, to);
		}

		/// <summary>
		///		Extra el contenido de la celda
		/// </summary>
		private string ExtractCell(ref string content)
		{
			const string validChars = "abcdefghABCDEFGH12345678";
			string cell = string.Empty;
			int index = 0;

				// Busca el final de la cadena de celda
				while (index < content.Length && validChars.IndexOf(content[index]) >= 0)
				{
					cell += content[index];
					index++;
				}
				// Extrae el contenido de la celda
				if (index > 0)
				{
					if (cell.Length < content.Length)
						content = content.Substring(cell.Length);
					else
						content = string.Empty;
				}
				// Devuelve el texto de la celda
				return cell;
		}

		/// <summary>
		///		Extrae la pieza de la cadena
		/// </summary>
		private PieceBaseModel.PieceType ExtractPiece(ref string content)
		{
			PieceBaseModel.PieceType piece = PieceBaseModel.PieceType.Pawn;
			bool extract = false;

				// Obtiene la pieza
				switch (content[0])
				{
					case 'P':
							piece = PieceBaseModel.PieceType.Pawn;
							extract = true;
						break;
					case 'N':
							piece = PieceBaseModel.PieceType.Knight;
							extract = true;
						break;
					case 'B':
							piece = PieceBaseModel.PieceType.Bishop;
							extract = true;
						break;
					case 'R':
							piece = PieceBaseModel.PieceType.Rook;
							extract = true;
						break;
					case 'Q':
							piece = PieceBaseModel.PieceType.Queen;
							extract = true;
						break;
					case 'K':
							piece = PieceBaseModel.PieceType.King;
							extract = true;
						break;
				}
				// Extrae el contenido de la cadena
				if (extract)
					content = content.Substring(1);
				// Devuelve la pieza
				return piece;
		}

		/// <summary>
		///		Convierte el resultado final del juego
		/// </summary>
		private MovementGameEndModel ConvertResultMovement(ResultMovementModel movement)
		{
			MovementGameEndModel movementEnd = new MovementGameEndModel(ConvertTurn(movement.Turn), movement.Content);

				// Convierte los datos básicos
				ConvertMovementBase(movement, movementEnd);
				// Devuelve el movimiento
				return movementEnd;
		}

		/// <summary>
		///		Convierte los datos del turno
		/// </summary>
		private MovementTurnModel ConvertTurn(TurnModel turn)
		{
			MovementTurnModel.TurnType ConvertTurnType(TurnModel.TurnType type)
			{
				switch (type)
				{
					case TurnModel.TurnType.Black:
						return MovementTurnModel.TurnType.Black;
					case TurnModel.TurnType.White:
						return MovementTurnModel.TurnType.White;
					default:
						return MovementTurnModel.TurnType.End;
				}
			}

			return new MovementTurnModel(turn.Number, ConvertTurnType(turn.Type));
		}

		/// <summary>
		///		Convierte una variación
		/// </summary>
		private MovementVariationModel ConvertVariation(VariationModel variation, MovementBaseModel parent)
		{
			MovementVariationModel movementVariation = new MovementVariationModel(parent);
			SquareModel board = Board.Clone();

				// Ajusta el movimiento al color de inicio de la variación
				board.Parameters.IsWhiteMove = variation.Movements[0].Turn.Type == TurnModel.TurnType.White;
				// Presenta el tablero
				//#if DEBUG
				//	System.Diagnostics.Debug.WriteLine("Board for variation:");
				//	System.Diagnostics.Debug.WriteLine(board.GetText());
				//	System.Diagnostics.Debug.WriteLine(new string('-', 80));
				//#endif
				// Convierte los movimientos
				movementVariation.Movements.AddRange(new MovementConversor(board).Convert(variation.Movements));
				// Presenta el tablero real
				//#if DEBUG
				//	System.Diagnostics.Debug.WriteLine("Board after variation:");
				//	System.Diagnostics.Debug.WriteLine(Board.GetText());
				//	System.Diagnostics.Debug.WriteLine(new string('-', 80));
				//#endif
				// Devuelve la variación
				return movementVariation;
		}

		/// <summary>
		///		Realiza el movimiento de la figura
		/// </summary>
		private void MakeMove(MovementFigureModel movement)
		{
			//// Debug
			//#if DEBUG
			//	System.Diagnostics.Debug.WriteLine($"Movement: {movement}");
			//#endif
			// Realiza los movimientos
			switch (movement.Type)
			{
				case MovementFigureModel.MovementType.CastleKingSide:
						if (!Board.CanMoveCastleKingSide(movement.Color))
							throw new ChessDataBase.Models.Exceptions.GameReaderException($"Error at movement castle king side {movement.Content}");
						else
							Board.MoveCastleKingSide(movement.Color);
					break;
				case MovementFigureModel.MovementType.CastleQueenSide:
						if (!Board.CanMoveCastleQueenSide(movement.Color))
							throw new ChessDataBase.Models.Exceptions.GameReaderException($"Error at movement castle queen side {movement.Content}");
						else
							Board.MoveCastleQueenSide(movement.Color);
					break;
				case MovementFigureModel.MovementType.CaptureEnPassant:
						throw new NotImplementedException("Cant do EnPassant capture");
				default:
						if (!Board.CanMove(movement.From.Row, movement.From.Column, movement.To.Row, movement.To.Column))
							throw new ChessDataBase.Models.Exceptions.GameReaderException($"Error at movement {movement.Content}");
						else
							Board.Move(movement);
					break;
			}
			//// Debug
			//#if DEBUG
			//	System.Diagnostics.Debug.WriteLine(Board.GetText());
			//	System.Diagnostics.Debug.WriteLine(new string('-', 80));
			//#endif
		}

		/// <summary>
		///		Datos del tablero
		/// </summary>
		private SquareModel Board { get; }
	}
}
