using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.Notes;

namespace Bau.Libraries.ToDoManager.Plugin.Views.Notes;

/// <summary>
///		Ventana para mostrar los datos de la nota
/// </summary>
public partial class NoteView : Window
{
	// Variables privadas
	private bool _isOpening = true;

	public NoteView(NoteViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.PropertyChanged += (sender, args) => {
															if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																	args.PropertyName.Equals(nameof(NoteViewModel.OverAllWindows), StringComparison.CurrentCultureIgnoreCase))
																Topmost = ViewModel.OverAllWindows;
													   };
		ViewModel.Close += (sender, eventArgs) => Close();
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private void InitForm()
	{
		// Controla el cambio de tamaño
		WindowStyle = WindowStyle.None;
		WindowStartupLocation = WindowStartupLocation.Manual;
		ResizeMode = ResizeMode.CanResizeWithGrip;
		ShowInTaskbar = false;
		// Cambia la posición / tamaño
		Top = ViewModel.Top;
		Left = ViewModel.Left;
		Width = ViewModel.Width;
		Height = ViewModel.Height;
		// Muestra sobre todo lo demás
		Topmost = ViewModel.OverAllWindows;
		// Indica que ya no se está abriendo la ventana
		_isOpening = false;
	}

	/// <summary>
	///		Actualiza las posiciones de la nota
	/// </summary>
	private void UpdatePositions()
	{
		if (!_isOpening)
		{
			ViewModel.Top = (int) Top;
			ViewModel.Left = (int) Left;
			ViewModel.Height = (int) Height;
			ViewModel.Width = (int) Width;
		}
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public NoteViewModel ViewModel { get; }

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		InitForm();
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		UpdatePositions();
		ViewModel.SaveCommand.Execute(null);
        }

	private void cmdClose_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void Window_LocationChanged(object sender, EventArgs e)
	{
		UpdatePositions();
	}

	private void mnuYellow_Click(object sender, RoutedEventArgs e)
	{
		ViewModel.Background = BauMvvm.ViewModels.Media.MvvmColor.Yellow;
	}

	private void mnuGreen_Click(object sender, RoutedEventArgs e)
	{
		ViewModel.Background = BauMvvm.ViewModels.Media.MvvmColor.Green;
	}

	private void mnuRed_Click(object sender, RoutedEventArgs e)
	{
		ViewModel.Background = BauMvvm.ViewModels.Media.MvvmColor.Red;
	}

	private void mnuBlue_Click(object sender, RoutedEventArgs e)
	{
		ViewModel.Background = BauMvvm.ViewModels.Media.MvvmColor.Navy;
	}

	private void mnuWhite_Click(object sender, RoutedEventArgs e)
	{
		ViewModel.Background = BauMvvm.ViewModels.Media.MvvmColor.White;
	}

	private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
			DragMove();
	}
}