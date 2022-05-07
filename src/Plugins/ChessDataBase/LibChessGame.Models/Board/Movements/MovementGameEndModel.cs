using System;

namespace Bau.Libraries.ChessDataBase.Models.Board.Movements
{
	/// <summary>
	///		Clase con los datos del resultado final de un juego
	/// </summary>
	public class MovementGameEndModel : MovementBaseModel
	{
		public MovementGameEndModel(MovementTurnModel turn, string content) : base(turn, content) {}

		/// <summary>
		///		Clona los datos
		/// </summary>
		public override MovementBaseModel Clone()
		{
			MovementGameEndModel target = new MovementGameEndModel(new MovementTurnModel(Turn.Number, Turn.Type), Content);

				// Asigna las propiedades básicas
				CloneInner(target);
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Resultado del juego
		/// </summary>
		public Games.GameModel.ResultType Result
		{
			get
			{
				switch (Content)
				{
					case "1-0":
						return Games.GameModel.ResultType.WhiteWins;
					case "0-1":
						return Games.GameModel.ResultType.BlackWins;
					case "1/2-1/2":
						return Games.GameModel.ResultType.Draw;
					default:
						return Games.GameModel.ResultType.Open;
				}
			}
		}
	}
}
