using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Csv
{
	/// <summary>
	///		Sentencia de exportación a CSV
	/// </summary>
	internal class SentenceExportCsv : SentenceCsvBase
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
		///		Conexión origen
		/// </summary>
		internal string Source { get; set; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Comando de carga de datos para exportar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }
	}
}
