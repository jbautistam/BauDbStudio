using System;

namespace Bau.Libraries.BauSparkScripts.ViewModels.Configuration
{
	/// <summary>
	///		Datos de la configuración
	/// </summary>
	public class ConfigurationModel
	{
		public ConfigurationModel(string configurationPath)
		{
			ConfigurationPath = configurationPath;
		}

		/// <summary>
		///		Directorio donde se graba la configuración
		/// </summary>
		public string ConfigurationPath { get; set; }
	}
}
