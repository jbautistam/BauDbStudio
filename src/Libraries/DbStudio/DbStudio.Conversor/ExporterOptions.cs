using System;
using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.DbStudio.Conversor
{
	/// <summary>
	///		Opciones de la exportación
	/// </summary>
	public class ExporterOptions
	{
		public ExporterOptions(string sourcePath, string targetPath)
		{
			SourcePath = sourcePath;
			TargetPath = targetPath;
		}

		/// <summary>
		///		Añade un parámetro
		/// </summary>
		public void AddParameter(string name, object value)
		{
			Parameters.Add(name, value);
		}

		/// <summary>
		///		Directorio origen de los archivos
		/// </summary>
		public string SourcePath { get; }

		/// <summary>
		///		Directorio destino de los archivos
		/// </summary>
		public string TargetPath { get; }

		/// <summary>
		///		Indica si se deben escribir los comentarios en los archivos finales
		/// </summary>
		public bool WriteComments { get; set; } = true;

		/// <summary>
		///		Indica si se deben reemplazar los argumentos por GetArgument
		/// </summary>
		public bool ReplaceArguments { get; set; } = true;

		/// <summary>
		///		Indica si se deben pasar a minúsculas los nombres de archivos
		/// </summary>
		public bool LowcaseFileNames { get; set; } = true;

		/// <summary>
		///		Parámetros
		/// </summary>
		internal NormalizedDictionary<object> Parameters { get; } = new NormalizedDictionary<object>();
	}
}
