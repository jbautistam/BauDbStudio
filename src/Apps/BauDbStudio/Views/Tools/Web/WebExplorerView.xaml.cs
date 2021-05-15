using System;
using System.Threading.Tasks;
using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.ViewModels.Tools.Web;

namespace Bau.DbStudio.Views.Tools.Web
{
	/// <summary>
	///		Ventana para mostrar el contenido de un archivo
	/// </summary>
	public partial class WebExplorerView : UserControl
	{
		public WebExplorerView(WebViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
			viewModel.Closed += async (sender, args) => await DestroyWindowAsync();
		}

		/// <summary>
		///		Inicializa el control
		/// </summary>
		private async Task InitControlAsync()
		{
			await wbBrowser.ShowUrlAsync(ViewModel.Url);
		}

		/// <summary>
		///		Libera la memoria de la ventana antes de cerrar
		/// </summary>
		private async Task DestroyWindowAsync()
		{
			await wbBrowser.ShowHtmlAsync(string.Empty);
			wbBrowser.Dispose();
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public WebViewModel ViewModel { get; }

		private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			await InitControlAsync();
		}
	}
}
