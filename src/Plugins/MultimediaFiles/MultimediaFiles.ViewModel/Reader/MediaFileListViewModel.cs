using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.MultimediaFiles.ViewModel.Reader
{
	/// <summary>
	///		ViewModel para una lista de <see cref="MediaFileViewModel"/>
	/// </summary>
	public class MediaFileListViewModel : BaseObservableObject
	{
		// Eventos públícos
		public EventHandler<EventArguments.MediaFileEventArgs> PlayFile;
		public EventHandler<EventArguments.MediaFileEventArgs> PauseFile;
		public EventHandler<EventArguments.MediaFileEventArgs> StopFile;
		// Variables privadas
		private MediaFileViewModel _actualFile;

		public MediaFileListViewModel(MultimediaFilesViewModel mainViewModel) : base(false)
		{ 
			MainViewModel = mainViewModel;
			ActualFile = new MediaFileViewModel(this, string.Empty, false);
		}

		/// <summary>
		///		Reproduce un archivo
		/// </summary>
		internal void PlayMediaFile(string fileName, bool isAudio)
		{
			// Cambia el archivo actual
			ActualFile = new MediaFileViewModel(this, fileName, isAudio);
			// Lanza el evento
			PlayFile?.Invoke(this, new EventArguments.MediaFileEventArgs(ActualFile));
		}

		/// <summary>
		///		Comienza la ejecución del archivo multimedia
		/// </summary>
		public void Play()
		{
			if (ActualFile != null)
			{
				// Indica que se está ejecutando
				ActualFile.IsPlaying = true;
				// Lanza el evento
				PlayFile?.Invoke(this, new EventArguments.MediaFileEventArgs(ActualFile));
			}
		}

		/// <summary>
		///		Comienza la ejecución del archivo multimedia
		/// </summary>
		public void Pause()
		{
			if (ActualFile != null && ActualFile.IsPlaying)
			{
				// Indica que ya no se está ejecutando
				ActualFile.IsPlaying = false;
				// Lanza el evento
				PauseFile?.Invoke(this, new EventArguments.MediaFileEventArgs(ActualFile));
			}
		}

		/// <summary>
		///		Detiene la ejecución del archivo multimedia
		/// </summary>
		public void Stop()
		{
			if (ActualFile != null && ActualFile.IsPlaying)
			{
				// Indica que se está ejecutando
				//? Tiene que ir antes de llamar al evento stop
				ActualFile.IsPlaying = false;
				// Lanza el evento
				StopFile?.Invoke(this, new EventArguments.MediaFileEventArgs(ActualFile));
			}
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MultimediaFilesViewModel MainViewModel { get; }

		/// <summary>
		///		Archivo actual
		/// </summary>
		public MediaFileViewModel ActualFile
		{
			get { return _actualFile; }
			set { CheckObject(ref _actualFile, value); }
		}
	}
}
