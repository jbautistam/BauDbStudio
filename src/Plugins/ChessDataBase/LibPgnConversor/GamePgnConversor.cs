using System;
using System.Collections.Generic;
using Bau.Libraries.ChessDataBase.Models.Board;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Board.Setup;
using Bau.Libraries.ChessDataBase.Models.Games;
using Bau.Libraries.LibPgnReader.Models;
using Bau.Libraries.LibPgnReader.Models.Movements;

namespace Bau.Libraries.LibPgn.Conversor
{
	/// <summary>
	///		Conversor de <see cref="LibraryPgnModel"/> a <see cref="GameModelCollection"/>
	/// </summary>
	public class GamePgnConversor
	{
		/// <summary>
		///		Convierte la librería de jugadas
		/// </summary>
		public LibraryModel Convert(LibraryPgnModel libraryPgn)
		{
			LibraryModel library = new LibraryModel();

				// Convierte los juegos
				foreach (GamePgnModel gamePgn in libraryPgn.Games)
					library.Games.Add(ConvertGame(gamePgn));
				// Devuelve la librería convertida
				return library;
		}

		/// <summary>
		///		Convierte los datos de una partida
		/// </summary>
		private GameModel ConvertGame(GamePgnModel gamePgn)
		{
			GameModel game = new GameModel();

				// Convierte las etiquetas
				ConvertTags(game, gamePgn.Headers);
				// Añade los comentarios
				foreach (string comment in gamePgn.Comments)
					game.Comments.Add(comment);
				//// Depuración
				//#if DEBUG
				//	System.Diagnostics.Debug.WriteLine("");
				//	System.Diagnostics.Debug.WriteLine(new string('=', 80));
				//	System.Diagnostics.Debug.WriteLine("Game");
				//	foreach (HeaderPgnModel header in gamePgn.Headers)
				//		System.Diagnostics.Debug.WriteLine($"[{header.Tag}: {header.Content}]");
				//	System.Diagnostics.Debug.WriteLine("");
				//#endif
				// Convierte los movimientos
				try
				{
					game.Variation.Movements.AddRange(ConvertMovements(game.Board, gamePgn.Movements));
				}
				catch (Exception exception)
				{
					game.ParseError = $"Error when convert movements: {exception.Message}";
					//#if DEBUG
					//	System.Diagnostics.Debug.WriteLine(new string('#', 80));
					//	System.Diagnostics.Debug.WriteLine($"Error when convert movements: {exception.Message}");
					//	System.Diagnostics.Debug.WriteLine(new string('#', 80));
					//#endif
				}
				// Devuelve la partida
				return game;
		}

		/// <summary>
		///		Convierte las etiquetas de una partida
		/// </summary>
		private void ConvertTags(GameModel game, List<HeaderPgnModel> headersPgn)
		{
			foreach (HeaderPgnModel headerPgn in headersPgn)
			{
				if (Compare(headerPgn.Tag, "Event"))
					game.Event = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "Site"))
					game.Site = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "Date"))
					game.Date = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "Round"))
					game.Round = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "White"))
					game.WhitePlayer = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "Black"))
					game.BlackPlayer = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "EventDate"))
					game.EventDate = headerPgn.Content;
				else if (Compare(headerPgn.Tag, "Result"))
					game.Result = ConvertResult(headerPgn.Content);
				else if (Compare(headerPgn.Tag, "FEN"))
				{
					game.Fen = headerPgn.Content;
					game.Board = new Parsers.FenParser().Parse(headerPgn.Content);
				}
				else 
					game.Tags.Add(new KeyValuePair<string, string>(headerPgn.Tag, headerPgn.Content));
			}
		}

		/// <summary>
		///		Convierte los movimientos
		/// </summary>
		private List<MovementBaseModel> ConvertMovements(BoardSetup board, List<BaseMovementModel> movements)
		{
			SquareModel square = new SquareModel();

				// Inicializa el tablero
				if (board == null)
					square.Reset();
				else
					square.Reset(board);
				// Genera los movimientos
				return new Parsers.MovementConversor(square).Convert(movements);
		}

		/// <summary>
		///		Convierte el resultado
		/// </summary>
		private GameModel.ResultType ConvertResult(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				return GameModel.ResultType.Unknown;
			else
				switch (content.Trim())
				{
					case "1-0":
						return GameModel.ResultType.WhiteWins;
					case "0-1":
						return GameModel.ResultType.BlackWins;
					case "1/2-1/2":
						return GameModel.ResultType.Draw;
					case "*":
						return GameModel.ResultType.Open;
					default:
						return GameModel.ResultType.Unknown;
				}
		}

		/// <summary>
		///		Comprara dos cadenas
		/// </summary>
		private bool Compare(string content, string value)
		{
			if (string.IsNullOrEmpty(content))
				return false;
			else
				return content.Trim().Equals(value, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}
