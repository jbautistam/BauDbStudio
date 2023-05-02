namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

/// <summary>
///		Extensiones de archivo asignadas a un plugin
/// </summary>
public class FileAssignedModel
{
	/// <summary>
	///		Id principal
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Nombre del archivo
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	///		Extensiones de archivo a las que se aplica la opción
	/// </summary>
	public string FileExtension { get; set; }

	/// <summary>
	///		Icono
	/// </summary>
	public string Icon { get; set; }

	/// <summary>
	///		Plantilla del archivo
	/// </summary>
	public string Template { get; set; }

	/// <summary>
	///		Indica si se puede crear un archivo de este tipo o sólo es de lectura
	/// </summary>
	public bool CanCreate { get; set; } = true;
}