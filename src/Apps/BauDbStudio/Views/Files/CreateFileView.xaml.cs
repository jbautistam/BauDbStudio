using System.Windows;

using Bau.Libraries.PluginsStudio.ViewModels.Tools;

namespace Bau.DbStudio.Views.Files;

/// <summary>
///		Vista para seleccionar un nombre de archivo
/// </summary>
public partial class CreateFileView : Window
{
	public CreateFileView(CreateFileViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.SelectEncoding(ViewModel.MainViewModel.MainController.PluginsController.ConfigurationController.LastEncodingIndex);
		ViewModel.ComboTypes.PropertyChanged += (sender, args) =>
													{
														if (!string.IsNullOrWhiteSpace(args.PropertyName) && args.PropertyName.Equals(nameof(ViewModel.ComboTypes.SelectedItem)))
															txtFileName.Focus();
													};
		ViewModel.Close += (sender, eventArgs) => 
								{
									// Guarda la codificación
									if (eventArgs.IsAccepted)
									{
										ViewModel.MainViewModel.MainController.PluginsController.ConfigurationController.LastEncodingIndex = (int) ViewModel.GetSelectedEncoding();
										ViewModel.MainViewModel.MainController.PluginsController.ConfigurationController.Save();
									}
									DialogResult = eventArgs.IsAccepted; 
									Close();
								};
	}

	/// <summary>
	///		Selecciona en el cuadro de texto del nombre de archivo sólo el nombre sin la extensión
	/// </summary>
	private void SelectFileName()
	{
		if (!string.IsNullOrEmpty(txtFileName.Text))
		{
			int index = txtFileName.Text.IndexOf('.');

				if (index > 0)
				{
					txtFileName.CaretIndex = 0;
					txtFileName.Select(0, index);
				}
		}
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public CreateFileViewModel ViewModel { get; }

	private void txtFileName_GotFocus(object sender, RoutedEventArgs e)
	{
		SelectFileName();
	}
}
