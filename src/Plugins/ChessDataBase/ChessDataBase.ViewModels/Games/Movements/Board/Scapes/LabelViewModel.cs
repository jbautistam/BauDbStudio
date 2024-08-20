using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes;

/// <summary>
///		ViewModel para las etiquetas del tablero
/// </summary>
public class LabelViewModel : ScapeBaseViewModel
{
	// Variables privadas
	private char _text;

	public LabelViewModel(int row, int column, char text) : base(row, column, PieceBaseModel.PieceColor.Black) 
	{
		Text = text;
	}

	/// <summary>
	///		Texto de la etiqueta
	/// </summary>
	public char Text
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}
}
