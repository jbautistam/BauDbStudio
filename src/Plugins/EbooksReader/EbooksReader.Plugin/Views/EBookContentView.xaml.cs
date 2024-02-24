using System.Windows.Controls;

using Bau.Libraries.EbooksReader.ViewModel.Reader.eBooks;

namespace Bau.Libraries.EbooksReader.Plugin.Views;

/// <summary>
///		Formulario para mostrar el contenido de un cómic
/// </summary>
public partial class EBookContentView : UserControl
{
	// Variables privadas
	private bool _isLoaded;

	public EBookContentView(EBookContentViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna la clase del documento
		DataContext = ViewModel = viewModel;
		ViewModel.ChangePage += async (sender, args) => await UpdatePageAsync();
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
	///		Modifica la página visualizada
	/// </summary>
	private async Task UpdatePageAsync()
	{
		string url = ViewModel.GetUrlPage();

			// Comprueba la URL
			if (string.IsNullOrWhiteSpace(url))
				url = "about:blank";
			// Cambia el explorador
			await wbExplorer.ShowUrlAsync(url);
	}

	/// <summary>
	///		ViewModel asociado al control
	/// </summary>
	public EBookContentViewModel ViewModel { get; }

	private async void UserControl_Loaded(object sender, EventArgs e)
	{
		await InitControlAsync();
	}

	private void trvExplorer_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
	{
		if ((sender as TreeView)?.SelectedItem is ViewModel.Reader.eBooks.Explorer.EbookNodeViewModel node)
			ViewModel.TreePages.SelectedNode = node;
	}
}