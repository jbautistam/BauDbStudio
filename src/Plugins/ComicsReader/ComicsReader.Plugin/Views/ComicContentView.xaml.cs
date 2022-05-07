using System;
using System.Windows.Controls;
using System.Threading.Tasks;

using Bau.Libraries.ComicsReader.ViewModel.Reader;

namespace Bau.Libraries.ComicsReader.Plugin.Views
{
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
	}
}