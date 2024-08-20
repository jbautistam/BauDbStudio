using Bau.Libraries.ChessDataBase.Models.Games;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Pgn;

/// <summary>
///		ViewModel con los datos de un juego
/// </summary>
public class PgnGameInfoViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _event = default!, _round = default!, _site = default!, _whitePlayer = default!, _blackPlayer = default!, _title = default!;
	private string _date = default!;
	private PgnGameInfoTagListViewModel _informationList = default!;

	public PgnGameInfoViewModel(PgnLibraryViewModel chessGameViewModel, GameModel game)
	{
		// Asigna el juego
		TopViewModel = chessGameViewModel;
		Game = game;
		// Carga los datos
		if (string.IsNullOrEmpty(game.Event))
			Event = "Sin evento definido";
		else
			Event = game.Event;
		Round = game.Round;
		Site = game.Site;
		WhitePlayer = game.WhitePlayer;
		BlackPlayer = game.BlackPlayer;
		Date = game.Date;
		Title = $"{Event}/{Round}: {WhitePlayer} - {BlackPlayer}";
		// Carga la información adicional
		InformationList = new PgnGameInfoTagListViewModel(this);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PgnLibraryViewModel TopViewModel { get; }

	/// <summary>
	///		Juego
	/// </summary>
	public GameModel Game { get; }

	/// <summary>
	///		Evento
	/// </summary>
	public string Event
	{
		get { return _event; }
		set { CheckProperty(ref _event, value); }
	}

	/// <summary>
	///		Ronda
	/// </summary>
	public string Round
	{
		get { return _round; }
		set { CheckProperty(ref _round, value); }
	}

	/// <summary>
	///		Sitio
	/// </summary>
	public string Site
	{
		get { return _site; }
		set { CheckProperty(ref _site, value); }
	}

	/// <summary>
	///		Jugador de blancas
	/// </summary>
	public string WhitePlayer
	{
		get { return _whitePlayer; }
		set { CheckProperty(ref _whitePlayer, value); }
	}

	/// <summary>
	///		Jugador de negras
	/// </summary>
	public string BlackPlayer
	{
		get { return _blackPlayer; }
		set { CheckProperty(ref _blackPlayer, value); }
	}

	/// <summary>
	///		Fecha
	/// </summary>
	public string Date
	{
		get { return _date; }
		set { CheckProperty(ref _date, value); }
	}

	/// <summary>
	///		Título
	/// </summary>
	public string Title
	{
		get { return _title; }
		set { CheckProperty(ref _title, value); }
	}

	/// <summary>
	///		ViewModel de información adicional
	/// </summary>
	public PgnGameInfoTagListViewModel InformationList
	{
		get { return _informationList; }
		set { CheckObject(ref _informationList, value); }
	}
}
