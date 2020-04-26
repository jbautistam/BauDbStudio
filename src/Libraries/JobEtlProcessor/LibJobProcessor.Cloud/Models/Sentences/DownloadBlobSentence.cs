using System;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences
{
	/// <summary>
	///		Sentencia de descarga de un blob
	/// </summary>
	internal class DownloadBlobSentence : BaseBlobSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate()
		{
			if (string.IsNullOrWhiteSpace(FileName))
				return "File name undefined";
			else
				return Source.Validate();
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Blob origen
		/// </summary>
		internal BlobModel Source { get; } = new BlobModel();
	}
}
