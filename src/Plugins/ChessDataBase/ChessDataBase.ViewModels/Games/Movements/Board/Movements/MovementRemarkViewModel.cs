namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements;

/// <summary>
///		ViewModel para un movimiento de comentario
/// </summary>
public class MovementRemarkViewModel : BaseMovementViewModel
{
	// Variables privadas
	private string _text = default!;

	public MovementRemarkViewModel(string text)
	{
		Text = text;
	}

	/// <summary>
	///		Texto del comentario
	/// </summary>
	public string Text
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}
}
