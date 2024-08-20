using Microsoft.Extensions.Logging;

namespace Bau.DbStudio.Controllers.Logging;

/// <summary>
///     Configuración del logger
/// </summary>
public sealed class ApplicationEventLoggerConfiguration
{
    /// <summary>
    ///     Añade una serie de elementos de log
    /// </summary>
	public void AddLogLevels(params LogLevel[] levels)
	{
		LogLevels.Clear();
        LogLevels.AddRange(levels);
	}

    /// <summary>
    ///     Id del evento a tratar
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    ///     Acción que se ejecuta cuando se escriba sobre el log
    /// </summary>
    public Action<LogLevel, string, Exception?>? WriteLog;

    /// <summary>
    ///     Niveles de log
    /// </summary>
    public List<LogLevel> LogLevels { get; } = new()
                                                   {
                                                        LogLevel.Debug,
                                                        LogLevel.Information,
                                                        LogLevel.Trace,
                                                        LogLevel.Warning,
                                                        LogLevel.Error,
                                                        LogLevel.Critical
                                                   };
}
