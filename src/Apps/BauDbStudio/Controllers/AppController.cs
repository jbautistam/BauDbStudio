using System;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador principal de la aplicación
	/// </summary>
	public class AppController
	{
		public AppController(string applicationName, MainWindow mainWindow, string appPath)
		{
			SparkSolutionController = new SparkSolutionController(applicationName, mainWindow, appPath);
		}

		/// <summary>
		///		Controlador de la solución
		/// </summary>
		public SparkSolutionController SparkSolutionController { get; }

		/// <summary>
		///		Controlador de configuración de la aplicación
		/// </summary>
		public AppConfigurationController ConfigurationController { get; } = new AppConfigurationController();
	}
}
