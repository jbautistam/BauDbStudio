using System;
using System.Threading.Tasks;
using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.ViewModels.Tools.Web;

namespace Bau.Libraries.PluginsStudio.Views.Tools.Web
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
		}

		/// <summary>
		///		Inicializa el control
		/// </summary>
		private async Task InitControlAsync()
		{
			await wbBrowser.ShowUrlAsync(ViewModel.Url);
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
