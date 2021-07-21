using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibBlobStorage.Metadata;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Managers.FileControllers.Storage
{
	/// <summary>
	///		Controlador de archivos en el storage
	/// </summary>
	internal class CloudFileStorage : BaseFileStorage
	{
		// Variables privadas
		private ICloudStorageManager _storageManager;

		internal CloudFileStorage(DbScriptProcessor processor, string storage, string container) : base(processor)
		{
			Storage = storage;
			Container = container.ToLower();
		}

		/// <summary>
		///		Abre la conexión
		/// </summary>
		internal override void Open()
		{
			_storageManager = Processor.GetCloudStorageManager(Storage);
		}

		/// <summary>
		///		Crea un directorio
		/// </summary>
		internal override async Task CreatePathAsync(string path)
		{
			// Evita los warnings
			await Task.Delay(1);
			// No hace nada más sólo implementa la inteface
		}

		/// <summary>
		///		Obtiene el stream asociado a un archivo
		/// </summary>
		internal async override Task<Stream> GetStreamAsync(string fileName, OpenFileMode mode)
		{
			switch (mode)
			{
				case OpenFileMode.Read:
					return await _storageManager.OpenReadAsync(Container, fileName);
				case OpenFileMode.Write:
					return await _storageManager.OpenWriteAsync(Container, fileName, true);
				default:
					throw new NotImplementedException($"Unknown open file mode {mode.ToString()}");
			}
		}

		/// <summary>
		///		Obtiene los archivos del directorio
		/// </summary>
		internal override async IAsyncEnumerable<string> GetFilesAsync(string folder, bool importFolder, string extension)
		{
			if (!importFolder)
				yield return folder;
			else
			{
				List<BlobModel> blobs = await _storageManager.ListBlobsAsync(Container, folder);

					// Obtiene los nombres de archivo
					foreach (BlobModel blob in blobs)
						if (blob.FullFileName.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
							yield return blob.FullFileName;
			}
		}

		/// <summary>
		///		Libera los datos
		/// </summary>
		protected override void DisposeData()
		{
			if (_storageManager != null)
				_storageManager = null;
		}

		/// <summary>
		///		Clave del storage
		/// </summary>
		internal string Storage { get; }

		/// <summary>
		///		Nombre del contenedor
		/// </summary>
		internal string Container { get; }
	}
}
