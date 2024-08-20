using System;
using System.Collections.Generic;

namespace Bau.Libraries.ChessDataBase.Models.Board.Movements
{
	/// <summary>
	///		Base para los movimientos
	/// </summary>
	public abstract class MovementBaseModel
	{
		protected MovementBaseModel(MovementTurnModel turn, string content)
		{
			Turn = turn;
			Content = content;
		}

		/// <summary>
		///		Clona los datos de un objeto
		/// </summary>
		public abstract MovementBaseModel Clone();

		/// <summary>
		///		Clona los datos internos
		/// </summary>
		protected void CloneInner(MovementBaseModel target)
		{
			// Clona las propiedades
			target.Id = Id;
			// Clona los comentarios
			foreach (string comment in Comments)
				target.Comments.Add(comment);
			// Clona la información adicional
			foreach (MovementInfoModel info in Info)
				target.Info.Add(new MovementInfoModel(info.Content));
			// Clona las variaciones
			foreach (MovementVariationModel variation in Variations)
				target.Variations.Add(variation.Clone(target));
		}

		/// <summary>
		///		Id del movimiento
		/// </summary>
		public Guid Id { get; private set; } = Guid.NewGuid();

		/// <summary>
		///		Datos del turno
		/// </summary>
		public MovementTurnModel Turn { get; protected set; }

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
		public List<MovementInfoModel> Info { get; } = new List<MovementInfoModel>();

		/// <summary>
		///		Variaciones a este movimiento parcial
		/// </summary>
		public List<MovementVariationModel> Variations { get; } = new List<MovementVariationModel>();
	}
}
