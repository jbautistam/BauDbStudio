using System;

namespace Bau.Libraries.LibPgnReader.Models.Movements
{
	/// <summary>
	///		Clase con los datos de un turno
	/// </summary>
	public class TurnModel
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

		public TurnModel(string number, TurnType type)
		{
			Number = number;
			Type = type;
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
