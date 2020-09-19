using System;

using Bau.Libraries.LibHelper.Files;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.Conversor
{
	/// <summary>
	///		Exportador de archivos a notebooks de databricks
	/// </summary>
	public class DatabrickExporter
	{
		public DatabrickExporter(LibLogger.Core.LogManager logger, ExporterOptions options)
		{
			Logger = logger;
			Options = options;
		}

		/// <summary>
		///		Exporta los archivos
		/// </summary>
		public void Export()
		{
			using (BlockLogModel block = Logger.Default.CreateBlock(LogModel.LogType.Info, "Start copy files"))
			{	
				// Elimina el directorio destino
				HelperFiles.KillPath(Options.TargetPath);
				// Copia los directorios
				CopyPath(block, Options.SourcePath, Options.TargetPath);
				// Borra los directorios vacíos
				HelperFiles.KillEmptyPaths(Options.TargetPath);
				// Log
				block.Info("End copy files");
			}
		}

		/// <summary>
		///		Copia los archivos de un directorio
		/// </summary>
		private void CopyPath(BlockLogModel block, string sourcePath, string targetPath)
		{
			// Log
			block.Info($"Copying '{sourcePath}' a '{targetPath}'");
			// Copia los archivos
			CopyFiles(sourcePath, targetPath);
			// Copia recursivamente los directorios
			foreach (string path in System.IO.Directory.EnumerateDirectories(sourcePath))
				CopyPath(block, path, System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(path)));
		}

		/// <summary>
		///		Copia los archivos
		/// </summary>
		private void CopyFiles(string sourcePath, string targetPath)
		{
			// Crea el directorio 
			//? Sí, está dos veces, no sé porqué si se ejecuta dos veces consecutivas este método, la segunda vez no crea el directorio a menos que
			//? ejecute dos veces la instrucción
			HelperFiles.MakePath(targetPath);
			HelperFiles.MakePath(targetPath);
			// Copia los archivos
			foreach (string file in System.IO.Directory.EnumerateFiles(sourcePath))
				if (MustCopy(file))
					GetFileTransformer(targetPath, file).Transform();
		}

		/// <summary>
		///		Obtiene el transformador de archivos adecuado
		/// </summary>
		private Transformers.BaseTransformer GetFileTransformer(string targetPath, string fileName)
		{
			if (fileName.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase))
				return new Transformers.PythonFileTransformer(this, targetPath, fileName);
			else if (fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
				return new Transformers.SqlFileTransformer(this, targetPath, fileName);
			else if (fileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
				return new Transformers.SqlExtendedFileTransformer(this, targetPath, fileName);
			else
				throw new NotImplementedException($"Unknown file extension: {fileName}");
		}

		/// <summary>
		///		Indica si se debe copiar un archivo
		/// </summary>
		private bool MustCopy(string fileName)
		{
			return fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase) ||
				   fileName.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Logger
		/// </summary>
		internal LibLogger.Core.LogManager Logger { get; }

		/// <summary>
		///		Configuración de distribución
		/// </summary>
		internal ExporterOptions Options { get; }
	}
}