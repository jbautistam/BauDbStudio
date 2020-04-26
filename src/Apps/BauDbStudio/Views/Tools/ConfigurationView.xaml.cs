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
		}

		/// <summary>
		///		Graba los datos de la configuración
		/// </summary>
		private void Save()
		{
			// Cambia los valores
			if (cboFontChooser.SelectedItem != null)
				MainWindow.MainController.ConfigurationController.EditorFontName = cboFontChooser.SelectedItem.ToString();
			MainWindow.MainController.ConfigurationController.EditorFontSize = txtFontSize.Value ?? 10;
			MainWindow.MainController.ConfigurationController.EditorShowLinesNumber = chkShowLineNumber.IsChecked ?? false;
			// Cierra la ventana
			Close();
		}

		private void cmdAccept_Click(object sender, RoutedEventArgs e)
		{
			Save();
		}
	}
}
