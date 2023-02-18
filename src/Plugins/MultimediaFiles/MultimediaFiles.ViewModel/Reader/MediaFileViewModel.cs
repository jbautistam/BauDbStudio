using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.MultimediaFiles.ViewModel.Reader
{
	/// <summary>
	///		ViewModel para ver el contenido de un archivo multimedia
	/// </summary>
	public class MediaFileViewModel : BaseObservableObject
	{
		// Variables privadas
		private string _fileName, _shortFileName;
		private bool _isAudio, _isPlaying;

		public MediaFileViewModel(MediaFileListViewModel mediaFileListView, string fileName, bool isAudio) : base(false)
		{ 
			// Asigna los objetos
			MediaFileListViewModel = mediaFileListView;
			// Asigna las propiedades
			FileName = fileName;
			if (!string.IsNullOrWhiteSpace(FileName))
				ShortFileName = System.IO.Path.GetFileName(fileName);
			else
				ShortFileName = "No file";
			IsAudio = isAudio;
			IsPlaying = false;
			// Asigna los comandos
			PlayCommand = new BaseCommand(_ => MediaFileListViewModel.Play(), _ => CanPlay())
									.AddListener(this, nameof(IsPlaying));
			PauseCommand = new BaseCommand(_ => MediaFileListViewModel.Pause(), _ => CanStop())
									.AddListener(this, nameof(IsPlaying));
			StopCommand = new BaseCommand(_ => MediaFileListViewModel.Stop(), _ => CanStop())
									.AddListener(this, nameof(IsPlaying));
		}

		/// <summary>
		///		Indica si se puede activar la visualización
		/// </summary>
		private bool CanPlay()
		{
			return !string.IsNullOrWhiteSpace(FileName) && System.IO.File.Exists(FileName) && !IsPlaying;
		}

		/// <summary>
		///		Indica si se puede detener la visualización
		/// </summary>
		private bool CanStop()
		{
			return IsPlaying;
		}

		/// <summary>
		///		ViewModel de la lista de archivos
		/// </summary>
		public MediaFileListViewModel MediaFileListViewModel { get; set; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string ShortFileName
		{
			get { return _shortFileName; }
			set { CheckProperty(ref _shortFileName, value); }
		}

		/// <summary>
		///		Indica si se está visualizando el archivo
		/// </summary>
		public bool IsPlaying
		{
			get { return _isPlaying; }
			set { CheckProperty(ref _isPlaying, value); }
		}

		/// <summary>
		///		Indica si es un archivo de audio
		/// </summary>
		public bool IsAudio
		{
			get { return _isAudio; }
			set { CheckProperty(ref _isAudio, value); }
		}

		/// <summary>
		///		Comando para activar el archivo multimedia
		/// </summary>
		public BaseCommand PlayCommand { get; }

		/// <summary>
		///		Comando para pausar el archivo multimedia
		/// </summary>
		public BaseCommand PauseCommand { get; }

		/// <summary>
		///		Comando para detener el archivo multimedia
		/// </summary>
		public BaseCommand StopCommand { get; }
	}
}
