using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos CSV
	/// </summary>
	public class CsvFileViewModel : BaseFileViewModel
	{
		public CsvFileViewModel(StructuredFilesStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "parquet") {}

		/// <summary>
		///		Carga el archivo
		/// </summary>
		protected override async Task<(DataTable table, long totalRecords)> LoadFileAsync(bool countRecords, CancellationToken cancellationToken)
		{
			DataTable table = new LibCsvFiles.Controllers.CsvDataTableReader(FileParameters)
										.Load(FileName, ActualPage, RecordsPerPage, countRecords, out long totalRecords);

				// Evita las advertencias
				await Task.Delay(1);
				// Devuelve la tabla
				return (table, totalRecords);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override async Task SaveFileAsync(LibLogger.Models.Log.BlockLogModel block, string fileName, CancellationToken cancellationToken)
		{
			// Graba el archivo
			using (CsvReader reader = new CsvReader(FileParameters, FileColumns))
			{
				await using (ParquetDataWriterAsync writer = new ParquetDataWriterAsync(200_000))
				{
					// Log
					writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
					// Abre el archivo
					reader.Open(FileName);
					// Escribe el archivo
					await writer.WriteAsync(fileName, reader, cancellationToken);
				}
			}
			// Log
			block.Progress(System.IO.Path.GetFileName(fileName), 0, 0);
			block.Info($"Fin de la grabación del archivo '{fileName}'");
			SolutionViewModel.MainController.Logger.Flush();
		}

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected override async Task OpenFilePropertiesAsync(CancellationToken cancellationToken)
		{
			if (SolutionViewModel.MainController.OpenDialog(new CsvFilePropertiesViewModel(SolutionViewModel, this)) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				await LoadFileAsync(cancellationToken);
		}

		/// <summary>
		///		Cierra el viewmodel
		/// </summary>
		public override void Close()
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Parámetros del archivo
		/// </summary>
		public LibCsvFiles.Models.FileModel FileParameters { get; } = new LibCsvFiles.Models.FileModel();

		/// <summary>
		///		Columnas del archivo
		/// </summary>
		public List<LibCsvFiles.Models.ColumnModel> FileColumns { get; } = new List<LibCsvFiles.Models.ColumnModel>();
	}
}
