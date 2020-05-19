using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv
{
	internal class SentenceExportPartitionedCsv : SentenceCsvBase
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
		//		if (Columns.Count == 0)
		//			error += Environment.NewLine + "Undefined partition columns";
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

		/// <summary>
		///		Separador de partición
		/// </summary>
		internal string PartitionSeparator { get; set; } = "_#_";

		/// <summary>
		///		Columnas de la partición
		/// </summary>
		internal List<string> Columns { get; } = new List<string>();
	}
}
