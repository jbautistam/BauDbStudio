using System;

using Bau.Libraries.LibHelper.Files;

namespace Bau.Libraries.DbStudio.Conversor.Transformers
{
	/// <summary>
	///		Clase base para los procesos de transformación
	/// </summary>
	internal abstract class BaseTransformer
	{
		internal BaseTransformer(DatabrickExporter exporter, string targetPath, string fileName)
		{
			Exporter = exporter;
			TargetPath = targetPath;
			Source = fileName;
		}

		/// <summary>
		///		Transforma el archivo
		/// </summary>
		internal abstract void Transform();

		/// <summary>
		///		Graba un texto en un archivo con codificación UTF8 pero sin los caracteres iniciales de BOM. 
		/// </summary>
		/// <remarks>
		///		Databricks no reconoce en los notebook los archivos de texto UTF8 que se graban con los caracteres iniciales
		///	que indican que el archivo es UTF8, por eso se tiene que indicar en la codificación que se omitan estos caracteres
		///	<see cref="https://stackoverflow.com/questions/2502990/create-text-file-without-bom"/>
		/// </remarks>
		protected void SaveFileWithoutBom(string fileName, string content)
		{
			HelperFiles.SaveTextFile(fileName, content, new System.Text.UTF8Encoding(false));
		}

		/// <summary>
		///		Obtiene el nombre del archivo destino
		/// </summary>
		protected string GetTargetFileName(string extension)
		{
			return System.IO.Path.Combine(TargetPath, System.IO.Path.GetFileNameWithoutExtension(Source) + extension);
		}

		/// <summary>
		///		Procesador de exportación
		/// </summary>
		protected DatabrickExporter Exporter { get; }

		/// <summary>
		///		Directorio destino del archivo
		/// </summary>
		protected string TargetPath { get; }

		/// <summary>
		///		Nombre del archivo origen
		/// </summary>
		protected string Source { get; }
	}
}
