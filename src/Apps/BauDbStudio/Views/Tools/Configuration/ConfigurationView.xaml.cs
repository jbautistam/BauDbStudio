using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

namespace Bau.DbStudio.Views.Tools.Configuration;

/// <summary>
///		Ventana de configuración
/// </summary>
public partial class ConfigurationView : Window
{   
	// Variables privadas
	private List<IPluginConfigurationView>? _configurationViews;

	public ConfigurationView(Controllers.DbStudioViewsManager viewsManager)
	{ 
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa las propiedades
		ViewsManager = viewsManager;
		// Inicializa los controles
		InitForm();
	}

	/// <summary>
	///		Inicializa el formulario con la configuración
	/// </summary>
	private void InitForm()
	{
		// Rellena el combo de fuentes
		for (int index = 0; index < cboFontChooser.Items.Count; index++)
			if ((cboFontChooser.Items[index]?.ToString() ?? string.Empty).Equals(MainWindow.DbStudioViewsManager.ConfigurationController.EditorFontName, 
																				 StringComparison.CurrentCultureIgnoreCase))
				cboFontChooser.SelectedItem = cboFontChooser.Items[index];
		// Selecciona la fuente
		if (cboFontChooser.SelectedItem == null && cboFontChooser.Items.Count > 0)
			cboFontChooser.SelectedIndex = 0;
		// Asigna las propiedades
		txtFontSize.Value = MainWindow.DbStudioViewsManager.ConfigurationController.EditorFontSize;
		chkShowLineNumber.IsChecked = MainWindow.DbStudioViewsManager.ConfigurationController.EditorShowLinesNumber;
		chkShowNotifications.IsChecked = MainWindow.DbStudioViewsManager.ConfigurationController.ShowWindowNotifications;
		// Inicializa los controles de configuración de los plugins
		InitPluginsControls();
	}

	/// <summary>
	///		Inicializa los controles
	/// </summary>
	private void InitPluginsControls()
	{
		// Guarda las vistas de configuración
		_configurationViews = ViewsManager.GetConfigurationViews();
		// Muestra las vistas de configuración
		foreach (IPluginConfigurationView control in ViewsManager.GetConfigurationViews())
		{
			UserControl configuration = control.GetUserControl();

				if (configuration != null)
				{
					TabItem tabControlItem = new TabItem();

						// Añade el control a la pestaña
						tabControlItem.Header = control.Header;
						tabControlItem.Content = configuration;
						// Añade la pestaña al control
						tabControls.Items.Add(tabControlItem);
				}
		}
	}

	/// <summary>
	///		Comprueba los datos introducidos en el formulario
	/// </summary>
	private bool ValidateData() => ValidatePluginsData();

	/// <summary>
	///		Comprueba los datos introducidos en los plugins
	/// </summary>
	private bool ValidatePluginsData()
	{
		bool validated = true; // ... supone que los datos son correctos

			// Comprueba los datos
			if (_configurationViews is not null)
				foreach (IPluginConfigurationView configurationView in _configurationViews)
					if (validated && !configurationView.ValidateData(out string error))
					{ 
						// Muestra el error
						ViewsManager.MainWindowsController.HostController.SystemController.ShowMessage(error);
						// Indica que la validación no es correcta
						validated = false;
					}
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos de la configuración
	/// </summary>
	private void Save()
	{
		if (ValidateData())
		{
			// Cambia los valores
			if (cboFontChooser.SelectedItem != null)
				MainWindow.DbStudioViewsManager.ConfigurationController.EditorFontName = cboFontChooser.SelectedItem?.ToString() ?? string.Empty;
			MainWindow.DbStudioViewsManager.ConfigurationController.EditorFontSize = txtFontSize.Value;
			MainWindow.DbStudioViewsManager.ConfigurationController.EditorShowLinesNumber = chkShowLineNumber.IsChecked ?? false;
			MainWindow.DbStudioViewsManager.ConfigurationController.ShowWindowNotifications = chkShowNotifications.IsChecked ?? false;
			// Graba los datos de los plugins
			SavePluginsData();
			// Graba la configuración
			ViewsManager.ConfigurationController.Save();
			// Cierra la ventana
			Close();
		}
	}

	/// <summary>
	///		Graba los datos de configuración de los plugins
	/// </summary>
	private void SavePluginsData()
	{
		if (_configurationViews is not null)
			foreach (IPluginConfigurationView configurationView in _configurationViews)
				configurationView.Save();
	}

	/// <summary>
	///		Controlador principal
	/// </summary>
	public Controllers.DbStudioViewsManager ViewsManager { get; }

	private void cmdSave_Click(object sender, RoutedEventArgs e)
	{
		Save();
	}
}
