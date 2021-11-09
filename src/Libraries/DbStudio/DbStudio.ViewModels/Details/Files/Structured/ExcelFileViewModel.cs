using System;
using System.Data;

using Bau.Libraries.LibExcelFiles.Data;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Files.Structured
{
	/// <summary>
	///		ViewModel para visualización de archivos Excel
	/// </summary>
	public class ExcelFileViewModel : BaseFileViewModel
	{
		public ExcelFileViewModel(DbStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "parquet") {}

		/// <summary>
		///		Carga el archivo
		/// </summary>
		protected override DataTable LoadFile(bool countRecords, out long totalRecords)
		{
			// Inicializa los argumentos de salida
			if (countRecords)
				totalRecords = new ExcelDataTableReader().CountRows(FileName, 1, true);
			else
				totalRecords = 0;
			// Carga el archivo
			return new ExcelDataTableReader().LoadFile(FileName, 1, (ActualPage - 1) * RecordsPerPage, RecordsPerPage, true);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override void SaveFile(LibLogger.Models.Log.BlockLogModel block, string fileName)
		{
			ExcelDataTableReader excelReader = new ExcelDataTableReader();
			long rows = excelReader.CountRows(FileName, 1, true);

				// Graba el archivo
				using (IDataReader reader = excelReader.LoadFile(FileName, 1, 0, rows, true).CreateDataReader())
				{
					using (ParquetWriter writer = new ParquetWriter(200_000))
					{
						// Log
						writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
						// Escribe el archivo
						writer.Write(fileName, reader);
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
		protected override void OpenFileProperties()
		{
			// No hace nada, sólo implementa la interface
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
	}
}
