namespace Bau.Libraries.PluginsStudio.Views.Base.Models;

/// <summary>
///		Modelo de una barra de herramientas
/// </summary>
public class ToolBarModel
{
	/// <summary>
	///		Clave de la barra de herramientas
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Barra de herramientas
	/// </summary>
	public System.Windows.Controls.ToolBar? ToolBar { get; set; }
}
