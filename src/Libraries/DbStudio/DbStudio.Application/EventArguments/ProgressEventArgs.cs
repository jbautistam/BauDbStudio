namespace Bau.Libraries.DbStudio.Application.EventArguments;

/// <summary>
///		Argumentos del evento de progreso
/// </summary>
public class ProgressEventArgs : EventArgs
{
	public ProgressEventArgs(long actual, long total)
	{
		Actual = actual;
		Total = total;
	}

	/// <summary>
	///		Progreso actual
	/// </summary>
	public long Actual { get; }

	/// <summary>
	///		Total de elementos
	/// </summary>
	public long Total { get; }
}
