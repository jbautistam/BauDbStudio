using System.Windows.Controls;

using Bau.Libraries.EbooksReader.ViewModel.Reader.Comics;

namespace Bau.Libraries.EbooksReader.Plugin.Views;

/// <summary>
///		Formulario para mostrar el contenido de un cómic
/// </summary>
public partial class ComicContentView : UserControl
{
	// Variables privadas
	private bool _isLoaded;

	public ComicContentView(ComicContentViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna la clase del documento
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private async Task InitControlAsync()
	{
		if (!_isLoaded)
		{
			// Indica que ya no se debe cargar de nuevo
			_isLoaded = true;
			// Carga el archivo
			await ViewModel.ParseAsync();
			// Asigna las propiedades al control de imagen
			ZoomAndPanControl.MinimumZoomType = Controls.ZoomAndPanControls.ZoomAndPanControl.MinimumZoomTypeEnum.FitScreen;
			ZoomAndPanControl.ZoomAndPanContent.MinimumZoomType = Controls.ZoomAndPanControls.ZoomAndPanControl.MinimumZoomTypeEnum.FitScreen;
			ZoomAndPanControl.ZoomAndPanContent.MaximumZoom = 3;
			ZoomAndPanControl.ZoomAndPanContent.MinimumZoom = 0.25;
			// Asigna los manejadores de eventos sobre el control de imagen
			ZoomAndPanControl.ZoomAndPanContent.ContentZoomChanged += (sender, args) => ViewModel.Zoom = ZoomAndPanControl.ZoomAndPanContent.ViewportZoom;
			// Asigna los manejadores de eventos sobre el ViewModel
			ViewModel.UpdateZoom += (sender, args) => ZoomAndPanControl.ZoomAndPanContent.ViewportZoom = args.Zoom; 
		}
	}

	/// <summary>
	///		ViewModel asociado al control
	/// </summary>
	public ComicContentViewModel ViewModel { get; }

	private async void UserControl_Loaded(object sender, EventArgs e)
	{
		await InitControlAsync();
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

	private void lstThumbs_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (lstThumbs.SelectedItem != null)
			lstThumbs.ScrollIntoView(lstThumbs.SelectedItem);
	}

	private void ZoomAndPanControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		switch (e.Key)
		{
			case System.Windows.Input.Key.PageDown:
					ViewModel.GoNextPage();
				break;
			case System.Windows.Input.Key.PageUp:
					ViewModel.GoPreviousPage();
				break;
		}
	}
}