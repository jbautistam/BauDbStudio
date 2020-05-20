using System;
using System.Data;

using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.LibParquetFiles;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos CSV
	/// </summary>
	public class CsvFileViewModel : BaseFileViewModel
	{
		public CsvFileViewModel(SolutionViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "parquet") {}

		/// <summary>
		///		Carga el archivo
		/// </summary>
		protected override DataTable LoadFile(bool countRecords, out int totalRecords)
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
			using (CsvReader reader = new CsvReader(FileName, FileParameters, null))
			{
				using (ParquetDataWriter writer = new ParquetDataWriter(fileName))
				{
					// Log
					writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
					// Escribe el archivo
					writer.Write(reader);
				}
			}
			// Log
			block.Progress(System.IO.Path.GetFileName(fileName), 0, 0);
			block.Info($"Fin de la grabación del archivo '{fileName}'");
			SolutionViewModel.MainViewModel.MainController.Logger.Flush();
		}

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected override void OpenFileProperties()
		{
			if (SolutionViewModel.MainViewModel.MainController.OpenDialog(new CsvFilePropertiesViewModel(SolutionViewModel, FileParameters)) == 
					BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				LoadFile();
		}

		/// <summary>
		///		Parámetros del archivo
		/// </summary>
		public LibCsvFiles.Models.FileModel FileParameters { get; } = new LibCsvFiles.Models.FileModel();
	}
}
