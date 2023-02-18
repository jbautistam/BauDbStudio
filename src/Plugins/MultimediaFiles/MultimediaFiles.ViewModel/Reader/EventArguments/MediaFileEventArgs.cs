using System;

namespace Bau.Libraries.MultimediaFiles.ViewModel.Reader.EventArguments
{
	/// <summary>
	///		Argumentos de los eventos de tratamiento de archivos multimedia
	/// </summary>
	public class MediaFileEventArgs : EventArgs
	{
		public MediaFileEventArgs(MediaFileViewModel fileViewModel)
		{
			FileViewModel = fileViewModel;
		}

		/// <summary>
		///		Archivo
		/// </summary>
		public MediaFileViewModel FileViewModel { get; }
	}
}
