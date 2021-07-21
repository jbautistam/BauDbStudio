using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Storage
{
	/// <summary>
	///		Controlador de archivos
	/// </summary>
	internal abstract class BaseFileStorage : IDisposable
	{
		// Enumerados públicos
		/// <summary>
		///		Modo de apertura del archivo
		/// </summary>
		internal enum OpenFileMode
		{
			/// <summary>Modo de lectura</summary>
			Read,
			/// <summary>Modo de escritura</summary>
			Write
		}

		internal BaseFileStorage(DbScriptProcessor processor)
		{
			Processor = processor;
		}

		/// <summary>
		///		Abre la conexión
		/// </summary>
		internal abstract void Open();

		/// <summary>
		///		Crea un directorio
		/// </summary>
		internal abstract Task CreatePathAsync(string path);

		/// <summary>
		///		Obtiene los archivos del directorio
		/// </summary>
		internal abstract IAsyncEnumerable<string> GetFilesAsync(string folder, bool importFolder, string extension);

		/// <summary>
		///		Obtiene el Stream
		/// </summary>
		internal abstract Task<System.IO.Stream> GetStreamAsync(string fileName, OpenFileMode mode);

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public virtual void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				// Libera la memoria
				if (disposing)
					DisposeData();
				// Indica que se ha liberado la memoria
				Disposed = true;
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected abstract void DisposeData();

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		///		Procesador
		/// </summary>
		protected DbScriptProcessor Processor { get; }

		/// <summary>
		///		Indica si se ha liberao la memoria
		/// </summary>
		internal bool Disposed { get; private set; }
	}
}
