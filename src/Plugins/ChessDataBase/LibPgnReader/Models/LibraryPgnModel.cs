using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibPgnReader.Models
{
	/// <summary>
	///		Modelo de una librería de juegos
	/// </summary>
	public class LibraryPgnModel
	{
		/// <summary>
		///		Juegos que componen la librería
		/// </summary>
		public List<GamePgnModel> Games { get; } = new List<GamePgnModel>();

		/// <summary>
		///		Errores de interpretación
		/// </summary>
		public List<string> Errors { get; } = new List<string>();
	}
}
