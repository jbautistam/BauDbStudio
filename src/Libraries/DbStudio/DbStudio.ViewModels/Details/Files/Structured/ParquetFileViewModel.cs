using System;
using System.Data;

using Bau.Libraries.LibParquetFiles.Readers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Files.Structured
{
	/// <summary>
	///		ViewModel para visualización de archivos parquet
	/// </summary>
	public class ParquetFileViewModel : BaseFileViewModel
	{
		public ParquetFileViewModel(DbStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "csv") {}

		/// <summary>
		///		Carga la página del archivo
		/// </summary>
		protected override DataTable LoadFile(bool countRecords, out long totalRecords)
		{
			return new ParquetDataTableReader().ParquetReaderToDataTable(FileName, (ActualPage - 1) * RecordsPerPage, RecordsPerPage, out totalRecords);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override void SaveFile(LibLogger.Models.Log.BlockLogModel block, string fileNameTarget)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

				// Escribe el archivo
				using (ParquetDataReader reader = new ParquetDataReader())
				{
					// Log
					writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileNameTarget), args.Records, args.Records + 1);
					// Abre el archivo
					reader.Open(FileName);
					// Escribe el archivo
					writer.Save(reader, fileNameTarget);
				}
				// Log
				block.Progress(System.IO.Path.GetFileName(fileNameTarget), 0, 0);
				block.Info($"Fin de la grabación del archivo '{fileNameTarget}'");
				SolutionViewModel.MainController.Logger.Flush();
		}

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected override void OpenFileProperties()
		{
			SolutionViewModel.MainController.OpenDialog(new ParquetFilePropertiesViewModel(SolutionViewModel, this));
		}

		/// <summary>
		///		Cierra el viewmodel
		/// </summary>
		public override void Close()
		{
			// ... no hace nada, sólo implementa la interface
		}
	}
}