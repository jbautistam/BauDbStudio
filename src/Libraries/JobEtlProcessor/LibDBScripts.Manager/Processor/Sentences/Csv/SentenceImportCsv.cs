using System;
using System.Collections.Generic;

using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Csv
{
	/// <summary>
	///		Sentencia de importación a CSV
	/// </summary>
	internal class SentenceImportCsv : SentenceCsvBase
	{
		// Variables privadas
		private string _fileName;

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
		///		Conexión destino
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		internal string FileName 
		{ 
			get
			{
				// Si no existe nombre de archivo, recoge el nombre de la tabla con la extensión de CSV
				if (string.IsNullOrWhiteSpace(_fileName))
					_fileName = Table + ".csv";
				// Devuelve el nombre de archivo
				return _fileName;
			}
			set { _fileName = value; }
		}

		/// <summary>
		///		Tabla sobre la que se van a importar los datos
		/// </summary>
		internal string Table { get; set; }

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
