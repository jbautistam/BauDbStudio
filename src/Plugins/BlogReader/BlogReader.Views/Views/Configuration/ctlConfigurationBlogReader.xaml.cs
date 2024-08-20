using System.Windows.Controls;

using Bau.Libraries.LibBlogReader.ViewModel.Configuration;

namespace Bau.Libraries.BlogReader.Views.Views.Configuration;

/// <summary>
///		Control de configuración de BlogReader
/// </summary>
public partial class ctlConfigurationBlogReader : UserControl, PluginsStudio.Views.Base.Interfaces.IPluginConfigurationView
{
	public ctlConfigurationBlogReader(ConfigurationViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa el ViewModel
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		Obtiene el control de usuario
	/// </summary>
	public UserControl GetUserControl() => this;

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	public bool ValidateData(out string error) => ViewModel.ValidateData(out error);

	/// <summary>
	///		Graba los datos de configuración
	/// </summary>
	public void Save()
	{
		ViewModel.Save();
	}

	/// <summary>
	///		ViewModel con los datos de configuración
	/// </summary>
	public ConfigurationViewModel ViewModel { get; }

	/// <summary>
	///		Cabecera del control
	/// </summary>
	public string Header { get; } = "BlogReader";
}
