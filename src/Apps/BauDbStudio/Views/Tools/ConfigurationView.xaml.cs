using System;
using System.Windows;

namespace Bau.DbStudio.Views.Tools
{
	/// <summary>
	///		Vista para mostrar la configuración
	/// </summary>
	public partial class ConfigurationView : Window
	{
		public ConfigurationView()
		{
			// Inicializa los componentes
			InitializeComponent();
			// Inicializa los valores con la configuración
			InitForm();
		}

		/// <summary>
		///		Inicializa el formulario con la configuración
		/// </summary>
		private void InitForm()
		{
			for (int index = 0; index < cboFontChooser.Items.Count; index++)
				if (cboFontChooser.Items[index].ToString().Equals(MainWindow.MainController.ConfigurationController.EditorFontName, StringComparison.CurrentCultureIgnoreCase))
					cboFontChooser.SelectedItem = cboFontChooser.Items[index];
			if (cboFontChooser.SelectedItem == null && cboFontChooser.Items.Count > 0)
				cboFontChooser.SelectedIndex = 0;
			txtFontSize.Value = MainWindow.MainController.ConfigurationController.EditorFontSize;
			chkShowLineNumber.IsChecked = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
			fnConsole.FileName = MainWindow.MainController.ConfigurationController.ConsoleExecutable;
			chkShowNotifications.IsChecked = MainWindow.MainController.ConfigurationController.ShowWindowNotifications;
		}

		/// <summary>
		///		Comprueba los datos introducidos en el formulario
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (!string.IsNullOrWhiteSpace(fnConsole.FileName) && !System.IO.File.Exists(fnConsole.FileName))
					MainWindow.MainController.MainWindowController.HostController.SystemController.ShowMessage("No se encuentra el archivo de ejecución de consola");
				else
					validated = true;
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
					MainWindow.MainController.ConfigurationController.EditorFontName = cboFontChooser.SelectedItem.ToString();
				MainWindow.MainController.ConfigurationController.EditorFontSize = txtFontSize.Value;
				MainWindow.MainController.ConfigurationController.EditorShowLinesNumber = chkShowLineNumber.IsChecked ?? false;
				MainWindow.MainController.ConfigurationController.ConsoleExecutable = fnConsole.FileName;
				MainWindow.MainController.ConfigurationController.ShowWindowNotifications = chkShowNotifications.IsChecked ?? false;
				// Graba la configuración
				MainWindow.MainController.ConfigurationController.Save();
				// Cierra la ventana
				Close();
			}
		}

		private void cmdAccept_Click(object sender, RoutedEventArgs e)
		{
			Save();
		}
	}
}
