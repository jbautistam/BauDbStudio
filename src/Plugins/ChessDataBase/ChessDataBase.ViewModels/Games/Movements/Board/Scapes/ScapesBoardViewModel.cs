using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes;

/// <summary>
///		Escaques del tablero
/// </summary>
public class ScapesBoardViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	public ScapesBoardViewModel(GameBoardViewModel viewModel)
	{
		ViewModel = viewModel;
	}

	/// <summary>
	///		Inicializa el tablero
	/// </summary>
	internal void Reset()
	{
		// Limpia los datos
		Scapes.Clear();
		// Inicializa las celdas
		InitCells();
		InitLabels();
		// Inicializa las figuras
		InitPieces();
	}

	/// <summary>
	///		Inicaliza las celdas
	/// </summary>
	private void InitCells()
	{
		PieceBaseModel.PieceColor color = PieceBaseModel.PieceColor.White;

			// Añade las celdas
			for (int row = 0; row < 8; row++)
			{
				// Rellena por columnas
				for (int column = 0; column < 8; column++)
				{
					Scapes.Add(new CellViewModel(row, column, color, CellViewModel.StatusCell.Unselected, 0));
					color = GetNextColor(color);
				}
				// Cambia el color de inicio de la siguiente fila
				color = GetNextColor(color);
			}
	}

	/// <summary>
	///		Obtiene el siguiente color
	/// </summary>
	private PieceBaseModel.PieceColor GetNextColor(PieceBaseModel.PieceColor color)
	{
		return color == PieceBaseModel.PieceColor.White ? PieceBaseModel.PieceColor.Black : PieceBaseModel.PieceColor.White;
	}

	/// <summary>
	///		Inicializa las etiquetas
	/// </summary>
	private void InitLabels()
	{
		for (int row = 0; row < 8; row++)
			Scapes.Add(new LabelViewModel(row, -1, (char) ('0' + 8 - row)));
		for (int column = 0; column < 8; column++)
			Scapes.Add(new LabelViewModel(-1, column, (char) ('A' + column)));
	}

	/// <summary>
	///		Inicializa las piezas
	/// </summary>
	private void InitPieces()
	{
		for (int row = 0; row < 8; row++)
			for (int column = 0; column < 8; column++)
			{
				PieceBaseModel piece = ViewModel.Board[row, column];

					if (piece != null)
						Scapes.Add(new FigureViewModel(row, column, piece.Type, piece.Color));
			}
	}

	/// <summary>
	///		Limpia las celadas seleccionadas
	/// </summary>
	public void CleanSelectedCells()
	{
		foreach (ScapeBaseViewModel scape in Scapes)
			if (scape is CellViewModel cell)
				cell.Status = CellViewModel.StatusCell.Unselected;
	}

	/// <summary>
	///		Selecciona una celda
	/// </summary>
	public void SelectCell(int row, int column, CellViewModel.StatusCell status)
	{
		foreach (ScapeBaseViewModel scape in Scapes)
			if (scape is CellViewModel cell && cell.Row == row && cell.Column == column)
			{
				// Se deselecciona si ya estaba seleccionada
				if (status == CellViewModel.StatusCell.Selected && cell.Status == CellViewModel.StatusCell.Selected)
					status = CellViewModel.StatusCell.Unselected;
				// Cambia el estado
				cell.Status = status;
			}
	}

	/// <summary>
	///		Obtiene las celdas con determinado estado
	/// </summary>
	public List<CellViewModel> GetCellsWithStatus(CellViewModel.StatusCell status)
	{
		List<CellViewModel> cells = new();

			// Busca las celdas con el estado
			foreach (ScapeBaseViewModel scape in Scapes)
				if (scape is CellViewModel cell && cell.Status == status)
					cells.Add(cell);
			// Devuelve la lista de celdas
			return cells;
	}

	/// <summary>
	///		ViewModel del tablero
	/// </summary>
	public GameBoardViewModel ViewModel { get; }

	/// <summary>
	///		Escaques del tablero
	/// </summary>
	public List<ScapeBaseViewModel> Scapes { get; } = new();
}
