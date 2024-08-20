using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes;

/// <summary>
///		ViewModel para las celdas en el tablero
/// </summary>
public class CellViewModel : ScapeBaseViewModel
{
	// Enumerados privados
	public enum StatusCell
	{
		/// <summary>No seleccionada</summary>
		Unselected,
		/// <summary>Seleccionada por el usuario</summary>
		Selected
	};
	// Variables privadas
	private string _fileImage = default!;
	private StatusCell _status = default!;
	private int _selectPosition;

	public CellViewModel(int row, int column, PieceBaseModel.PieceColor color, StatusCell status, int selectPosition) : base(row, column, color) 
	{
		Status = status;
		FileImage = GetFileImage();
	}

	/// <summary>
	///		Obtiene el archivo de imagen
	/// </summary>
	private string GetFileImage()
	{
		string status = string.Empty;

			// Cambia el sufijo con el estado
			if (Status == StatusCell.Selected)
				status = "selected";
			// Devuelve la imagen de la celda
			if (Color == PieceBaseModel.PieceColor.White)
				return $"BoardLight{status}.gif";
			else
				return $"BoardDark{status}.gif";
	}

	/// <summary>
	///		Estado de la celda
	/// </summary>
	public StatusCell Status
	{
		get { return _status; }
		set 
		{ 
			if (CheckProperty(ref _status, value))
				FileImage = GetFileImage();
		}
	}

	/// <summary>
	///		Orden en el que se ha seleccionado esta celda
	/// </summary>
	public int SelectPosition
	{
		get { return _selectPosition; }
		set { CheckProperty(ref _selectPosition, value); }
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
