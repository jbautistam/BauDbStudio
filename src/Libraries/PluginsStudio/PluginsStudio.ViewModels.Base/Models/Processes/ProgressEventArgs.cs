namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

/// <summary>
///		Eventos de progreso
/// </summary>
public class ProgressEventArgs : EventArgs
{
	public ProgressEventArgs(long actual, long total, string? message = null)
	{
		Actual = actual;
		Total = total;
		Message = message;
	}

	/// <summary>
	///		Actual
	/// </summary>
	public long Actual { get; }

	/// <summary>
	///		Total
	/// </summary>
	public long Total { get; }

	/// <summary>
	///		Porcentaje de ejecución
	/// </summary>
	public double Percent
	{
		get
		{
			if (Total == 0)
				return 0;
			else
				return Actual * 100 / Total;
		}
	}

	/// <summary>
	///		Mensaje
	/// </summary>
	public string? Message { get; }
}
