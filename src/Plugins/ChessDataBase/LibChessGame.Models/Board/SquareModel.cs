using System;
using System.Collections.Generic;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Models.Board
{
	/// <summary>
	///		Clase con las celdas del tablero
	/// </summary>
	/// <remarks>
	///		El tablero tiene esta estructura ( -> Celda blanca, X -> Celda negra)
	///		       01234567
	///		       --------
	///			8 | X X X X| 0
	///			7 |X X X X | 1
	///			6 | X X X X| 2
	///			5 |X X X X | 3
	///			4 | X X X X| 4
	///			3 |X X X X | 5 
	///			2 | X X X X| 6
	///			1 |X X X X | 7
	///		       --------
	///			   ABCDEFGH
	/// </remarks>
	public class SquareModel
	{
		/// <summary>
		///		Clona el tablero
		/// </summary>
		public SquareModel Clone()
		{
			SquareModel target = new SquareModel();

				// Clona los datos
				for (int row = 0; row < 8; row++)
					for (int column = 0; column < 8; column++)
					{
						PieceBaseModel piece = this[row, column];

							if (this[row, column] == null)
								target[row, column] = null;
							else
							{
								target.Create(piece.Type, piece.Color, row, column);
								target[row, column].IsMoved = piece.IsMoved;
							}
					}
				// Clona los parámetros
				target.Parameters = Parameters.Clone();
				// Devuelve el tablero
				return target;
		}

		/// <summary>
		///		Limpia el tablero
		/// </summary>
		private void Clear()
		{
			// Limpia el tablero
			for (int index = 0; index < Cells.Length; index++)
				Cells[index] = null;
			// Limpia los parámetros
			Parameters.Clear();
		}

		/// <summary>
		///		Inicializa el tablero
		/// </summary>
		public void Reset()
		{
			// Limpia el tablero
			Clear();
			// Añade las piezas blancas
			Create(PieceBaseModel.PieceType.Rook, PieceBaseModel.PieceColor.White, 7, 0);
			Create(PieceBaseModel.PieceType.Knight, PieceBaseModel.PieceColor.White, 7, 1);
			Create(PieceBaseModel.PieceType.Bishop, PieceBaseModel.PieceColor.White, 7, 2);
			Create(PieceBaseModel.PieceType.Queen, PieceBaseModel.PieceColor.White, 7, 3);
			Create(PieceBaseModel.PieceType.King, PieceBaseModel.PieceColor.White, 7, 4);
			Create(PieceBaseModel.PieceType.Bishop, PieceBaseModel.PieceColor.White, 7, 5);
			Create(PieceBaseModel.PieceType.Knight, PieceBaseModel.PieceColor.White, 7, 6);
			Create(PieceBaseModel.PieceType.Rook, PieceBaseModel.PieceColor.White, 7, 7);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 0);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 1);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 2);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 3);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 4);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 5);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 6);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.White, 6, 7);
			// Añade las piezas negras
			Create(PieceBaseModel.PieceType.Rook, PieceBaseModel.PieceColor.Black, 0, 0);
			Create(PieceBaseModel.PieceType.Knight, PieceBaseModel.PieceColor.Black, 0, 1);
			Create(PieceBaseModel.PieceType.Bishop, PieceBaseModel.PieceColor.Black, 0, 2);
			Create(PieceBaseModel.PieceType.Queen, PieceBaseModel.PieceColor.Black, 0, 3);
			Create(PieceBaseModel.PieceType.King, PieceBaseModel.PieceColor.Black, 0, 4);
			Create(PieceBaseModel.PieceType.Bishop, PieceBaseModel.PieceColor.Black, 0, 5);
			Create(PieceBaseModel.PieceType.Knight, PieceBaseModel.PieceColor.Black, 0, 6);
			Create(PieceBaseModel.PieceType.Rook, PieceBaseModel.PieceColor.Black, 0, 7);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 0);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 1);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 2);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 3);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 4);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 5);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 6);
			Create(PieceBaseModel.PieceType.Pawn, PieceBaseModel.PieceColor.Black, 1, 7);
		}

		/// <summary>
		///		Inicializa el tablero a una configuración en concreto
		/// </summary>
		public void Reset(Setup.BoardSetup setup)
		{
			// Limpia el tablero
			Clear();
			// Añade las piezas
			foreach (Setup.PieceWithCellModel piece in setup.Pieces)
				Create(piece.Piece.Type, piece.Piece.Color, piece.Cell.Row, piece.Cell.Column);
			// Clona los parámetros
			Parameters = setup.Parameters.Clone();
		}

		/// <summary>
		///		Asigna una pieza a una posición
		/// </summary>
		private void Create(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color, int row, int column)
		{
			this[row, column] = CreatePiece(type, color);
		}

		/// <summary>
		///		Crea una pieza
		/// </summary>
		private PieceBaseModel CreatePiece(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color)
		{
			switch (type)
			{
				case PieceBaseModel.PieceType.Pawn:
					return new PawnModel(color);
				case PieceBaseModel.PieceType.Rook:
					return new RookModel(color);
				case PieceBaseModel.PieceType.Knight:
					return new KnightModel(color);
				case PieceBaseModel.PieceType.Bishop:
					return new BishopModel(color);
				case PieceBaseModel.PieceType.Queen:
					return new QueenModel(color);
				case PieceBaseModel.PieceType.King:
					return new KingModel(color);
				default:
					throw new ArgumentException($"Piece type unknown: {type}");
			}
		}

		/// <summary>
		///		Deshace un movimiento
		/// </summary>
		public void Undo(MovementFigureModel movement)
		{
			if (movement != null)
				switch (movement.Type)
				{
					case MovementFigureModel.MovementType.CastleKingSide:
							UndoCastleKingSide(movement.Color);
						break;
					case MovementFigureModel.MovementType.CastleQueenSide:
							UndoCastleQueenSide(movement.Color);
						break;
					default:
							UndoMove(movement);
						break;
				}
		}

		/// <summary>
		///		Realiza un movimiento
		/// </summary>
		public bool Move(MovementFigureModel movement)
		{
			bool canMove = false;

				// Si hay algún movimiento por hacer
				if (movement != null)
				{
					// Dependiendo del tipo de movimiento
					switch (movement.Type)
					{
						case MovementFigureModel.MovementType.CastleKingSide:
								canMove = MoveCastleKingSide(movement.Color);
							break;
						case MovementFigureModel.MovementType.CastleQueenSide:
								canMove = MoveCastleQueenSide(movement.Color);
							break;
						default:
								canMove = Move(movement.From.Row, movement.From.Column, movement.To.Row, movement.To.Column);
							break;
					}
					// Promociona la pieza
					if (canMove && movement.PromotedPiece != null)
						canMove = Promote(movement.To.Row, movement.To.Column, movement.PromotedPiece ?? PieceBaseModel.PieceType.Pawn);
				}
				// Devuelve el valor que indica si se puede hacer un movimiento
				return canMove;
		}

		/// <summary>
		///		Enroque corto
		/// </summary>
		public bool MoveCastleKingSide(PieceBaseModel.PieceColor color)
		{
			bool canMove = CanMoveCastleKingSide(color);

				// Realiza el enroque
				if (canMove)
				{
					int row = GetRowKing(color);

						// Mueve el rey
						MoveWithoutCheck(row, 4, row, 6, false);
						// Mueve la torre
						MoveWithoutCheck(row, 7, row, 5, true);
						// Deshabilita los enroques (los dos)
						Parameters.DisableCastle(color);
				}
				// Devuelve el valor que indica si se ha movido
				return canMove;
		}

		/// <summary>
		///		Deshace el movimiento de enroque corto
		/// </summary>
		private void UndoCastleKingSide(PieceBaseModel.PieceColor color)
		{
			int row = GetRowKing(color);

				// Mueve el rey
				MoveWithoutCheck(row, 6, row, 4, false);
				// Mueve la torre
				MoveWithoutCheck(row, 5, row, 7, false);
				// Habilita los enroques
				Parameters.EnableCastle(color);
				//TODO --> Debería indicar que no se ha movido la torre ni el rey
		}

		/// <summary>
		///		Enroque largo
		/// </summary>
		public bool MoveCastleQueenSide(PieceBaseModel.PieceColor color)
		{
			bool canMove = CanMoveCastleQueenSide(color);

				// Realiza el enroque
				if (canMove)
				{
					int row = GetRowKing(color);

						// Mueve el rey
						MoveWithoutCheck(row, 4, row, 2, false);
						// Mueve la torre
						MoveWithoutCheck(row, 0, row, 3, true);
						// Deshabilita los enroques (los dos)
						Parameters.DisableCastle(color);
				}
				// Devuelve el valor que indica si se ha movido
				return canMove;
		}

		/// <summary>
		///		Deshace el movimiento de enroque largo
		/// </summary>
		private void UndoCastleQueenSide(PieceBaseModel.PieceColor color)
		{
			int row = GetRowKing(color);

				// Mueve el rey
				MoveWithoutCheck(row, 2, row, 4, false);
				// Mueve la torre
				MoveWithoutCheck(row, 3, row, 0, false);
				// Habilita los enroques (los dos)
				Parameters.EnableCastle(color);
				//TODO --> Debería indicar que no se ha movido la torre ni el rey
		}

		/// <summary>
		///		Realiza un movimiento
		/// </summary>
		public bool Move(int fromRow, int fromColumn, int toRow, int toColumn)
		{
			bool canMove = CanMove(fromRow, fromColumn, toRow, toColumn);

				// Mueve la pieza
				if (canMove)
					MoveWithoutCheck(fromRow, fromColumn, toRow, toColumn);
				// Devuelve el valor que indica que se ha podido mover
				return canMove;
		}

		/// <summary>
		///		Deshace el movimiento
		/// </summary>
		private void UndoMove(MovementFigureModel movement)
		{
			// Deshace la promoción
			if (movement.PromotedPiece != null)
				this[movement.To] = new PawnModel(movement.Color);
			// Deshace el movimiento
			MoveWithoutCheck(movement.To.Row, movement.To.Column, movement.From.Row, movement.From.Column, false);
			// Deshace la captura
			if (movement.Captured != null)
				CreatePiece(movement.Captured.Type, movement.Captured.Color);
			//? Debería cambiar los parámetros de movimiento de la figura
		}

		/// <summary>
		///		Realiza un movimiento (sin comprobaciones por los enroques)
		/// </summary>
		private void MoveWithoutCheck(int fromRow, int fromColumn, int toRow, int toColumn, bool adjustParameters = true)
		{
			PieceBaseModel piece = this[fromRow, fromColumn];

				if (piece != null)
				{
					// Mueve la pieza
					this[toRow, toColumn] = piece;
					this[fromRow, fromColumn] = null;
					// Asigna los datos para la captura al paso
					CheckEnPassantMove(piece, fromRow, toRow, toColumn);
					// Indica que se ha movido
					piece.IsMoved = true;
					// Ajusta los parámetros (en el caso del enroque, se hacen dos movimientos pero sólo se debe contabilizar uno)
					if (adjustParameters)
					{
						// Añade el movimiento
						Parameters.HalfMoveClock++;
						if (Parameters.HalfMoveClock % 2 == 0)
							Parameters.FullMoveCount++;
						// Cambia el turno
						Parameters.IsWhiteMove = !Parameters.IsWhiteMove;
						// Si es un rey o una torre, invalida los movimientos de enroque
						if (piece.Type == PieceBaseModel.PieceType.King)
							Parameters.DisableCastle(piece.Color);
						else if (piece.Type == PieceBaseModel.PieceType.Rook)
						{
							if (fromColumn == 0)
								Parameters.DisableCastleQueenSide(piece.Color);
							else if (fromColumn == 7)
								Parameters.DisableCastleKingSide(piece.Color);
						}
					}
				}
		}

		/// <summary>
		///		Comprueba un movimiento al paso
		/// </summary>
		private void CheckEnPassantMove(PieceBaseModel piece, int fromRow, int toRow, int toColumn)
		{
			if (piece is PawnModel && Math.Abs(fromRow - toRow) == 2)
			{
				// Asigna la celda donde se coloca el peón que se puede capturar
				Parameters.EnPassantCell = new CellModel(toRow, toColumn);
				// Asigna la celda que se puede capturar al paso
				if (piece.Color == PieceBaseModel.PieceColor.White)
					Parameters.EnPassantCellTarget = new CellModel(toRow + 1, toColumn);
				else
					Parameters.EnPassantCellTarget = new CellModel(toRow - 1, toColumn);
				// Asigna el color que se puede capturar al paso
				Parameters.EnPassantColor = piece.Color;
			}
			else
				Parameters.EnPassantCellTarget = null;
		}

		/// <summary>
		///		Promociona una pieza
		/// </summary>
		public bool Promote(int row, int column, PieceBaseModel.PieceType type)
		{
			bool canPromote = false;
			PieceBaseModel piece = this[row, column];

				if (piece != null && piece.Type == PieceBaseModel.PieceType.Pawn && piece.Color == Parameters.GetTurnColor() &&
					type != PieceBaseModel.PieceType.Pawn && type != PieceBaseModel.PieceType.King &&
					((piece.Color == PieceBaseModel.PieceColor.White && row == 0) ||
						(piece.Color == PieceBaseModel.PieceColor.Black && row == 7)))
				{
					// Promociona la pieza
					this[row, column] = CreatePiece(type, piece.Color);
					// Indica que se ha promocionado
					canPromote = true;
				}
				// Devuelve el valor que indica si ha podido promocionar
				return canPromote;
		}

		/// <summary>
		///		Busca la pieza de un color que se puede mover a una celda
		/// </summary>
		public (int row, int column, PieceBaseModel piece) SearchMoveTo(PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color, 
																		int fromRow, int fromColumn, int toRow, int toColumn)
		{
			List<(int row, int column, PieceBaseModel piece)> pieces = new List<(int row, int column, PieceBaseModel piece)>();

				// Busca las piezas
				for (int row = 0; row < 8; row++)
					for (int column = 0; column < 8; column++)
					{
						PieceBaseModel piece = this[row, column];

							if (piece != null && piece.Type == type && piece.Color == color && 
									piece.CanMoveTo(this, row, column, toRow, toColumn))
								pieces.Add((row, column, piece));
					}
				// Si se le ha pasado una fila / columna para ajustarlo, obtiene la pieza que estaba inicialmente en esa posición
				if (pieces.Count > 1 && (fromRow != -1 || fromColumn != -1))
					foreach ((int row, int column, PieceBaseModel piece) piece in pieces)
						if ((fromColumn == -1 && piece.row == fromRow) ||
								(fromRow == -1 && piece.column == fromColumn) ||
								(piece.row == fromRow && piece.column == fromColumn))
							return piece;
				// Devuelve la primera pieza localizada
				if (pieces.Count > 0)
					return pieces[0];
				else
					return (-1, -1, null);
		}

		/// <summary>
		///		Comprueba si se puede mover de un lugar a otro
		/// </summary>
		public bool CanMove(int fromRow, int fromColumn, int toRow, int toColumn)
		{
			bool canMove = false;

				// Si realmente hay un movimiento
				if (fromRow != toRow || fromColumn != toColumn)
				{
					PieceBaseModel pieceSource = this[fromRow, fromColumn];

						// Si hay una pieza en el origen del color que toca jugar y no hay nada en el destino o en el destino hay una pieza de otro color
						if (pieceSource != null && pieceSource.Color == Parameters.GetTurnColor() &&
								(this[toRow, toColumn] == null || this[toRow, toColumn].Color != pieceSource.Color))
							canMove = pieceSource.CanMoveTo(this, fromRow, fromColumn, toRow, toColumn);
				}
				// Devuelve el valor que indica si se puede mover
				return canMove;
		}

		/// <summary>
		///		Comprueba si se puede hacer un enroque corto
		/// </summary>
		public bool CanMoveCastleKingSide(PieceBaseModel.PieceColor color)
		{
			return color == Parameters.GetTurnColor() && Parameters.CanMoveCastleKingSide(color) && CanMoveCastle(color, 7);
		}

		/// <summary>
		///		Comprueba si se puede hacer un enroque largo
		/// </summary>
		public bool CanMoveCastleQueenSide(PieceBaseModel.PieceColor color)
		{
			return color == Parameters.GetTurnColor() && Parameters.CanMoveCastleQueenSide(color) && CanMoveCastle(color, 0);
		}

		/// <summary>
		///		Comprueba si se puede realizar un enroque (las piezas no se han movido y no hay nada entre medias)
		/// </summary>
		private bool CanMoveCastle(PieceBaseModel.PieceColor color, int column)
		{
			bool canMove = false;
			int row = GetRowKing(color);
			PieceBaseModel king = this[row, 4];
			PieceBaseModel rook = this[row, column];

				//TODO --> Debería comprobar además si las celdas intermedias no está en jaque
				// Comprueba el movimiento
				if (king != null && rook != null && !king.IsMoved && !rook.IsMoved)
					canMove = IsPathEmpty(row, 4, row, column);
				// Devuelve el valor que indica si se puede mover
				return canMove;
		}

		/// <summary>
		///		Obtiene la fila de rey de un color
		/// </summary>
		private int GetRowKing(PieceBaseModel.PieceColor color)
		{
			return color == PieceBaseModel.PieceColor.White ? 7 : 0;
		}

		/// <summary>
		///		Comprueba si está vacía el camino entre una fila y una columna
		/// </summary>
		internal bool IsPathEmpty(int fromRow, int fromColumn, int toRow, int toColumn)
		{
			int verticalSign = -1 * Math.Sign(fromRow - toRow);
			int horizontalSign = -1 * Math.Sign(fromColumn - toColumn);
			int row, column;

				// Comprueba el recorrido
				row = fromRow;
				column = fromColumn;
				do
				{
					// Comprueba si la celda está ocupada saltándose la primera celda ...
					if ((row != fromRow || column != fromColumn) && !IsEmpty(row, column))
						return false;
					// Incrementa fila / columna
					row += verticalSign;
					column += horizontalSign;
				}
				while (row != toRow || column != toColumn);
				// Si ha llegado hasta aquí es porque las celdas intermedias estaban vacías
				return true;
		}

		/// <summary>
		///		Comprueba si es legal colocar una pieza en una posición
		/// </summary>
		internal bool IsLegalMoveTo(PieceBaseModel piece, int row, int column)
		{
			PieceBaseModel target = this[row, column];

				return target == null || target.Color != piece.Color;
		}

		/// <summary>
		///		Comprueba si puede capturar una pieza
		/// </summary>
		internal bool CanCapture(PieceBaseModel piece, int row, int column)
		{
			PieceBaseModel target = this[row, column];

				return IsCaptureEnPassant(piece, row, column, target) || target != null && target.Color != piece.Color;
		}

		/// <summary>
		///		Comprueba si es una captura al paso
		/// </summary>
		public bool IsCaptureEnPassant(PieceBaseModel piece, int row, int column, PieceBaseModel target)
		{
			return piece is PawnModel && target == null && Parameters.EnPassantCellTarget != null && 
						Parameters.EnPassantCellTarget.Row == row && Parameters.EnPassantCellTarget.Column == column &&
						Parameters.EnPassantColor != piece.Color;
		}

		/// <summary>
		///		Comprueba si una celda está vacía
		/// </summary>
		internal bool IsEmpty(int row, int column)
		{
			return this[row, column] == null;
		}

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
						PieceBaseModel piece = this[row, column];

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
		///		Indexador por celda
		/// </summary>
		public PieceBaseModel this[CellModel cell]
		{
			get { return this[cell.Row, cell.Column]; }
			private set { this[cell.Row, cell.Column] = value; }
		}

		/// <summary>
		///		Indexador por fila y columna
		/// </summary>
		public PieceBaseModel this[int row, int column]
		{
			get { return Cells[8 * row + column]; }
			private set { Cells[8 * row + column] = value; }
		}

		/// <summary>
		///		Celdas de un tablero
		/// </summary>
		private PieceBaseModel[] Cells { get; } = new PieceBaseModel[64];

		/// <summary>
		///		Parámetros del tablero
		/// </summary>
		public SquareParametersModel Parameters { get; private set; } = new SquareParametersModel();
	}
}
