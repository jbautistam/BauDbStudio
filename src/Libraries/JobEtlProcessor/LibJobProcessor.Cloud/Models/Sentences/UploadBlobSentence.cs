using System;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences
{
	/// <summary>
	///		Sentencia para subir un archivo a un blob
	/// </summary>
	internal class UploadBlobSentence : BaseBlobSentence
	{
		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		protected override string Validate()
		{
			if (string.IsNullOrWhiteSpace(FileName))
				return "File name undefined";
			else
				return Target.Validate();
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Blob destino
		/// </summary>
		internal BlobModel Target { get; } = new BlobModel();
	}
}
