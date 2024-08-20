using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements;

/// <summary>
///		Lista de movimientos
/// </summary>
public class MovementListViewModel : BaseObservableObject
{
	// Variables privadas
	private BaseMovementViewModel? _selectedMovement;
	private MovementFigureViewModel? _actualMovement, _nextMovement;

	public MovementListViewModel(GameBoardViewModel gameBoardViewModel)
	{
		GameBoardViewModel = gameBoardViewModel;
	}

	/// <summary>
	///		Carga los movimientos de la variación
	/// </summary>
	internal void LoadMovements(List<BaseMovementViewModel> movements)
	{
		// Limpia los movimientos
		Movements.Clear();
		// Inicializa los movimientos
		if (movements is null || movements.Count == 0)
			Movements.Add(new MovementRemarkViewModel("No hay ningún movimiento en este juego"));
		else
			LoadVariationMovements(movements);
	}

	/// <summary>
	///		Carga en la lista una variación de movimientos
	/// </summary>
	private void LoadVariationMovements(List<BaseMovementViewModel> movements)
	{
		MovementFigureDoubleViewModel? lastMovement = null;
		int movementIndex = 1;

			foreach (BaseMovementViewModel movement in movements)
				switch (movement)
				{
					case MovementFigureViewModel move:
							// Añade a la lista un movimiento real o un movimiento recursivo
							if (lastMovement == null)
							{
								lastMovement = new MovementFigureDoubleViewModel(movementIndex);
								Movements.Add(lastMovement);
							}
							// Asigna el movimiento de negras o blancas
							if (move.Color == Models.Pieces.PieceBaseModel.PieceColor.Black)
							{
								// Asigna el movimiento de negras
								lastMovement.BlackMovement = move;
								// Incrementa el índice y vacía el último movimiento
								movementIndex++;
								lastMovement = null;
							}
							else
								lastMovement.WhiteMovement = move;
						break;
					case MovementGameEndViewModel move:
							Movements.Add(move);
							lastMovement = null;
						break;
				}
	}

	/// <summary>
	///		Selecciona un movimiento en la lista
	/// </summary>
	internal void SelectMovement(MovementFigureModel movement)
	{
		// Obtiene el movimiento actual
		if (movement == null)
		{
			SelectedMovement = null;
			ActualMovement = null;
		}
		else
			foreach (BaseMovementViewModel baseMovement in Movements)
				if (baseMovement is MovementFigureDoubleViewModel movementDouble)
				{
					// Selecciona el movimiento blanco
					if (movementDouble.WhiteMovement != null)
					{
						movementDouble.WhiteMovement.Selected = movementDouble.WhiteMovement.Movement.Id == movement.Id;
						if (movementDouble.WhiteMovement.Selected)
						{
							SelectedMovement = movementDouble;
							ActualMovement = movementDouble.WhiteMovement;
						}
					}
					// Selecciona el movimiento negro
					if (movementDouble.BlackMovement != null)
					{
						movementDouble.BlackMovement.Selected = movementDouble.BlackMovement.Movement.Id == movement.Id;
						if (movementDouble.BlackMovement.Selected)
						{
							SelectedMovement = movementDouble;
							ActualMovement = movementDouble.BlackMovement;
						}
					}
				}
		// Asigna el siguiente movimiento
		AssignNextMovement(movement);
	}

	/// <summary>
	///		Asigna el siguiente movimiento
	/// </summary>
	private void AssignNextMovement(MovementFigureModel? movement)
	{
		if (movement is null)
			NextMovement = null;
		else
			NextMovement = new MovementFigureViewModel(this, movement);
	}

	/// <summary>
	///		ViewModel del tablero
	/// </summary>
	public GameBoardViewModel GameBoardViewModel { get; }

	/// <summary>
	///		Movimientos para mostrar en la vista: combina movimientos con comentarios y resultados
	/// </summary>
	public ObservableCollection<BaseMovementViewModel> Movements { get; } = new();

	/// <summary>
	///		Movimiento seleccionado
	/// </summary>
	public BaseMovementViewModel? SelectedMovement
	{
		get { return _selectedMovement; }
		set { CheckObject(ref _selectedMovement, value); }
	}

	/// <summary>
	///		Movimiento actual
	/// </summary>
	public MovementFigureViewModel? ActualMovement
	{
		get { return _actualMovement; }
		set { CheckObject(ref _actualMovement, value); }
	}

	/// <summary>
	///		Movimiento actual
	/// </summary>
	public MovementFigureViewModel? NextMovement
	{
		get { return _nextMovement; }
		set { CheckObject(ref _nextMovement, value); }
	}
}