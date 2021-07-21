using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files
{
	/// <summary>
	///		Sentencia de exportación de todas las tablas de un esquema a archivos CSV
	/// </summary>
	internal class SentenceFileImportSchema : SentenceFileBase
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
		///		Reglas de exclusión de tablas 
		/// </summary>
		internal Files.SchemaExcludeRuleCollection ExcludeRules { get; } = new Files.SchemaExcludeRuleCollection();
	}
}
