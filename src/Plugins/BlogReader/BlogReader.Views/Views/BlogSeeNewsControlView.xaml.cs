using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

using Bau.Libraries.LibBlogReader.ViewModel.Blogs;

namespace Bau.Libraries.BlogReader.Views.Views;

/// <summary>
///		Ventana para ver las noticias del blog
/// </summary>
public partial class BlogSeeNewsControlView : UserControl
{ 
	// Variables privadas
	private readonly DispatcherTimer _tmrRead;
	private DateTime _lastRead = DateTime.Now;

	public BlogSeeNewsControlView(BlogSeeNewsViewModel viewModel)
	{ 
		// Inicializa los componentes
		InitializeComponent();
		// Crea el temporizador de lectura
		_tmrRead = new DispatcherTimer();
		_tmrRead.Interval = new TimeSpan(0, 0, 30);
		_tmrRead.Tick += tmrRead_Tick;
		// Inicializa el ViewModel
		ViewModel = viewModel;
		grdData.DataContext = ViewModel;
		lswEntries.ItemsSource = ViewModel.EntriesList;
		// Observa el evento PropertyChanged para ver cuándo se cambia el Html
		ViewModel.PropertyChanged += (sender, args) =>
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
														args.PropertyName.Equals(nameof(BlogSeeNewsViewModel.HtmlNews), StringComparison.CurrentCultureIgnoreCase))
													ShowHtmlNews();
											};
		ViewModel.Closed += async (sender, args) => await DestroyWindowAsync();
		// Añade el viewModel al explorador
		wbExplorer.DataContext = ViewModel;
		wbExplorer.FunctionExecute += (sender, evntArgs) => ViewModel.ExecuteFromExplorer(evntArgs.Parameters);
		wbExplorer.BeforeNavigateTo += TreatExplorerNavigateTo;
		wbExplorer.OpenWindowRequested += (sender, args) => OpenBrowser(args.Url);
		// Agrupa los elementos
		if (lswEntries.ItemsSource != null)
		{
			CollectionView views = (CollectionView) CollectionViewSource.GetDefaultView(lswEntries.ItemsSource);
			PropertyGroupDescription groupDescription = new PropertyGroupDescription("BlogName");

				views.GroupDescriptions.Add(groupDescription);
		}
		// Muestra el HTML
		ShowHtmlNews();
	}

	/// <summary>
	///		Muestra el HTML de las noticias
	/// </summary>
	private void ShowHtmlNews()
	{
		// Inicializa el temporizador de lectura
		_tmrRead.Start();
		// Muestra el HTML en el navegador
		wbExplorer.HtmlContent = ViewModel.HtmlNews;
	}

	/// <summary>
	///		Marca los elementos seleccionados como leídos
	/// </summary>
	private void MarkRead()
	{
		ViewModel.MarkRead();
	}

	/// <summary>
	///		Trata la navegación a una URL: abre un nuevo explorador si es necesario
	/// </summary>
	private void TreatExplorerNavigateTo(object? sender, Controls.WebExplorers.WebExplorerNavigateToEventArgs e)
	{
		OpenBrowser(e.Url);
		e.Cancel = true;
	}

	/// <summary>
	///		Abre el navegador sobre una ventana
	/// </summary>
	private void OpenBrowser(string url)
	{
		ViewModel.MainViewModel.ViewsController.HostPluginsController.OpenWebBrowser(url);
	}

	/// <summary>
	///		Destruye la ventana cuando se cierra
	/// </summary>
	private async Task DestroyWindowAsync()
	{
		// Libera el temporizador
		_tmrRead.Stop();
		_tmrRead.Tick -= tmrRead_Tick;
		// Cierra el navegador
		await wbExplorer.ShowHtmlAsync(string.Empty);
		wbExplorer.Dispose();
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public BlogSeeNewsViewModel ViewModel { get; }

	/// <summary>
	///		Tratamiento del evento del temporizador
	/// </summary>
	private void tmrRead_Tick(object? sender, EventArgs e)
	{
		if ((DateTime.Now - _lastRead).TotalSeconds > 30 && IsVisible)
		{ 
			// Detiene el temporizador y guarda la hora
			_lastRead = DateTime.Now;
			_tmrRead.Stop();
			// Marca los elementos como leídos
			MarkRead();
		}
	}

	private void lswEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ViewModel.MarkAsDirty();
	}

	private void DeleteCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = true;
	}

	private void DeleteCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ViewModel.DeleteCommand.Execute(null);
	}
}