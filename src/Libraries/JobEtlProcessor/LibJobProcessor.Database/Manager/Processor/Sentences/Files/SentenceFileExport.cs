using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files
{
	/// <summary>
	///		Sentencia de exportación a archivo
	/// </summary>
	internal class SentenceFileExport : SentenceFileBase
	{
		///// <summary>
		/////		Comprueba los datos
		///// </summary>
		//protected override string Validate(ProjectModel project)
		//{
		//	string error = string.Empty;

		//		// Comprueba los datos
		//		if (string.IsNullOrWhiteSpace(Source))
		//			error += Environment.NewLine + $"{nameof(Source)} can't be empty";
		//		if (string.IsNullOrWhiteSpace(FileName))
		//			error += Environment.NewLine + $"{nameof(FileName)} can't be empty";
		//		if (string.IsNullOrWhiteSpace(LoadCommand))
		//			error += Environment.NewLine + $"{nameof(LoadCommand)} can't be empty";
		//		if (RecordsPerBlock < 1)
		//			error += Environment.NewLine + $"{nameof(RecordsPerBlock)} can't be less than 1";
		//		// Devuelve el error
		//		return error;
		//}

		/// <summary>
		///		Número de registros por grupo de filas
		/// </summary>
		internal int RowGroupSize { get; set; } = 45_000;

		/// <summary>
		///		Comando de carga de datos para exportar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }

		/// <summary>
		///		Indica si los archivos están en el storage
		/// </summary>
		internal bool FilesAtStorage 
		{ 
			get { return !string.IsNullOrWhiteSpace(Target) && !string.IsNullOrWhiteSpace(Container); }
		}
	}
}
