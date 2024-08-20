using System;

namespace Bau.Libraries.ChessDataBase.Models.Board.Movements
{
	/// <summary>
	///		Variación de una partida
	/// </summary>
	public class MovementVariationModel
	{	
		public MovementVariationModel(MovementBaseModel parent)
		{
			Parent = parent;
		}

		/// <summary>
		///		Clona una variación
		/// </summary>
		public MovementVariationModel Clone(MovementBaseModel parent)
		{
			MovementVariationModel target = new MovementVariationModel(parent);

				// Clona las propiedades
				target.Id = Id;
				// Clona los movimientos
				target.Movements.AddRange(Movements.Clone());
				// Devuelve la variación clonada
				return target;
		}

		/// <summary>
		///		Clona hasta una variación
		/// </summary>
		public MovementVariationModel CloneTo(MovementBaseModel parent, MovementVariationModel target, MovementFigureModel movement)
		{
			MovementVariationModel variation = new MovementVariationModel(parent);

				// Clona los movimientos
				variation.Movements.AddRange(Movements.CloneTo(variation, target, movement));
				// Devuelve la variación clonada
				return variation;
		}

		/// <summary>
		///		Movimiento padre
		/// </summary>
		public MovementBaseModel Parent { get; }

		/// <summary>
		///		Id de la variación
		/// </summary>
		public Guid Id { get; private set; } = Guid.NewGuid();

		/// <summary>
		///		Movimientos hijo
		/// </summary>
		public MovementModelCollection Movements { get; } = new MovementModelCollection();
	}
}
