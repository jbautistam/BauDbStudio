using System;

namespace Bau.Libraries.LibPgnReader.Models.Movements
{
	/// <summary>
	///		Clase con los datos de un movimiento parcial
	/// </summary>
	public class PieceMovementModel : BaseMovementModel
	{
		public PieceMovementModel(TurnModel turn, string content) : base(turn, content) {}

		/// <summary>
		///		Cambia el tipo del movimiento
		/// </summary>
		internal void SetType(TurnModel.TurnType turnType)
		{
			Turn = new TurnModel(Turn.Number, turnType);
		}
	}
}
