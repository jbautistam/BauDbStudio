namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models;

/// <summary>
///		Modelo de un menú
/// </summary>
public class MenuModel
{
	/// <summary>
	///		Clave del menú
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Cabecera del menú
	/// </summary>
	public string Header { get; set; } = string.Empty;

	/// <summary>
	///		Tecla
	/// </summary>
	public string? InputGestureText { get; set; }

	/// <summary>
	///		Icono (nombre del recurso)
	/// </summary>
	public string Icon { get; set; } = string.Empty;

	/// <summary>
	///		Ancho del icono
	/// </summary>
	public int IconWidth { get; set; } = 16;

	/// <summary>
	///		Alto del icono
	/// </summary>
	public int IconHeight { get; set; } = 16;

	/// <summary>
	///		Indica si se puede pulsar
	/// </summary>
	public bool IsCheckable { get; set; }

	/// <summary>
	///		Indica si está seleccionado
	/// </summary>
	public bool Checked { get; set; }

	/// <summary>
	///		Comando
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand? Command { get; set; }

	/// <summary>
	///		Objeto asociado
	/// </summary>
	public object? Tag { get; set; }
}
