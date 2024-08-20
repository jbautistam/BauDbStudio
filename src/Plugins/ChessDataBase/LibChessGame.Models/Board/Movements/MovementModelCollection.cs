using System;
using System.Collections.Generic;

namespace Bau.Libraries.ChessDataBase.Models.Board.Movements
{
	/// <summary>
	///		Colección de <see cref="MovementFigureModel"/>
	/// </summary>
	public class MovementModelCollection : List<MovementBaseModel>
	{
		/// <summary>
		///		Busca el último movimiento de una pieza en una colección
		/// </summary>
		internal MovementFigureModel SearchLastPieceMovement()
		{
			// Busca el último movimiento de pieza
			for (int index = Count - 1; index >= 0; index--)
				if (this[index] is MovementFigureModel movement)
					return movement;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Clona los movimientos
		/// </summary>
		public MovementModelCollection Clone()
		{
			MovementModelCollection target = new MovementModelCollection();

				// Clona los datos
				foreach (MovementBaseModel movement in this)
					target.Add(movement.Clone());
				// Devuelve la colección clonada
				return target;
		}

		/// <summary>
		///		Clona los movimientos hasta una variación
		/// </summary>
		internal MovementModelCollection CloneTo(MovementVariationModel variation, MovementVariationModel targetVariation, 
												 MovementFigureModel targetMovement)
		{
			MovementModelCollection movements = new MovementModelCollection();
			bool variationFound = false;

				// Clona los datos
				foreach (MovementBaseModel movement in this)
					if (!variationFound)
					{
						// Busca la variación entre las variaciones del movimiento
						foreach (MovementVariationModel variationChild in movement.Variations)
							if (variationChild.Id == targetVariation.Id)
							{
								// Añade los movimientos de la variación
								foreach (MovementBaseModel movementBase in variationChild.Movements)
									movements.Add(movementBase.Clone());
								// Indica que se ha encontrado la variación y no se debe seguir clonando
								variationFound = true;
							}
						// Si no se ha encontrado la variación, se sigue clonando, si se ha encontrado, se sustituye el
						// movimiento por el destino y se deja de clonar
						if (!variationFound)
							movements.Add(movement.Clone());
					}
				// Devuelve la colección clonada
				return movements;
		}
	}
}
