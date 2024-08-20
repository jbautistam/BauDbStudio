namespace Bau.Libraries.EbooksReader.ViewModel.Reader.Comics.EventArguments;

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
