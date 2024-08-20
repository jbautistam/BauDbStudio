using System.Collections.ObjectModel;

using Bau.Libraries.ChessDataBase.Models.Board.Movements;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements;

/// <summary>
///		Lista de selección de variaciones
/// </summary>
public class MovementListSelectVariationViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private ObservableCollection<MovementSelectVariationViewModel> _variations = default!;

	public MovementListSelectVariationViewModel(GameBoardViewModel gameBoardViewModel, MovementFigureModel movement)
	{
		// Crea la colección
		Variations = new ObservableCollection<MovementSelectVariationViewModel>();
		// Añade las variaciones que se pueden seleccionar
		foreach (MovementVariationModel variation in movement.Variations)
		{
			bool isAdded = false;

				// Añade el primer movimiento de figura de la variación
				foreach (MovementBaseModel movementVariation in variation.Movements)
					if (!isAdded && movementVariation is MovementFigureModel movementFigure)
					{
						// Añade la variación
						Variations.Add(new MovementSelectVariationViewModel(gameBoardViewModel, variation, movementFigure));
						// Indica que se ha añadido
						isAdded = true;
					}
		}
	}

	/// <summary>
	///		Variaciones
	/// </summary>
	public ObservableCollection<MovementSelectVariationViewModel> Variations
	{
		get { return _variations; }
		set { CheckObject(ref _variations, value); }
	}
}
