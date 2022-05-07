using System;
using System.Collections.Generic;

namespace Bau.Libraries.ChessDataBase.Models.Games
{
	/// <summary>
	///		Clase con los datos de un juego
	/// </summary>
	public class GameModel
	{
		/// <summary>
		///		Resultado del juego
		/// </summary>
		public enum ResultType
		{
			/// <summary>Resultado desconocido</summary>
			Unknown,
			/// <summary>Ganan las blancas</summary>
			WhiteWins,
			/// <summary>Ganan las negras</summary>
			BlackWins,
			/// <summary>Empate</summary>
			Draw,
			/// <summary>Juego no finalizado</summary>
			Open
		}

		/// <summary>
		///		Evento
		/// </summary>
		public string Event { get; set; }

		/// <summary>
		///		Ronda
		/// </summary>
		public string Round { get; set; }

		/// <summary>
		///		Lugar
		/// </summary>
		public string Site { get; set; }

		/// <summary>
		///		Nombre del jugador de blancas
		/// </summary>
		public string WhitePlayer { get; set; }

		/// <summary>
		///		Nombre del jugador de negras
		/// </summary>
		public string BlackPlayer { get; set; }

		/// <summary>
		///		Configuración del tablero (formato FEN)
		/// </summary>
		public string Fen { get; set; }

		/// <summary>
		///		Resultado
		/// </summary>
		public ResultType Result { get; set; }

		/// <summary>
		///		Fecha de la partida
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		///		Fecha del evento
		/// </summary>
		public string EventDate { get; set; }

		/// <summary>
		///		Comentarios
		/// </summary>
		public List<string> Comments { get; } = new List<string>();

		/// <summary>
		///		Etiquetas
		/// </summary>
		public List<KeyValuePair<string, string>> Tags { get; } = new List<KeyValuePair<string, string>>();

		/// <summary>
		///		Posición inicial de la partida
		/// </summary>
		public Board.Setup.BoardSetup Board { get; set; }

		/// <summary>
		///		Variación / partida
		/// </summary>
		public Board.Movements.MovementVariationModel Variation { get; internal set; } = new Board.Movements.MovementVariationModel(null);

		/// <summary>
		///		Error de interpretación
		/// </summary>
		public string ParseError { get; set; }
	}
}
