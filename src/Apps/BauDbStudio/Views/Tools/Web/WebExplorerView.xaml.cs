using System.Windows.Controls;

using Bau.Libraries.PluginsStudio.ViewModels.Tools.Web;

namespace Bau.DbStudio.Views.Tools.Web;

/// <summary>
///		Ventana para mostrar el contenido de un archivo
/// </summary>
public partial class WebExplorerView : UserControl
{
	// Variables privadas
	private bool _isLoadedUrl;

	public WebExplorerView(WebViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		wbBrowser.EndNavigate += (sender, args) => 
										{
											viewModel.Header = wbBrowser.Title;
											viewModel.Url = args.Url;
										};
		wbBrowser.OpenWindowRequested += (sender, args) => ViewModel.OpenBrowser(args.Url);
		viewModel.Closed += async (sender, args) => await DestroyWindowAsync();
		ViewModel.RefreshPage += async (sender, args) => await ShowUrlAsync();
		ViewModel.PropertyChanged += async (sender, args) =>
										{
											if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
													args.PropertyName.Equals(nameof(WebViewModel.Html), StringComparison.CurrentCultureIgnoreCase))
												await ShowHtmlAsync(viewModel.Html);
										};
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private async Task InitControlAsync()
	{
		if (!_isLoadedUrl) // ... entra por Loaded cada vez que se cambia de control (por tanto, cada vez que se cambia de ficha)
		{
			// Muestra la URL
			await ShowUrlAsync();
			// Indica que ya se ha cargado
			_isLoadedUrl = true;
		}
	}

	/// <summary>
	///		Muestra la URL
	/// </summary>
	private async Task ShowUrlAsync()
	{
		await wbBrowser.ShowUrlAsync(ViewModel.Url);
	}

	/// <summary>
	///		Muestra el HTML en el explorador
	/// </summary>
	private async Task ShowHtmlAsync(string html)
	{
		await wbBrowser.ShowHtmlAsync(html);
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
