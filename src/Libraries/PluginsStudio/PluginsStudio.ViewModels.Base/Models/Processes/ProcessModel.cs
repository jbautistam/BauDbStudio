namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

/// <summary>
///		Modelo para ejecución de un proceso
/// </summary>
public abstract class ProcessModel
{
    // Eventos
    public event EventHandler<LogEventArgs>? Log;
    public event EventHandler<ProgressEventArgs>? Progress;

    protected ProcessModel(string group, string name)
    {
        Group = group;
        Name = name;
    }

    /// <summary>
    ///		Ejecuta el proceso
    /// </summary>
    public abstract Task ExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Lanza un evento de log
    /// </summary>
    protected void RaiseLog(LogEventArgs.Status status, string message, Dictionary<string, string>? additionalInfo = null)
    {
        Log?.Invoke(this, new LogEventArgs(status, message, additionalInfo));
    }

    /// <summary>
    ///     Lanza un evento de progreso
    /// </summary>
    protected void RaiseProgress(long actual, long total, string? message = null)
    {
        Progress?.Invoke(this, new ProgressEventArgs(actual, total, message));
    }

    /// <summary>
    ///		Identificador del proceso
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///		Grupo al que pertenece el proceso
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    ///		Nombre del proceso
    /// </summary>
    public string Name { get; set; }
}
