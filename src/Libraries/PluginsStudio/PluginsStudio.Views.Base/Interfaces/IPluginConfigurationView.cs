namespace Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

/// <summary>
///		Interface para el control de configuración de un plugin
/// </summary>
public interface IPluginConfigurationView
{
	/// <summary>
	///		Obtiene el control de usuario de configuración
	/// </summary>
	System.Windows.Controls.UserControl GetUserControl();

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	bool ValidateData(out string error);

	/// <summary>
	///		Graba los datos introducidos
	/// </summary>
	void Save();

	/// <summary>
	///		Cabecera
	/// </summary>
	string Header { get; }
}
