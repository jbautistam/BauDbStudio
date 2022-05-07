using System;
using System.Collections.Generic;
using System.Data;

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
		protected override DataTable LoadFile(bool countRecords, out long totalRecords)
		{
			return new LibCsvFiles.Controllers.CsvDataTableReader(FileParameters)
													.Load(FileName, ActualPage, RecordsPerPage, countRecords, out totalRecords);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override void SaveFile(LibLogger.Models.Log.BlockLogModel block, string fileName)
		{
			// Graba el archivo
			using (CsvReader reader = new CsvReader(FileParameters, FileColumns))
			{
				using (ParquetWriter writer = new ParquetWriter(200_000))
				{
					// Log
					writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
					// Abre el archivo
					reader.Open(FileName);
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
			if (SolutionViewModel.MainController.OpenDialog(new CsvFilePropertiesViewModel(SolutionViewModel, this)) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				LoadFile();
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
