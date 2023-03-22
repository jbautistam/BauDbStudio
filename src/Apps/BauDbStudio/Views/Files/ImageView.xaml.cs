using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Bau.Libraries.PluginsStudio.ViewModels.Files;

namespace Bau.DbStudio.Views.Files
{
	/// <summary>
	///		Ventana para presentar una imagen
	/// </summary>
	public partial class ImageView : UserControl
	{
		public ImageView(ImageViewModel viewModel)
		{
			// Inicializa los componentes
			InitializeComponent();
			// Carga el archivo
			LoadImage(viewModel.FileName);
			//// Cambia el zoom
			//pnlZoom.ZoomMode = Bau.Controls.Graphical.ImageZoomBoxPanel.eZoomMode.ActualSize;
		}

		/// <summary>
		///		Carga una imagen
		/// </summary>
		private void LoadImage(string fileName)
		{
			// Asigna la imagen
			imgImage.Source = CreateBitmapImage(fileName);
			// Muestra las propiedades de la imagen
			// lblStatus.Text = $"Dimensiones {image.PixelWidth} x {image.PixelHeight}";
			// Inicializa los controles
			try
			{
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
				System.Diagnostics.Debug.WriteLine($"Error when set image zoom. {exception.Message}");
			}
		}

		/// <summary>
		///		Crea la imagen en memoria
		/// </summary>
		private ImageSource CreateBitmapImage(string fileName)
		{
			BitmapImage image = new BitmapImage();

				// Lee el archivo sobre la imagen
				image.BeginInit();
				image.StreamSource = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.EndInit();
				// Libera el stream para evitar excepciones de acceso al archivo cuando se intenta borrar la imagen
				image.StreamSource.Dispose();
				// Asigna la imagen
				return image;
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
	}
}
