using System;
using System.Collections.Generic;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador para la configuración de la aplicación
	/// </summary>
	public class ConfigurationController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IConfigurationController
	{
		// Constantes privadas
		private const string SetupSeparator = "({#@~=#})";

		/// <summary>
		///		Carga la configuración
		/// </summary>
		public void Load(string pathData)
		{
			// Asigna las configuraciones de la aplicación
			LastPathSelected = Properties.Settings.Default.LastPathSelected;
			LastThemeSelected = Properties.Settings.Default.LastThemeSelected;
			EditorFontName = Properties.Settings.Default.EditorFontName;
			EditorFontSize = Properties.Settings.Default.EditorFontSize;
			EditorShowLinesNumber = Properties.Settings.Default.EditorShowLinesNumber;
			EditorZoom = Properties.Settings.Default.EditorZoom;
			ConsoleExecutable = Properties.Settings.Default.ConsoleExecutable;
			LastWorkSpace = Properties.Settings.Default.LastWorkSpace;
			LastFiles = Properties.Settings.Default.LastFiles;
			ShowWindowNotifications = Properties.Settings.Default.ShowWindowNotifications;
			LastEncodingIndex = Properties.Settings.Default.LastEncodingIndex;
			PathData = Properties.Settings.Default.PathData;
			if (string.IsNullOrWhiteSpace(PathData) || !System.IO.Directory.Exists(PathData))
				PathData = pathData;
			// Carga la configuración de plugins
			LoadPluginsCongiguration(Properties.Settings.Default.PluginsSetup);
		}

		/// <summary>
		///		Carga la configuración de los plugins
		/// </summary>
		private void LoadPluginsCongiguration(string setup)
		{
			// Limpia la configuración
			PluginsConfiguration.Clear();
			// Asigna la configuración
			if (!string.IsNullOrWhiteSpace(setup))
			{
				string [] parts = setup.Split(SetupSeparator);

					for (int index = 0; index < parts.Length; index += 3)
						if (parts.Length > index + 3 && !string.IsNullOrWhiteSpace(parts[index]) && !string.IsNullOrWhiteSpace(parts[index + 1]))
							PluginsConfiguration.Add((parts[index], parts[index + 1], parts[index + 2]));
			}
		}

		/// <summary>
		///		Obtiene un valor de configuración de un plugin
		/// </summary>
		public string GetConfiguration(string plugin, string key)
		{
			// Busca la configuración del plugin
			if (!string.IsNullOrWhiteSpace(plugin) && !string.IsNullOrWhiteSpace(key))
				foreach ((string plugin, string key, string value) configuration in PluginsConfiguration)
					if (configuration.plugin.Equals(plugin, StringComparison.CurrentCultureIgnoreCase) &&
							configuration.key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
						return configuration.value;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return string.Empty;
		}

		/// <summary>
		///		Cambia un valor de configuración de un plugin
		/// </summary>
		public void SetConfiguration(string plugin, string key, string value)
		{
			if (!string.IsNullOrWhiteSpace(plugin) && !string.IsNullOrWhiteSpace(key))
			{
				// Elimina la configuración del plugin para esa clave
				for (int index = PluginsConfiguration.Count - 1; index >= 0; index--)
					if (PluginsConfiguration[index].plugin.Equals(plugin, StringComparison.CurrentCultureIgnoreCase) &&
							PluginsConfiguration[index].key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
						PluginsConfiguration.RemoveAt(index);
				// Añade la configuración
				PluginsConfiguration.Add((plugin, key, value));
			}
		}

		/// <summary>
		///		Graba la configuración
		/// </summary>
		public void Save()
		{
			// Asigna las propiedades
			Properties.Settings.Default.LastPathSelected = LastPathSelected;
			Properties.Settings.Default.LastThemeSelected = LastThemeSelected;
			Properties.Settings.Default.EditorFontName = EditorFontName;
			Properties.Settings.Default.EditorFontSize = EditorFontSize;
			Properties.Settings.Default.EditorShowLinesNumber = EditorShowLinesNumber;
			Properties.Settings.Default.EditorZoom = EditorZoom;
			Properties.Settings.Default.ConsoleExecutable = ConsoleExecutable;
			Properties.Settings.Default.LastWorkSpace = LastWorkSpace;
			Properties.Settings.Default.LastFiles = LastFiles;
			Properties.Settings.Default.ShowWindowNotifications = ShowWindowNotifications;
			Properties.Settings.Default.LastEncodingIndex = LastEncodingIndex;
			Properties.Settings.Default.PathData = PathData;
			Properties.Settings.Default.PluginsSetup = GetPluginsConfiguration();
			// Graba la configuración
			Properties.Settings.Default.Save();
		}

		/// <summary>
		///		Obtiene la configuración de plugins
		/// </summary>
		private string GetPluginsConfiguration()
		{
			string result = string.Empty;

				// Añade las configuraciones a la cadena
				foreach ((string plugin, string key, string value) in PluginsConfiguration)
				{
					// Añade el separador si es necesario
					if (!string.IsNullOrWhiteSpace(result))
						result += SetupSeparator;
					// Añade los datos
					result += plugin + SetupSeparator + key + SetupSeparator + value;
				}
				// Devuelve la caena resultante
				return result;
		}

		/// <summary>
		///		Ultimo directorio seleccionado para grabación
		/// </summary>
		public string LastPathSelected { get; set; }

		/// <summary>
		///		Ultimo tema seleccionado
		/// </summary>
		public int LastThemeSelected { get; set; }

		/// <summary>
		///		Nombre de la fuente del editor
		/// </summary>
		public string EditorFontName { get; set; } = "Consolas";

		/// <summary>
		///		Tamaños de la fuente del editor
		/// </summary>
		public double EditorFontSize { get; set; } = 18;

		/// <summary>
		///		Indica si se debe mostrar el número de línea en el editor
		/// </summary>
		public bool EditorShowLinesNumber { get; set; } = true;

		/// <summary>
		///		Indica el nivel de zoom del editor
		/// </summary>
		public double EditorZoom { get; set; } = 1.0;

		/// <summary>
		///		Nombre del ejecutable de la consola
		/// </summary>
		public string ConsoleExecutable { get; set; }

		/// <summary>
		///		Ultimo workspace seleccionado
		/// </summary>
		public string LastWorkSpace { get; set; }

		/// <summary>
		///		Ultimos archivos abiertos
		/// </summary>
		public string LastFiles { get; set; }

		/// <summary>
		///		Indica si se deben mostrar las notificaciones de sistema
		/// </summary>
		public bool ShowWindowNotifications { get; set; }

		/// <summary>
		///		Indice de la codificación seleccionada al grabar un archivo 
		/// </summary>
		public int LastEncodingIndex { get; set; }

		/// <summary>
		///		Directorio de datos
		/// </summary>
		public string PathData { get; set; }

		/// <summary>
		///		Configuraciones de los plugins
		/// </summary>
		private List<(string plugin, string key, string value)> PluginsConfiguration { get; } = new();
	}
}
