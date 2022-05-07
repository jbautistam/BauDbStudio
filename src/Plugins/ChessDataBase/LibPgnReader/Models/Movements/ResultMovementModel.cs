using System;

namespace Bau.Libraries.LibPgnReader.Models.Movements
{
	/// <summary>
	///		Clase con los datos del resultado de la partida
	/// </summary>
	public class ResultMovementModel : BaseMovementModel
	{
		/// <summary>
		///		Tipo de resultado
		/// </summary>
		public enum ResultType
		{
			/// <summary>Ganan las blancas</summary>
			WhiteWins,
			/// <summary>Ganan las negras</summary>
			BlackWins,
			/// <summary>Empate</summary>
			Drawn,
			/// <summary>Juego abandonado</summary>
			Abandoned
		}

		public ResultMovementModel(TurnModel turn, string content) : base(turn, content) {}

		/// <summary>
		///		Resultado del juego
		/// </summary>
		public ResultType Result
		{
			get
			{
				switch (Content)
				{
					case "1-0":
						return ResultType.WhiteWins;
					case "0-1":
						return ResultType.BlackWins;
					case "1/2-1/2":
						return ResultType.Drawn;
					default:
						return ResultType.Abandoned;
				}
			}
		}
	}
}
