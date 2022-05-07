using System;

namespace Bau.Libraries.LibPgnReader.Models.Movements
{
	/// <summary>
	///		Modelo con la información asociada a un movimiento
	/// </summary>
	public class InfoMovementModel
	{
		public InfoMovementModel(string content)
		{
			Content = content;
		}

		/// <summary>
		///		Contenido
		/// </summary>
		public string Content { get; }
	}
}
