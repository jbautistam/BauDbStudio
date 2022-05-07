using System;

using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Models.Board.Movements
{
	/// <summary>
	///		Datos de un movimiento
	/// </summary>
	public class MovementFigureModel : MovementBaseModel
	{
		/// <summary>
		///		Tipo de movimiento
		/// </summary>
		public enum MovementType
		{
			/// <summary>Normal</summary>
			Normal,
			/// <summary>Captura</summary>
			Capture,
			/// <summary>Captura en passant</summary>
			CaptureEnPassant,
			/// <summary>Enroque corto</summary>
			CastleKingSide,
			/// <summary>Enroque largo</summary>
			CastleQueenSide
		}

		public MovementFigureModel(MovementTurnModel turn, string content) : base(turn, content) {}

		/// <summary>
		///		Clona los datos
		/// </summary>
		public override MovementBaseModel Clone()
		{
			MovementFigureModel target = new MovementFigureModel(new MovementTurnModel(Turn.Number, Turn.Type), Content);

				// Clona los datos internos
				CloneInner(target);
				// Clona las propiedades
				target.Color = Color;
				target.Type = Type;
				target.OriginPiece = OriginPiece;
				target.From = new CellModel(From);
				target.To = new CellModel(To);
				target.PromotedPiece = PromotedPiece;
				target.Captured = Captured?.Clone();
				if (CapturedEnPassant != null)
					target.CapturedEnPassant = new CellModel(CapturedEnPassant);
				target.IsCheck = IsCheck;
				target.IsDoubleCheck = IsDoubleCheck;
				target.IsCheck = IsCheckMate;
				target.IsDrawOffered = IsDrawOffered;
				// Devuelve el objeto
				return target;
		}

		/// <summary>
		///		Convierte los datos del movimiento en una cadena
		/// </summary>
		public override string ToString()
		{
			string converted = $"{Turn} {Content} - {OriginPiece.ToString()} {Color.ToString()} ({Type.ToString()})";

				// Añade las posiciones
				if (From != null)
					converted += $" From {From}";
				if (To != null)
					converted += $" To {To}";
				// Añade los datos
				if (PromotedPiece != null)
					converted += $" Promoted to {PromotedPiece.ToString()}";
				if (IsCheck)
					converted += " Check";
				if (IsDoubleCheck)
					converted += " Double check";
				if (IsCheckMate)
					converted += " Check mate";
				if (IsDrawOffered)
					converted += " Draw offered";
				// Devuelve la cadena final
				return converted;
		}

		/// <summary>
		///		Color
		/// </summary>
		public PieceBaseModel.PieceColor Color { get; set; }

		/// <summary>
		///		Tipo de movimiento
		/// </summary>
		public MovementType Type { get; set; }

		/// <summary>
		///		Pieza movida
		/// </summary>
		public PieceBaseModel.PieceType OriginPiece { get; set; }

		/// <summary>
		///		Celda origen
		/// </summary>
		public CellModel From { get; set; }

		/// <summary>
		///		Celda destino
		/// </summary>
		public CellModel To { get; set; }

		/// <summary>
		///		Pieza promocionada
		/// </summary>
		public PieceBaseModel.PieceType? PromotedPiece { get; set; }

		/// <summary>
		///		Pieza capturada
		/// </summary>
		public PieceBaseModel Captured { get; set; }

		/// <summary>
		///		Celda donde se encuentra el peón cuando es una captura al paso
		/// </summary>
		public CellModel CapturedEnPassant { get; set; }

		/// <summary>
		///		Jaque
		/// </summary>
		public bool IsCheck { get; set; }

		/// <summary>
		///		Jaque doble
		/// </summary>
		public bool IsDoubleCheck { get; set; }

		/// <summary>
		///		Jaque mate
		/// </summary>
		public bool IsCheckMate { get; set; }

		/// <summary>
		///		El jugador ofrece tablas
		/// </summary>
		public bool IsDrawOffered { get; set; }
	}
}
