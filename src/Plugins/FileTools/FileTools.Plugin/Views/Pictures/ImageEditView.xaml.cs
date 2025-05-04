using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Logging;

using Bau.Libraries.FileTools.ViewModel.Pictures;

namespace Bau.Libraries.FileTools.Plugin.Views.Pictures;

/// <summary>
///		Ventana para editar una imagen
/// </summary>
public partial class ImageEditView : UserControl
{
	public ImageEditView(ImageEditViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		// Indica al viewmodel que se cargue
		viewModel.Load();
		// Carga el archivo
		LoadImage(viewModel.FileName);
		// Asigna los manejadores de eventos
		viewModel.SaveImage += (sender, args) => SaveAs(viewModel.FileName);
		//// Cambia el zoom
		//pnlZoom.ZoomMode = Bau.Controls.Graphical.ImageZoomBoxPanel.eZoomMode.ActualSize;
	}

	/// <summary>
	///		Carga una imagen
	/// </summary>
	private void LoadImage(string fileName)
	{
		try
		{
			// Asigna la imagen
			imgImage.Source = CreateBitmapImage(fileName);
			// Muestra las propiedades de la imagen
			// lblStatus.Text = $"Dimensiones {image.PixelWidth} x {image.PixelHeight}";
			// Inicializa los controles
			if (ZoomAndPanControl.ZoomAndPanContent is not null)
			{
				ZoomAndPanControl.ZoomAndPanContent.MinimumZoom = 0.25;
				ZoomAndPanControl.ZoomAndPanContent.MaximumZoom = 4;
				ZoomAndPanControl.ZoomAndPanContent.ZoomAndPanInitialPosition = Controls.ZoomAndPanControls.ZoomAndPanInitialPositionEnum.FitScreen;
			}
			// Oculta el thumb
			chkShowThumb.IsChecked = false;
			wndZoom.Visibility = System.Windows.Visibility.Collapsed;
		}
		catch (Exception exception)
		{
			ViewModel.MainViewModel.MainController.MainWindowController.HostController.SystemController.ShowMessage($"Error when load image {fileName}");
			ViewModel.MainViewModel.MainController.Logger.LogError(exception, $"Error when load image {fileName}");
		}
	}

	/// <summary>
	///		Crea la imagen en memoria
	/// </summary>
	private ImageSource? CreateBitmapImage(string fileName)
	{
		if (System.IO.File.Exists(fileName))
			return new BauMvvm.Views.Wpf.Tools.ImageToolsWpf().GetFromFileName(fileName);
		else
			return null;
	}

	/// <summary>
	///		Graba la imagen con otro nombre
	/// </summary>
	private void SaveAs(string fileName)
	{
		bool saved = false;

			// Graba la imagen
			if (imgImage.Source is BitmapImage image)
				saved = new BauMvvm.Views.Wpf.Tools.ImageHelper().SaveImage(image, fileName);
			// Si no se ha grabado, muestra un mensaje al usuario, si se ha grabado, actualiza el árbol
			if (!saved)
				ViewModel.MainViewModel.MainController.MainWindowController.HostController.SystemController.ShowMessage($"Can't save the image {fileName}");
			else
			{
				// Actualiza el árbol
				ViewModel.MainViewModel.MainController.HostPluginsController.RefreshFiles();
				// Añade el archivo a los últimos archivos abiertos
				ViewModel.MainViewModel.MainController.HostPluginsController.AddFileUsed(fileName);
			}
	}

	private void chkShowThumb_Click(object sender, System.Windows.RoutedEventArgs e)
	{
		if (wndZoom != null)
		{
			if (chkShowThumb.IsChecked ?? false)
				wndZoom.Visibility = System.Windows.Visibility.Visible;
			else
				wndZoom.Visibility = System.Windows.Visibility.Collapsed;
		}
	}

	/// <summary>
	///		ViewModel de la imagen
	/// </summary>
	private ImageEditViewModel ViewModel { get; }
}
