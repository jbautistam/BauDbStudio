namespace Bau.Libraries.ComicsReader.ViewModel.Reader.EventArguments;

/// <summary>
///		Argumentos del evento de zoom
/// </summary>
public class ZoomEventArgs : EventArgs
{
	public ZoomEventArgs(double zoom)
	{
		Zoom = zoom;
	}

	/// <summary>
	///		Zoom
	/// </summary>
	public double Zoom { get; }
}
