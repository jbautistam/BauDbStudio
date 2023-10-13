using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Games;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements;

/// <summary>
///		ViewModel para el resultado de un movimiento
/// </summary>
public class MovementGameEndViewModel : BaseMovementViewModel
{
	// Variables privadas
	private string _text = default!;

	public MovementGameEndViewModel(MovementGameEndModel movement)
	{
		Movement = movement;
		switch (movement.Result)
		{ 
			case GameModel.ResultType.WhiteWins:
					Text = "White wins";
				break;
			case GameModel.ResultType.BlackWins:
					Text = "Black wins";
				break;
			case GameModel.ResultType.Draw:
					Text = "Draw";
				break;
			default:
					Text = "Open play";
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
