using System;

using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Games;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements
{
	/// <summary>
	///		ViewModel para el resultado de un movimiento
	/// </summary>
	public class MovementGameEndViewModel : BaseMovementViewModel
	{
		// Variables privadas
		private string _text;

		public MovementGameEndViewModel(MovementGameEndModel movement)
		{
			Movement = movement;
			switch (movement.Result)
			{ 
				case GameModel.ResultType.WhiteWins:
						Text = "Ganan las blancas";
					break;
				case GameModel.ResultType.BlackWins:
						Text = "Ganan las negras";
					break;
				case GameModel.ResultType.Draw:
						Text = "Empate";
					break;
				default:
						Text = "Juego abierto";
					break;
			}
		}

		/// <summary>
		///		Movimiento
		/// </summary>
		public MovementGameEndModel Movement { get; }

		/// <summary>
		///		Texto del comentario
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { CheckProperty(ref _text, value); }
		}
	}
}
