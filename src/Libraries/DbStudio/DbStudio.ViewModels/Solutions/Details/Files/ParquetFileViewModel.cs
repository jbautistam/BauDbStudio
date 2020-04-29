using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibParquetFiles;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos parquet
	/// </summary>
	public class ParquetFileViewModel : BaseFileViewModel
	{
		public ParquetFileViewModel(SolutionViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "csv") {}

		/// <summary>
		///		Carga la página del archivo
		/// </summary>
		protected override DataTable LoadFile(out int totalRecords)
		{
			return new ParquetDataTableReader().ParquetReaderToDataTable(FileName, (ActualPage - 1) * RecordsPerPage, RecordsPerPage, out totalRecords);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override void SaveFile(LibLogger.Models.Log.BlockLogModel block, string fileName)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

				// Escribe el archivo
				using (ParquetDataReader reader = new ParquetDataReader(FileName))
				{
					// Log
					writer.Progress += (sender, args) => block.Progress(System.IO.Path.GetFileName(fileName), args.Records, args.Records + 1);
					// Escribe el archivo
					writer.Save(reader, fileName);
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
			//TODO --> Por ahora no hace nada
		}
	}
}