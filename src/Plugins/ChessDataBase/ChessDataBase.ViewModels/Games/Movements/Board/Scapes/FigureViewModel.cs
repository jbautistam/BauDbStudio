using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes;

/// <summary>
///		ViewModel para las figuras en el tablero
/// </summary>
public class FigureViewModel : ScapeBaseViewModel
{
	// Variables privadas
	private PieceBaseModel.PieceType _type;
	private string _fileImage = default!;

	public FigureViewModel(int row, int column, PieceBaseModel.PieceType type, PieceBaseModel.PieceColor color) : base(row, column, color)
	{
		Type = type;
	}

	/// <summary>
	///		Modifica el nombre de archivo
	/// </summary>
	private void UpdateFileImage()
	{
		string pieceColor = Color == PieceBaseModel.PieceColor.White ? "White" : "Black";

			switch (Type)
			{
				case PieceBaseModel.PieceType.Pawn:
						FileImage = $"Pawn{pieceColor}.gif";
					break;
				case PieceBaseModel.PieceType.Rook:
						FileImage = $"Rook{pieceColor}.gif";
					break;
				case PieceBaseModel.PieceType.Knight:
						FileImage = $"Knight{pieceColor}.gif";
					break;
				case PieceBaseModel.PieceType.Bishop:
						FileImage = $"Bishop{pieceColor}.gif";
					break;
				case PieceBaseModel.PieceType.King:
						FileImage = $"King{pieceColor}.gif";
					break;
				case PieceBaseModel.PieceType.Queen:
						FileImage = $"Queen{pieceColor}.gif";
					break;
				default:
						FileImage = string.Empty;
					break;
			}
	}

	/// <summary>
	///		Tipo de pieza
	/// </summary>
	public PieceBaseModel.PieceType Type
	{
		get { return _type; }
		set 
		{ 
			if (CheckProperty(ref _type, value))
				UpdateFileImage();
		}
	}

	/// <summary>
	///		Archivo de imagen
	/// </summary>
	public string FileImage
	{
		get { return _fileImage; }
		set { CheckProperty(ref _fileImage, value); }
	}
}