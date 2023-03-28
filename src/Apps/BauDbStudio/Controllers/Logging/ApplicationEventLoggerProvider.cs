using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bau.DbStudio.Controllers.Logging;

/// <summary>
///     Proveedor de log
/// </summary>
[UnsupportedOSPlatform("browser")]
[ProviderAlias("ApplicationEventLogger")]
public sealed class ApplicationEventLoggerProvider : ILoggerProvider
{
    // Variables privadas
    private readonly IDisposable? _onChangeToken;
    private readonly ConcurrentDictionary<string, ApplicationEventLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
    private ApplicationEventLoggerConfiguration _currentConfig;

    public ApplicationEventLoggerProvider(IOptionsMonitor<ApplicationEventLoggerConfiguration> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }

    /// <summary>
    ///     Crea el logger sobre una categoría
    /// </summary>
    public ILogger CreateLogger(string categoryName)
    {  
        return _loggers.GetOrAdd(categoryName, name => new ApplicationEventLogger(name, GetCurrentConfig));
    }

    /// <summary>
    ///     Obtiene la configuración
    /// </summary>
    private ApplicationEventLoggerConfiguration GetCurrentConfig() => _currentConfig;

    /// <summary>
    ///     Libera la memoria
    /// </summary>
    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken?.Dispose();
    }
}