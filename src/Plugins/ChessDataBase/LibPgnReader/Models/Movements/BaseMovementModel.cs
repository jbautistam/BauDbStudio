using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibPgnReader.Models.Movements
{
	/// <summary>
	///		Base del movimiento
	/// </summary>
	public abstract class BaseMovementModel
	{
		protected BaseMovementModel(TurnModel turn, string content)
		{
			Turn = turn;
			Content = content;
		}

		/// <summary>
		///		Datos del turno
		/// </summary>
		public TurnModel Turn { get; protected set; }

		/// <summary>
		///		Contenido del movimiento
		/// </summary>
		public string Content { get; }

		/// <summary>
		///		Comentario
		/// </summary>
		public List<string> Comments { get; } = new List<string>();

		/// <summary>
		///		Información adicional asociada al movimiento
		/// </summary>
		public List<InfoMovementModel> Info { get; } = new List<InfoMovementModel>();

		/// <summary>
		///		Variaciones a este movimiento parcial
		/// </summary>
		public List<VariationModel> Variations { get; } = new List<VariationModel>();
	}
}
