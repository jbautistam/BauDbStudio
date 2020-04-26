using System;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences
{
	/// <summary>
	///		Sentencia para copiar o mover blobs
	/// </summary>
	internal class CopyBlobSentence : BaseBlobSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate()
		{
			string error = Source.Validate();

				// Comprueba el resto de datos
				if (string.IsNullOrWhiteSpace(error))
					error = Target.Validate();
				// Devuelve el error
				return error;
		}

		/// <summary>
		///		Blob origen
		/// </summary>
		internal BlobModel Source { get; } = new BlobModel();

		/// <summary>
		///		Blob destino
		/// </summary>
		internal BlobModel Target { get; } = new BlobModel();

		/// <summary>
		///		Indica si se debe mover el archivo
		/// </summary>
		internal bool Move { get; set; }

		/// <summary>
		///		Indica si se debe cambiar el nombre de archivo destino por un nombre de archivo único
		/// </summary>
		internal bool TransformFileName { get; set; }
	}
}
