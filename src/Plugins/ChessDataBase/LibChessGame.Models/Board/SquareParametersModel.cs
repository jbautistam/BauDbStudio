using System;

using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Models.Board
{
	/// <summary>
	///		Parámetros del tablero
	/// </summary>
	public class SquareParametersModel
	{
		/// <summary>
		///		Limpia los datos
		/// </summary>
		public void Clear()
		{
			IsWhiteMove = true;
			CanWhiteCastleKingSide = true;
			CanWhiteCastleQueenSide = true;
			CanBlackCastleKingSide = true;
			CanBlackCastleQueenSide = true;
			EnPassantCellTarget = null;
			HalfMoveClock = 0;
			FullMoveCount = 0;
		}

		/// <summary>
		///		Clona los parámetros
		/// </summary>
		public SquareParametersModel Clone()
		{
			SquareParametersModel target = new SquareParametersModel();

				// Asigna las propiedades
				target.IsWhiteMove = IsWhiteMove;
				target.CanWhiteCastleKingSide = CanWhiteCastleKingSide;
				target.CanWhiteCastleQueenSide = CanWhiteCastleQueenSide;
				target.CanBlackCastleKingSide = CanBlackCastleKingSide;
				target.CanBlackCastleQueenSide = CanBlackCastleQueenSide;
				if (EnPassantCellTarget != null)
					target.EnPassantCellTarget = new CellModel(EnPassantCellTarget.Row, EnPassantCellTarget.Column);
				target.HalfMoveClock = HalfMoveClock;
				target.FullMoveCount = FullMoveCount;
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Inhabilita los enroques
		/// </summary>
		internal void DisableCastle(PieceBaseModel.PieceColor color)
		{
			DisableCastleKingSide(color);
			DisableCastleQueenSide(color);
		}

		/// <summary>
		///		Habilita los enroques
		/// </summary>
		internal void EnableCastle(PieceBaseModel.PieceColor color)
		{
			if (color == PieceBaseModel.PieceColor.White)
			{
				CanWhiteCastleKingSide = true;
				CanWhiteCastleQueenSide = true;
			}
			else
			{
				CanBlackCastleKingSide = true;
				CanBlackCastleQueenSide = true;
			}
		}

		/// <summary>
		///		Comprueba si se puede hacer un enroque corto
		/// </summary>
		internal bool CanMoveCastleKingSide(PieceBaseModel.PieceColor color)
		{
			if (color == PieceBaseModel.PieceColor.White)
				return CanWhiteCastleKingSide;
			else
				return CanBlackCastleKingSide;
		}

		/// <summary>
		///		Inhabilita el enroque corto
		/// </summary>
		internal void DisableCastleKingSide(PieceBaseModel.PieceColor color)
		{
			if (color == PieceBaseModel.PieceColor.White)
				CanWhiteCastleKingSide = false;
			else
				CanBlackCastleKingSide = false;
		}

		/// <summary>
		///		Comprueba si se puede hacer un enroque corto
		/// </summary>
		internal bool CanMoveCastleQueenSide(PieceBaseModel.PieceColor color)
		{
			if (color == PieceBaseModel.PieceColor.White)
				return CanWhiteCastleQueenSide;
			else
				return CanBlackCastleQueenSide;
		}

		/// <summary>
		///		Inhabilita el enroque largo
		/// </summary>
		internal void DisableCastleQueenSide(PieceBaseModel.PieceColor color)
		{
			if (color == PieceBaseModel.PieceColor.White)
				CanWhiteCastleQueenSide = false;
			else
				CanBlackCastleQueenSide = false;
		}

		/// <summary>
		///		Obtiene el color del turno
		/// </summary>
		public PieceBaseModel.PieceColor GetTurnColor()
		{
			if (IsWhiteMove)
				return PieceBaseModel.PieceColor.White;
			else
				return PieceBaseModel.PieceColor.Black;
		}

        /// <summary>
        ///		Indica si es un movimiento de blancas
        /// </summary>
        public bool IsWhiteMove { get; set; }

        /// <summary>
        ///		Indica si las blancas pueden enrocar por el lado de rey (enroque corto)
        /// </summary>
        public bool CanWhiteCastleKingSide { get; set; }

        /// <summary>
        ///		Indica si las blancas pueden enrocar por el lado de dama (enroque largo)
        /// </summary>
        public bool CanWhiteCastleQueenSide { get; set; }

        /// <summary>
        ///		Indica si las negras pueden enrocar por el lado de rey (enroque corto)
        /// </summary>
        public bool CanBlackCastleKingSide { get; set; }

        /// <summary>
        ///		Indica si las negras pueden enrocar por el lado de dama (enroque largo)
        /// </summary>
        public bool CanBlackCastleQueenSide { get; set; }

		/// <summary>
		///		Celda del peón que se puede capturar al paso (si hay alguno)
		/// </summary>
        public CellModel EnPassantCellTarget { get; set; }

		/// <summary>
		///		Celda donde se coloca el peón que se puede capturar al paso (si hay alguno)
		/// </summary>
        public CellModel EnPassantCell { get; set; }

		/// <summary>
		///		Color de la pieza que se puede capturar al paso (si hay alguno)
		/// </summary>
		public PieceBaseModel.PieceColor EnPassantColor { get; set; }

        /// <summary>
		///		Número de movimientos (por jugador) antes de este movimiento
        /// </summary>
        public int HalfMoveClock { get; set; }

        /// <summary>
        ///		Número de movimientos total
        /// </summary>
        public int FullMoveCount { get; set; }
	}
}
