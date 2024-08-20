using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibPgnReader.Models
{
	/// <summary>
	///		Modelo con los datos de una partida
	/// </summary>
	public class GamePgnModel
	{
		/// <summary>
		///		Cabeceras
		/// </summary>
		public List<HeaderPgnModel> Headers { get; } = new List<HeaderPgnModel>();

		/// <summary>
		///		Comentarios del juego
		/// </summary>
		public List<string> Comments { get; } = new List<string>();

		/// <summary>
		///		Movimientos
		/// </summary>
		public List<Movements.BaseMovementModel> Movements { get; } = new List<Movements.BaseMovementModel>();
	}
}
