namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

/// <summary>
///		Controlador para la configuración
/// </summary>
public interface IConfigurationController
{
	/// <summary>
	///		Obtiene un valor de configuración
	/// </summary>
	string GetConfiguration(string plugin, string key);

	/// <summary>
	///		Cambia un valor de configuración de un plugin
	/// </summary>
	void SetConfiguration(string plugin, string key, string value);

	/// <summary>
	///		Graba la configuración
	/// </summary>
	void Save();

	/// <summary>
	///		Nombre de la fuente del editor
	/// </summary>
	string EditorFontName { get; set; }

	/// <summary>
	///		Tamaños de la fuente del editor
	/// </summary>
	double EditorFontSize { get; set; }

	/// <summary>
	///		Indica si se debe mostrar el número de línea en el editor
	/// </summary>
	bool EditorShowLinesNumber { get; set; }

	/// <summary>
	///		Indica el nivel de zoom del editor
	/// </summary>
	double EditorZoom { get; set; }

	/// <summary>
	///		Indica si se deben borrar archivos utilizando la papelera de reciclaje
	/// </summary>
	bool RemoveFilesToRecycleBin { get; }

	/// <summary>
	///		Indice de la codificación seleccionada al grabar un archivo 
	/// </summary>
	int LastEncodingIndex { get; set; }
}
