using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bau.DbStudio.Controllers.Logging;

namespace Bau.DbStudio;

/// <summary>
///		Clase principal de la aplicación
/// </summary>
public partial class App : Application
{
	public App()
	{
		MainHost = Host.CreateDefaultBuilder()
							.ConfigureServices(services =>  
													{
													}
											  )
							.ConfigureLogging(logging => {
															logging.AddConsole();
															logging.AddColorConsoleLogger(configuration =>
																								{
																									// Configura la función que se ejecuta al configurar el log
																									configuration.WriteLog = TreatLog;
																								}
																						  );
															logging.SetMinimumLevel(LogLevel.Debug);
														}
												)
						.Build();

		//ILoggerFactory? loggerFactory = MainHost.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

		//? Wpf tiene un error al formatear las fechas. No recoge la cultura actual si no que siempre lo pone en formato inglés.
		//? Para evitarlo se utiliza la siguiente línea (que tiene que estar antes de empezar a abrir ventanas)
		//? http://www.nbdtech.com/Blog/archive/2009/02/22/wpf-data-binding-cheat-sheet-update-the-internationalization-fix.aspx
		FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
														   new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		// Inicializa los manejadores de eventos para las excepciones no controladas
		// En principio sólo haría falta el primero, el resto son para excepciones de otros hilos
		#if !DEBUG
			DispatcherUnhandledException += (sender, evntArgs) => 
												{ 
													TreatException(evntArgs.Exception);
													evntArgs.Handled = true;
												};
			AppDomain.CurrentDomain.UnhandledException += (sender, evntArgs) => TreatException(evntArgs.ExceptionObject as Exception);
		#endif
	}

	/// <summary>
	///		Rutina de tratamiento de log
	/// </summary>
	private void TreatLog(LogLevel level, string content, Exception? exception)
	{
		DbStudio.MainWindow.DbStudioViewsManager.PluginsStudioViewModel.LogViewModel.WriteLog(level, content, exception);
	}

	/// <summary>
	///		Trata las excepciones no controladas de la aplicación
	/// </summary>
	private void TreatException(Exception exception)
	{
		// Trata la excepción
		try
		{
			System.Diagnostics.Trace.TraceError($"Unhandled exception: {exception?.Message}");
			MessageBox.Show($"Error: {exception?.Message}");
		}
		catch (Exception newException)
		{
			System.Diagnostics.Trace.TraceError($"Unhandled exception: {newException?.Message}");
		}
		// Escribe la excepción en el log
		try
		{
			ILoggerFactory? loggerFactory = MainHost?.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

				if (loggerFactory is not null)
					loggerFactory.CreateLogger<App>().LogError(exception, "Unhandled exception");
		}
		catch (Exception newException)
		{
			System.Diagnostics.Trace.TraceError($"Unhandled exception when log: {newException?.Message}");
		}
	}

	/// <summary>
	///		Host con el inyector de dependencias
	/// </summary>
	internal static IHost? MainHost { get; private set; }
}
