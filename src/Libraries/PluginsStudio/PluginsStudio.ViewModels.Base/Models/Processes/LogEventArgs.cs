namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

/// <summary>
///		Argumentos del evento de log
/// </summary>
public class LogEventArgs : EventArgs
{
	/// <summary>
	///		Estado del log
	/// </summary>
	public enum Status
	{
		/// <summary>Informativo</summary>
		Info,
		/// <summary>Advertencia</summary>
		Warning,
		/// <summary>Error</summary>
		Error,
		/// <summary>Finalizado correctamente</summary>
		Success
	}

	public LogEventArgs(Status state, string message, Dictionary<string, string>? additionalInfo = null)
	{	
		State = state;
		Message = message;
		AdditionalInfo = additionalInfo;
	}

	/// <summary>
	///		Estado
	/// </summary>
	public Status State { get; }

	/// <summary>
	///		Mensaje
	/// </summary>
	public string Message { get; }

	/// <summary>
	///		Información adicional
	/// </summary>
	public Dictionary<string, string>? AdditionalInfo { get; }
}
