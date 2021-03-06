﻿using System;
using System.Collections.Generic;

namespace Bau.Libraries.DbStudio.Models
{
	/// <summary>
	///		Clase con los datos de la solución
	/// </summary>
	public class SolutionModel : LibDataStructures.Base.BaseExtendedModel
	{
		// Variables privadas
		private string _lastConnectionParameters = string.Empty;

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
		///		Añade el parámetro
		/// </summary>
		private void AddConnectionParameter(string parametersFile)
		{
			if (!string.IsNullOrWhiteSpace(parametersFile))
				QueueConnectionParameters.Add(parametersFile);
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
		///		Nombre del último archivo de parámetros de conexión seleccionado
		/// </summary>
		public string LastConnectionParametersFileName 
		{ 
			get { return _lastConnectionParameters; }
			set
			{
				// Guarda el parámetro
				_lastConnectionParameters = value;
				// lo añade a la cola
				AddConnectionParameter(_lastConnectionParameters);
			}
		}

		/// <summary>
		///		Cola de los últimos archivos de parámetros
		/// </summary>
		public LibDataStructures.Collections.QueueLimited<string> QueueConnectionParameters { get; } = new LibDataStructures.Collections.QueueLimited<string>();

		/// <summary>
		///		Nombre del último archivo de parámetros para proyectos ETL seleccionado
		/// </summary>
		public string LastEtlParametersFileName { get; set; }

		/// <summary>
		///		Id de la última conexión seleccionada
		/// </summary>
		public string LastConnectionSelectedGlobalId { get; set; }

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
