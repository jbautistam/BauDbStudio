using System.Collections.ObjectModel;

using Bau.Libraries.ChessDataBase.Models.Games;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Pgn;

/// <summary>
///		ViewModel para una lista con información adicional sobre el juego
/// </summary>
public class PgnGameInfoTagListViewModel
{
	public PgnGameInfoTagListViewModel(PgnGameInfoViewModel game)
	{
		Game = game;
		LoadInfo(game.Game);
	}

	/// <summary>
	///		Carga la información del juego
	/// </summary>
	private void LoadInfo(GameModel game)
	{
		// Limpia la información
		Items.Clear();
		// Añade la información adicional
		Add("WhitePlayer", game.WhitePlayer);
		Add("BlackPlayer", game.BlackPlayer);
		Add("Fen", game.Fen);
		Add("Result", game.Result.ToString());
		Add("Date", game.Date);
		Add("EventDate", game.EventDate);
		Add("Event", game.Event);
		Add("Round", game.Round);
		Add("Site", game.Site);
		// Añade las etiquetas
		foreach (KeyValuePair<string, string> tag in game.Tags)
			Add(tag.Key, tag.Value);
		// Añade los comentarios
		foreach (string comment in game.Comments)
			Add("Comment", comment);
		// Añade los errores de interpretación
		Add("Error", game.ParseError);
	}

	/// <summary>
	///		Añade un elemento
	/// </summary>
	private void Add(string header, string text)
	{
		if (!string.IsNullOrWhiteSpace(text) && !Exists(header, text))
			Items.Add(new PgnGameInfoTagItemListViewModel(header, text));
	}

	/// <summary>
	///		Comprueba si existe un elemento en la lista
	/// </summary>
	private bool Exists(string header, string text)
	{
		// Busca el elemento en la lista
		if (!string.IsNullOrWhiteSpace(header))
			foreach (PgnGameInfoTagItemListViewModel item in Items)
				if (header.Equals(item.Header, StringComparison.CurrentCultureIgnoreCase) &&
						text.Equals(item.Text, StringComparison.CurrentCultureIgnoreCase))
					return true;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return false;
	}

	/// <summary>
	///		ViewModel del juego
	/// </summary>
	public PgnGameInfoViewModel Game { get; }

	/// <summary>
	///		Elementos
	/// </summary>
	public ObservableCollection<PgnGameInfoTagItemListViewModel> Items { get; } = new();
}
