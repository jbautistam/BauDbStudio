namespace Bau.Libraries.PluginsStudio.Views.Base.Models;

/// <summary>
///		Modelo con los datos de un panel
/// </summary>
public class PaneModel
{
	/// <summary>
	///		Posición
	/// </summary>
	public enum PositionType
	{
		/// <summary>Izquierda</summary>
		Left,
		/// <summary>Superior</summary>
		Top,
		/// <summary>Derecha</summary>
		Right,
		/// <summary>Inferior</summary>
		Bottom
	}

	/// <summary>
	///		Id del panel
	/// </summary>
	public string Id { get; set; } = string.Empty;

	/// <summary>
	///		Título asociado al panel
	/// </summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>
	///		Posición
	/// </summary>
	public PositionType Position { get; set; }

	/// <summary>
	///		Vista del panel
	/// </summary>
	public System.Windows.Controls.UserControl? View { get; set; }

	/// <summary>
	///		ViewModel del panel
	/// </summary>
	public BauMvvm.ViewModels.BaseObservableObject? ViewModel { get; set; }
}
