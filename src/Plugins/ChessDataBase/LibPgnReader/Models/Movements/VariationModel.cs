using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibPgnReader.Models.Movements
{
	/// <summary>
	///		Clase con los datos de una variación
	/// </summary>
	public class VariationModel
	{
		public VariationModel(BaseMovementModel parent)
		{
			Parent = parent;
		}

		/// <summary>
		///		Movimiento padre de la variación
		/// </summary>
		public BaseMovementModel Parent { get; }

		/// <summary>
		///		Información
		/// </summary>
		public List<InfoMovementModel> Info { get; } = new List<InfoMovementModel>();

		/// <summary>
		///		Comentarios
		/// </summary>
		public List<string> Comments { get; } = new List<string>();

		/// <summary>
		///		Movimientos
		/// </summary>
		public List<BaseMovementModel> Movements { get; } = new List<BaseMovementModel>();
	}
}
