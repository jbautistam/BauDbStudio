using System;
using System.Collections.Generic;

using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes
{
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
						Scapes.Add(new CellViewModel(row, column, color));
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
		///		ViewModel del tablero
		/// </summary>
		public GameBoardViewModel ViewModel { get; }

		/// <summary>
		///		Escaques del tablero
		/// </summary>
		public List<ScapeBaseViewModel> Scapes { get; } = new List<ScapeBaseViewModel>();
	}
}
