using System;
using System.Collections.Generic;

namespace Bau.Libraries.CloudStudio.Models
{
	/// <summary>
	///		Clase con los datos de la solución
	/// </summary>
	public class SolutionModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///		Directorio base de la solución
		/// </summary>
		public string Path
		{
			get 
			{
				if (string.IsNullOrWhiteSpace(FileName))
					return string.Empty;
				else
					return System.IO.Path.GetDirectoryName(FileName);
			}
		}

		/// <summary>
		///		Blob storage
		/// </summary>
		public Cloud.StorageModelCollection	Storages { get; } = new Cloud.StorageModelCollection();

		/// <summary>
		///		Carpetas abiertas
		/// </summary>
		public List<string> Folders { get; } = new List<string>();
	}
}
