using Microsoft.Extensions.Logging;

namespace Bau.DbStudio.Controllers.Logging;

/// <summary>
///		Logger interno de la aplicación
/// </summary>
public sealed class ApplicationEventLogger : ILogger
{
	// Variables privadas
	private readonly string _name;
	private readonly Func<ApplicationEventLoggerConfiguration> _getCurrentConfig;

	public ApplicationEventLogger(string name, Func<ApplicationEventLoggerConfiguration> getCurrentConfig)
	{
		_name = name;
		_getCurrentConfig = getCurrentConfig;
	}

	/// <summary>
	///		Arranca un ámbito
	/// </summary>
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return default!;
	}

	/// <summary>
	///		Indica si el logger está activo para este nivel
	/// </summary>
	public bool IsEnabled(LogLevel logLevel)
	{
		return _getCurrentConfig().LogLevels.Contains(logLevel);
	}

	/// <summary>
	///		Trata un log
	/// </summary>
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (IsEnabled(logLevel))
		{
			ApplicationEventLoggerConfiguration config = _getCurrentConfig();

				if (config.EventId == 0 || config.EventId == eventId.Id)
				{
					// Escribe el log
					if (config.WriteLog is not null)
						config.WriteLog(logLevel, formatter(state, exception), exception);
					// Escribe el log además en la salida
					#if DEBUG
						System.Diagnostics.Debug.WriteLine($"[{logLevel.ToString()}] - {formatter(state, exception)}");
					#endif
				}
		}
	}
}