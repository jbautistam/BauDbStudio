using System;
using System.Collections.Generic;

using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects
{
	/// <summary>
	///		Opciones para la generación de los archivos de validación
	/// </summary>
	public class ScriptsValidationOptions
	{
		/// <summary>
		///		Modo de validación
		/// </summary>
		public enum ValidationMode
		{
			/// <summary>Comprueba archivos</summary>
			Files,
			/// <summary>Comprueba base de datos</summary>
			Database
		}

		/// <summary>
		///		Conexión
		/// </summary>
		public ConnectionModel Connection { get; set; } 
		
		/// <summary>
		///		Tablas para las que se genera la validación
		/// </summary>
		public List<ConnectionTableModel> Tables { get; set; } = new List<ConnectionTableModel>();

		/// <summary>
		///		Directorio de salida de los archivos
		/// </summary>
		public string OutputPath { get; set; } 

		/// <summary>
		///		Nombre de la variable de base de datos
		/// </summary>
		public string DataBaseVariable { get; set; }

		/// <summary>
		///		Indica el modo de validación
		/// </summary>
		public ValidationMode Mode { get; set; }

		/// <summary>
		///		Nombre de la variable de directorio de archivos
		/// </summary>
		public string MountPathVariable { get; set; }

		/// <summary>
		///		Contenido de la variable de directorio de archivos
		/// </summary>
		public string MountPathContent { get; set; }
		
		/// <summary>
		///		Formato de los archivos
		/// </summary>
		public SolutionManager.FormatType FormatType { get; set; }

		/// <summary>
		///		Subdirectorio de validación
		/// </summary>
		public string SubpathValidate { get; set; }

		/// <summary>
		///		Base de datos a comparar
		/// </summary>
		public string DatabaseTarget { get; set; }

		/// <summary>
		///		Indica si se debe generar un archivo QVS de validación
		/// </summary>
		public bool GenerateQvs { get; set; }
	}
}
