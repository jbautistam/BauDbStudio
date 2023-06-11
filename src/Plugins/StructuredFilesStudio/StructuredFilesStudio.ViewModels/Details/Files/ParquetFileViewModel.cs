using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibParquetFiles.Readers;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files
{
	/// <summary>
	///		ViewModel para visualización de archivos parquet
	/// </summary>
	public class ParquetFileViewModel : BaseFileViewModel
	{
		public ParquetFileViewModel(StructuredFilesStudioViewModel solutionViewModel, string fileName) : base(solutionViewModel, fileName, "csv") {}

		/// <summary>
		///		Carga la página del archivo
		/// </summary>
		protected override async Task<(DataTable table, long totalRecords)> LoadFileAsync(bool countRecords, CancellationToken cancellationToken)
		{
			return await new ParquetDataTableReader().ParquetReaderToDataTableAsync(FileName, (ActualPage - 1) * RecordsPerPage, RecordsPerPage, cancellationToken);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		protected override async Task SaveFileAsync(ILogger logger, string fileNameTarget, CancellationToken cancellationToken)
		{
			LibCsvFiles.Controllers.CsvDataReaderWriter writer = new LibCsvFiles.Controllers.CsvDataReaderWriter();

				// Evita las advertencias
				await Task.Delay(1);
				// Escribe el archivo
				using (ParquetDataReader reader = new ParquetDataReader())
				{
					// Log
					writer.Progress += (sender, args) => logger.LogInformation($"Save '{System.IO.Path.GetFileName(fileNameTarget)}' {args.Records:0,##0} / {args.Records + 1:#,##0}");
					// Abre el archivo
					await reader.OpenAsync(FileName, cancellationToken);
					// Escribe el archivo
					writer.Save(reader, fileNameTarget);
				}
				// Log
				logger.LogInformation($"Fin de la grabación del archivo '{fileNameTarget}'");
		}

		/// <summary>
		///		Abre las propiedades del archivo
		/// </summary>
		protected override async Task OpenFilePropertiesAsync(CancellationToken cancellationToken)
		{
			await Task.Delay(1, cancellationToken);
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