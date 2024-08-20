using System;

namespace Bau.Libraries.ChessDataBase.Models.Board.Movements
{
	/// <summary>
	///		Datos de un turno
	/// </summary>
	public class MovementTurnModel
	{
		/// <summary>
		///		Tipo de turno
		/// </summary>
		public enum TurnType
		{
			/// <summary>Blancas</summary>
			White,
			/// <summary>Negras</summary>
			Black,
			/// <summary>Final de partida</summary>
			End
		}

		public MovementTurnModel(string number, TurnType type)
		{
			Number = number;
			Type = type;
		}

		/// <summary>
		///		Convierte el turno a una cadena
		/// </summary>
		public override string ToString()
		{
			return $"{Number}. {Type.ToString()}";
		}

		/// <summary>
		///		Número de movimiento
		/// </summary>
		public string Number { get; }

		/// <summary>
		///		Tipo de movimiento
		/// </summary>
		public TurnType Type { get; }
	}
}
