using System.Windows.Data;

using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.Plugin.Converters;

/// <summary>
///		Conversor para los iconos asociados a los archivos del árbol de secciones
/// </summary>
public class FileChessPieceConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is MovementFigureModel movement)
			return GetIcon(movement);
		else
			return null;
	}

	/// <summary>
	///		Obtiene el icono asociado a un <see cref="MovementFigureModel"/>
	/// </summary>
	private object? GetIcon(MovementFigureModel movement)
	{	
		string nameColor = "Black";
		string namePiece = string.Empty;

			// Obtiene el nombre del color
			if (movement.Color == PieceBaseModel.PieceColor.White)
				nameColor = "White";
			// Obtiene el nombre de la pieza
			switch (movement.OriginPiece)
			{ 
				case PieceBaseModel.PieceType.Pawn:
						namePiece = "Pawn";
					break;
				case PieceBaseModel.PieceType.Rook:
						namePiece = "Rook";
					break;
				case PieceBaseModel.PieceType.Knight:
						namePiece = "Knight";
					break;
				case PieceBaseModel.PieceType.Bishop:
						namePiece = "Bishop";
					break;
				case PieceBaseModel.PieceType.Queen:
						namePiece = "Queen";
					break;
				case PieceBaseModel.PieceType.King:
						namePiece = "King";
					break;
			}
			// Obtiene el nombre de la imagen
			if (!string.IsNullOrEmpty(nameColor) && !string.IsNullOrEmpty(namePiece))
				return ChessDataBasePlugin.ImagesCache.GetImage($"pack://application:,,,/ChessDataBase.Plugin;component/Resources/ChessBoard/{namePiece}{nameColor}.gif", true);
			else
				return null;
	}

	/// <summary>
	///		Convierte un valor de vuelta
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		throw new NotImplementedException();
	}
}