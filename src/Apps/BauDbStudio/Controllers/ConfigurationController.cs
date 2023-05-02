using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.DbStudio.Controllers;

/// <summary>
///		Controlador para la configuración de la aplicación
/// </summary>
public class ConfigurationController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IConfigurationController
{
	// Constantes privadas
	private const string TagRoot = "Configuration";
	private const string TagPlugin = "Plugin";
	private const string TagName = "Name";
	private const string TagKey = "Key";
	private const string TagValue = "Value";
	// Registros privados
	private record PluginConfiguration(string Plugin, string Key, string Value);

	public ConfigurationController(DbStudioViewsManager viewsManager)
	{
		ViewsManager = viewsManager;
	}

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
		LastWorkSpace = Properties.Settings.Default.LastWorkSpace;
		LastFiles = Properties.Settings.Default.LastFiles;
		ShowWindowNotifications = Properties.Settings.Default.ShowWindowNotifications;
		LastEncodingIndex = Properties.Settings.Default.LastEncodingIndex;
		PathData = Properties.Settings.Default.PathData;
		if (string.IsNullOrWhiteSpace(PathData) || !System.IO.Directory.Exists(PathData))
			PathData = pathData;
		// Carga la configuración de plugins
		LoadPluginsConfiguration(GetPluginsSetupFileName(pathData));
	}

	/// <summary>
	///		Carga la configuración de los plugins
	/// </summary>
	private void LoadPluginsConfiguration(string fileName)
	{
		// Limpia la configuración
		PluginsConfiguration.Clear();
		// Carga el texto del archivo
		if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
			try
			{
				MLFile fileML = new Libraries.LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

					if (fileML is not null)
					{
						foreach (MLNode rootML in fileML.Nodes)
							if (rootML.Name == TagRoot)
								foreach (MLNode nodeML in rootML.Nodes)
									if (nodeML.Name == TagPlugin)
										PluginsConfiguration.Add(GetConfiguration(nodeML));
					}
					else
						ViewsManager.MainWindowsController.Logger.LogError($"Can't open the configuration file {fileName}");
			}
			catch (Exception exception)
			{
				ViewsManager.MainWindowsController.Logger.LogError(exception, $"Can't open the configuration file {fileName}");
			}
			// Carga la configuración desde una cadena si no puede leer el XML
			if (PluginsConfiguration.Count == 0)
				try
				{
					LoadConfigurationFromString(fileName);
				}
				catch (Exception exception)
				{
					ViewsManager.MainWindowsController.Logger.LogError(exception, $"Can't load configuration from text of the configuration file {fileName}");
				}
	}

	/// <summary>
	///		Carga la configuración de una cadena
	/// </summary>
	private void LoadConfigurationFromString(string fileName)
	{
		string setup = Libraries.LibHelper.Files.HelperFiles.LoadTextFile(fileName);

			// Asigna la configuración
			if (!string.IsNullOrWhiteSpace(setup))
			{
				string [] parts = setup.Split("({#@~=#})");

					for (int index = 0; index < parts.Length; index += 3)
						if (parts.Length > index + 3 && !string.IsNullOrWhiteSpace(parts[index]) && !string.IsNullOrWhiteSpace(parts[index + 1]))
							PluginsConfiguration.Add(new PluginConfiguration(parts[index], parts[index + 1], parts[index + 2]));
			}
	}

	/// <summary>
	///		Obtiene la configuración
	/// </summary>
	private PluginConfiguration GetConfiguration(MLNode nodeML)
	{
		string plugin = nodeML.Attributes[TagName].Value.TrimIgnoreNull();
		string key = nodeML.Attributes[TagKey].Value.TrimIgnoreNull();
		string value = nodeML.Attributes[TagValue].Value.TrimIgnoreNull();

			// Si no hay un valor en el atributo, recoge el valor del nodo
			if (string.IsNullOrWhiteSpace(value))
				value = nodeML.Value.TrimIgnoreNull();
			// Devuelve los valores
			return new PluginConfiguration(plugin, key, value);
	}

	/// <summary>
	///		Obtiene un valor de configuración de un plugin
	/// </summary>
	public string GetConfiguration(string plugin, string key)
	{
		// Busca la configuración del plugin
		if (!string.IsNullOrWhiteSpace(plugin) && !string.IsNullOrWhiteSpace(key))
			foreach (PluginConfiguration configuration in PluginsConfiguration)
				if (configuration.Plugin.Equals(plugin, StringComparison.CurrentCultureIgnoreCase) &&
						configuration.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
					return configuration.Value;
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
				if (PluginsConfiguration[index].Plugin.Equals(plugin, StringComparison.CurrentCultureIgnoreCase) &&
						PluginsConfiguration[index].Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
					PluginsConfiguration.RemoveAt(index);
			// Añade la configuración
			PluginsConfiguration.Add(new PluginConfiguration(plugin, key, value));
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
		Properties.Settings.Default.LastWorkSpace = LastWorkSpace;
		Properties.Settings.Default.LastFiles = LastFiles;
		Properties.Settings.Default.ShowWindowNotifications = ShowWindowNotifications;
		Properties.Settings.Default.LastEncodingIndex = LastEncodingIndex;
		Properties.Settings.Default.PathData = PathData;
		// Graba la configuración
		Properties.Settings.Default.Save();
		// Graba la configuración de los plugins
		SavePluginsConfiguration(GetPluginsSetupFileName(PathData));
	}

	/// <summary>
	///		Grab la configuración de los plugins
	/// </summary>
	private void SavePluginsConfiguration(string fileName)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Crea los nodos
			foreach (PluginConfiguration configuration in PluginsConfiguration)
			{
				MLNode nodeML = new(TagPlugin);

					// Añade los atributos
					nodeML.Attributes.Add(TagName, configuration.Plugin);
					nodeML.Attributes.Add(TagKey, configuration.Key);
					nodeML.Attributes.Add(TagValue, configuration.Value);
					// Añade el nodo a la colección
					rootML.Nodes.Add(nodeML);
			}
			// Graba el archivo
			if (!new Libraries.LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML))
				ViewsManager.MainWindowsController.Logger.LogError($"Error saving the configuration file {fileName}");
	}

	/// <summary>
	///		Obtiene el nombre del archivo de setup
	/// </summary>
	private string GetPluginsSetupFileName(string pathData)
	{
		return System.IO.Path.Combine(pathData, "PluginsSetup.xml");
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	public DbStudioViewsManager ViewsManager { get; }

	/// <summary>
	///		Ultimo directorio seleccionado para grabación
	/// </summary>
	public string LastPathSelected { get; set; } = default!;

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
	///		Ultimo workspace seleccionado
	/// </summary>
	public string LastWorkSpace { get; set; } = default!;

	/// <summary>
	///		Ultimos archivos abiertos
	/// </summary>
	public string LastFiles { get; set; } = default!;

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
	public string PathData { get; set; } = default!;

	/// <summary>
	///		Configuraciones de los plugins
	/// </summary>
	private List<PluginConfiguration> PluginsConfiguration { get; } = new();
}
