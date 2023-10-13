namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Pgn;

/// <summary>
///		ViewModel para los elementos de <see cref="PgnGameInfoTagListViewModel"/>
/// </summary>
public class PgnGameInfoTagItemListViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _header = default!, _text = default!;

	public PgnGameInfoTagItemListViewModel(string header, string text)
	{
		Header = header;
		Text = text;
	}

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Texto
	/// </summary>
	public string Text
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}
}
