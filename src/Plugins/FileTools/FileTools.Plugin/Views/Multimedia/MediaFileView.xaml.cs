using System.Windows;

using Bau.Libraries.FileTools.ViewModel.Multimedia;

namespace Bau.Libraries.FileTools.Plugin.Views.Multimedia;

/// <summary>
///		Ventana de visualización de archivos multimedia
/// </summary>
public partial class MediaFileView : Window
{
	// Variables privadas
	private System.Timers.Timer _tmrTimer;

	public MediaFileView(MediaFileViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Inicializa el DataContext
		grdView.DataContext = ViewModel = viewModel;
		// Crea el temporizador
		_tmrTimer = new System.Timers.Timer(TimeSpan.FromSeconds(2).TotalMilliseconds);
		_tmrTimer.Elapsed += (sender, args) => Dispatcher.Invoke(new Action(() => UpdateProgressBar()), null);
		// Asigna los manejadores de eventos
		Unloaded += (sender, args) => Stop();
		ViewModel.PlayFile += (sender, args) => Play();
		ViewModel.PauseFile += (sender, args) => Pause();
		ViewModel.StopFile += (sender, args) => Stop();
		// Asigna los manejadores de eventos del MediaPlayer
		plyMedia.MediaOpened += (sender, args) => ViewModel.IsPlaying = true;
		plyMedia.MediaEnded += (sender, args) => UpdateProgressBar();
		DataContext = ViewModel = viewModel;
		ViewModel.PropertyChanged += (sender, args) => {
															if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																	args.PropertyName.Equals(nameof(MediaFileViewModel.OverAllWindows), 
																							 StringComparison.CurrentCultureIgnoreCase))
																Topmost = ViewModel.OverAllWindows;
													   };
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private void InitForm()
	{
		// Controla el cambio de tamaño
		WindowStartupLocation = WindowStartupLocation.Manual;
		ResizeMode = ResizeMode.CanResizeWithGrip | ResizeMode.CanMinimize | ResizeMode.CanResize;
		WindowStyle = WindowStyle.SingleBorderWindow;
		ShowInTaskbar = true;
		// Muestra sobre todo lo demás
		Topmost = ViewModel.OverAllWindows;
	}

	/// <summary>
	///		Comienza la visualización el archivo
	/// </summary>
	private void Play()
	{
		if (!string.IsNullOrWhiteSpace(ViewModel.FileName) && System.IO.File.Exists(ViewModel.FileName))
		{
			// Reproduce el archivo
			if (!new Uri(ViewModel.FileName).Equals(plyMedia.Source))
				plyMedia.Source = new Uri(ViewModel.FileName);
			// Pone en marcha el archivo
			plyMedia.Play();
			// Muestra la barra de progreso e indica que se está reproduciendo un archivo
			ShowProgressBar();
		}
	}

	/// <summary>
	///		Detiene la visualización del archivo
	/// </summary>
	private void Pause()
	{
		plyMedia.Pause();
	}

	/// <summary>
	///		Muestra la barra de progreso
	/// </summary>
	private void ShowProgressBar()
	{
		// Muestra la barra de progreso
		prgMedia.Minimum = 0;
		if (plyMedia.NaturalDuration.HasTimeSpan)
			prgMedia.Maximum = plyMedia.NaturalDuration.TimeSpan.TotalSeconds;
		prgMedia.Value = 0;
		prgMedia.Visibility = Visibility.Visible;
		// Arranca el temporizador
		_tmrTimer.Start();
	}

	/// <summary>
	///		Inicia o detiene la visualización
	/// </summary>
	private void PlayOrStop()
	{
		if (ViewModel.IsPlaying)
			ViewModel.Pause();
		else
			ViewModel.Play();
	}

	/// <summary>
	///		Actualiza la barra de progreso
	/// </summary>
	private void UpdateProgressBar()
	{
		// Actualiza el máximo (puede que a la hora de mostrar la barra el control aún no tenga los datos)
		if (plyMedia.NaturalDuration.HasTimeSpan)
			prgMedia.Maximum = plyMedia.NaturalDuration.TimeSpan.TotalSeconds;
		// Actualiza la posición
		if (prgMedia.IsVisible && prgMedia.Maximum >= plyMedia.Position.TotalSeconds)
			prgMedia.Value = plyMedia.Position.TotalSeconds;
	}

	/// <summary>
	///		Detiene la reproducción de un archivo
	/// </summary>
	private void Stop()
	{
		// Detiene la reproducción
		plyMedia.Stop();
		// y oculta la barra de progreso y detiene el temporizador
		prgMedia.Visibility = Visibility.Hidden;
		_tmrTimer.Stop();
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public MediaFileViewModel ViewModel { get; }

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		InitForm();
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		Stop();
        }

	private void prgMedia_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		double x = e.GetPosition(prgMedia).X;

			if (prgMedia.ActualWidth > 0 && x > 0)
			{
				double percent = Math.Min(x * 100 / prgMedia.ActualWidth, 100.0);
				TimeSpan newPosition = TimeSpan.FromSeconds(prgMedia.Maximum * percent / 100);

					// Cambia la posición del vídeo
					if (newPosition.TotalSeconds < plyMedia.NaturalDuration.TimeSpan.TotalSeconds)
						plyMedia.Position = newPosition;
			}
	}

	private void plyMedia_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		PlayOrStop();
	}
}