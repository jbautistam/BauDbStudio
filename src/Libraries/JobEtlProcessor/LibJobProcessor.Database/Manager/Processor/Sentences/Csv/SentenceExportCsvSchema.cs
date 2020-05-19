using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv
{
	/// <summary>
	///		Sentencia de exportación de todas las tablas de un esquema a archivos CSV
	/// </summary>
	internal class SentenceExportCsvSchema : SentenceCsvBase
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
		///		Directorio
		/// </summary>
		internal string Path { get; set; }

		/// <summary>
		///		Indica si se deben borrar los archivos antiguos
		/// </summary>
		internal bool DeleteOldFiles { get; set; }

		/// <summary>
		///		Reglas de exclusión de tablas 
		/// </summary>
		internal SchemaExcludeRuleCollection ExcludeRules { get; } = new SchemaExcludeRuleCollection();
	}
}
