using System;
using System.Collections.Generic;

using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files
{
	/// <summary>
	///		Sentencia de importación a CSV
	/// </summary>
	internal class SentenceFileImport : SentenceFileBase
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
		//		if (string.IsNullOrWhiteSpace(FileName))
		//			error += Environment.NewLine + $"{nameof(FileName)} can't be empty";
		//		if (string.IsNullOrWhiteSpace(Table))
		//			error += Environment.NewLine + $"{nameof(Table)} can't be empty";
		//		if (RecordsPerBlock < 1)
		//			error += Environment.NewLine + $"{nameof(RecordsPerBlock)} can't be less than 1";
		//		// Devuelve el error
		//		return error;
		//}

		/// <summary>
		///		Indica si se deben importar los archivos de la carpeta
		/// </summary>
		internal bool ImportFolder { get; set; }

		/// <summary>
		///		Tabla sobre la que se van a importar los datos
		/// </summary>
		internal string Table { get; set; }

		/// <summary>
		///		Indica si la importación de este archivo es obligatoria
		/// </summary>
		internal bool Required { get; set; } = true;

		/// <summary>
		///		Mapeo de columnas. Clave: nombre de columna origen, Valor: nombre de columna destino
		/// </summary>
		internal Dictionary<string, string> Mappings { get; } = new Dictionary<string, string>();

		/// <summary>
		///		Columnas del archivo
		/// </summary>
		internal List<ColumnModel> Columns { get; } = new List<ColumnModel>();
	}
}
