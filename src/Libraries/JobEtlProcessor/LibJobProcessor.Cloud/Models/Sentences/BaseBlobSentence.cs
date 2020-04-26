using System;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences
{
	/// <summary>
	///		Sentencia base para las sentencias de blob
	/// </summary>
	internal abstract class BaseBlobSentence : BaseSentence
	{
		/// <summary>
		///		Valida la sentencia
		/// </summary>
		protected override string Validate(Core.Models.JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(StorageKey))
				return "Storage key undefined";
			//else if (!project.Context.BlobStorages.ContainsKey(StorageKey))
			//	error = $"Can't find the blob storage {StorageKey}";
			else 
				return Validate();
		}

		/// <summary>
		///		Valida la sentencia
		/// </summary>
		protected abstract string Validate();

		/// <summary>
		///		Clave del storage
		/// </summary>
		internal string StorageKey { get; set; }
	}
}
