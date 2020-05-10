using System;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences
{
	/// <summary>
	///		Sentencia de descarga de un directorio de blobs de un contenedor
	/// </summary>
	internal class DownloadBlobFolderSentence : BaseBlobSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate()
		{
			if (string.IsNullOrWhiteSpace(Path))
				return "Path name undefined";
			else
				return Source.Validate();
		}

		/// <summary>
		///		Directorio de destino
		/// </summary>
		internal string Path { get; set; }

		/// <summary>
		///		Blob origen
		/// </summary>
		internal BlobModel Source { get; } = new BlobModel();
	}
}
