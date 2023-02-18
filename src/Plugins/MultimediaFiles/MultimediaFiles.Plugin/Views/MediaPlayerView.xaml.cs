using System;
using System.Windows.Controls;

using Bau.Libraries.MultimediaFiles.ViewModel.Reader;

namespace Bau.Libraries.MultimediaFiles.Plugin.Views
{
	/// <summary>
	///		Ventana de visualización de archivos multimedia
	/// </summary>
	public partial class MediaPlayerView : UserControl
	{   
		// Variables privadas
		private System.Timers.Timer _tmrTimer;

		public MediaPlayerView(MediaFileListViewModel viewModel)
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
			ViewModel.PlayFile += (sender, args) => Play(args.FileViewModel);
			ViewModel.PauseFile += (sender, args) => Pause();
			ViewModel.StopFile += (sender, args) => Stop();
			// Asigna los manejadores de eventos del MediaPlayer
			plyMedia.MediaOpened += (sender, args) => ViewModel.ActualFile.IsPlaying = true;
			plyMedia.MediaEnded += (sender, args) => UpdateProgressBar();
		}

		/// <summary>
		///		Comienza la visualización el archivo
		/// </summary>
		private void Play(MediaFileViewModel fileViewModel)
		{
			if (fileViewModel is not null && !string.IsNullOrWhiteSpace(fileViewModel.FileName) &&
				System.IO.File.Exists(fileViewModel.FileName))
			{
				// Reproduce el archivo
				if (!new Uri(fileViewModel.FileName).Equals(plyMedia.Source))
					plyMedia.Source = new Uri(fileViewModel.FileName);
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
			prgMedia.Visibility = System.Windows.Visibility.Visible;
			// Arranca el temporizador
			_tmrTimer.Start();
		}

		/// <summary>
		///		Inicia o detiene la visualización
		/// </summary>
		private void PlayOrStop()
		{
			if (ViewModel.ActualFile.IsPlaying)
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
			prgMedia.Visibility = System.Windows.Visibility.Hidden;
			_tmrTimer.Stop();
		}

		/// <summary>
		///		ViewModel de lista de archivos multimedia
		/// </summary>
		public MediaFileListViewModel ViewModel { get; }

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
}
