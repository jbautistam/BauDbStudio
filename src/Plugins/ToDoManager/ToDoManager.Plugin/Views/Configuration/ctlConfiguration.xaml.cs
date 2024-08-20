using System.Windows.Controls;

using Bau.Libraries.ToDoManager.ViewModel.Configuration;

namespace Bau.Libraries.ToDoManager.Plugin.Views.Configuration;

/// <summary>
///		Control de configuración de Todo
/// </summary>
public partial class ctlConfiguration : UserControl, PluginsStudio.Views.Base.Interfaces.IPluginConfigurationView
{
	public ctlConfiguration(ConfigurationViewModel viewModel)
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
	public bool ValidateData(out string error)
	{
		error = string.Empty;
		return true;
	}

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
	public string Header => "ToDo plugin";
}
