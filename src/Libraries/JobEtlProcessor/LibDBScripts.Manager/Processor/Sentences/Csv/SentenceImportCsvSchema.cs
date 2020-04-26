using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Csv
{
	/// <summary>
	///		Sentencia de exportación de todas las tablas de un esquema a archivos CSV
	/// </summary>
	internal class SentenceImportCsvSchema : SentenceCsvBase
	{
		///// <summary>
		/////		Comprueba los datos
		///// </summary>
		//protected override string Validate(ProjectModel project)
		//{
		//	string error = string.Empty;

		//		// Comprueba los datos
		//		if (string.IsNullOrWhiteSpace(Target))
		//			error += Environment.NewLine + $"{nameof(Target)} can't be empty";
		//		if (RecordsPerBlock < 1)
		//			error += Environment.NewLine + $"{nameof(RecordsPerBlock)} can't be less than 1";
		//		// Devuelve el error
		//		return error;
		//}

		/// <summary>
		///		Conexión destino
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Directorio
		/// </summary>
		internal string Path { get; set; }

		/// <summary>
		///		Reglas de exclusión de tablas 
		/// </summary>
		internal SchemaExcludeRuleCollection ExcludeRules { get; } = new SchemaExcludeRuleCollection();
	}
}
