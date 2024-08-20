using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Bau.DbStudio.Controllers.Logging;

/// <summary>
///		Extensión para los servicios del Logger
/// </summary>
public static class ApplicationEventLoggerExtensions
{
	/// <summary>
	///		Añade el logger
	/// </summary>
	public static ILoggingBuilder AddColorConsoleLogger(this ILoggingBuilder builder)
	{
		// Añade los datos del logger
		builder.AddConfiguration();
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ApplicationEventLoggerProvider>());
		LoggerProviderOptions.RegisterProviderOptions<ApplicationEventLoggerConfiguration, ApplicationEventLoggerProvider>(builder.Services);
		// Devuelve el generador
		return builder;
	}

	/// <summary>
	///		Añade el logger con una configuración
	/// </summary>
	public static ILoggingBuilder AddColorConsoleLogger(this ILoggingBuilder builder, Action<ApplicationEventLoggerConfiguration> configure)
	{
		// Añade los datos del logger
		builder.AddColorConsoleLogger();
		builder.Services.Configure(configure);
		// Devuelve el generador
		return builder;
	}
}