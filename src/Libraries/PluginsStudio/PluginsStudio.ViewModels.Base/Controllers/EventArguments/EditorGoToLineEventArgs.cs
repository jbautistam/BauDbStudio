namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers.EventArguments;

/// <summary>
///		Argumentos del evento del editor para ir a una línea
/// </summary>
public class EditorGoToLineEventArgs : EventArgs
{
	public EditorGoToLineEventArgs(string textSelected, int line)
	{
		TextSelected = textSelected;
		Line = line;
	}

	/// <summary>
	///		Texto que se debe seleccionar en la línea
	/// </summary>
	public string TextSelected { get; }

	/// <summary>
	///		Línea solicitada
	/// </summary>
	public int Line { get; }
}
