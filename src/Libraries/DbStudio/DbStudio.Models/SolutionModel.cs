using System;
using System.Collections.Generic;

namespace Bau.Libraries.DbStudio.Models
{
	/// <summary>
	///		Clase con los datos de la solución
	/// </summary>
	public class SolutionModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Añade una carpeta a la solución
		/// </summary>
		public void AddFolder(string folder)
		{
			if (!string.IsNullOrWhiteSpace(folder))
			{
				bool found = false;

					// Comprueba si existe ya la carpeta
					foreach (string path in Folders)
						if (path.Equals(folder, StringComparison.CurrentCultureIgnoreCase))
							found = true;
					// Si no existe, la añade
					if (!found)
						Folders.Add(folder);
			}
		}

		/// <summary>
		///		Elimina una carpeta de la solución
		/// </summary>
		public void RemoveFolder(string folder)
		{
			if (!string.IsNullOrWhiteSpace(folder))
				for (int index = Folders.Count - 1; index >= 0; index--)
					if (!string.IsNullOrWhiteSpace(Folders[index]) &&
							Folders[index].Equals(folder, StringComparison.CurrentCultureIgnoreCase))
						Folders.RemoveAt(index);
		}

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
		///		Nombre del último archivo de parámetros seleccionado
		/// </summary>
		public string LastParametersFileName { get; set; }

		/// <summary>
		///		Conexiones
		/// </summary>
		public Connections.ConnectionModelCollection Connections { get; } = new Connections.ConnectionModelCollection();

		/// <summary>
		///		Modelos de distribución
		/// </summary>
		public Deployments.DeploymentModelCollection Deployments { get; } = new Deployments.DeploymentModelCollection();

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
