using System;

using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parquet
{
	/// <summary>
	///		Sentencia de importación desde archivos Parquet
	/// </summary>
	internal class SentenceImportParquet : Compiler.LibInterpreter.Processor.Sentences.SentenceBase
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
		///		Clave de la conexión destino
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
					_fileName = Table + ".parquet";
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
		///		Registros por bloque
		/// </summary>
		internal int RecordsPerBlock { get; set; } = 200_000;

		/// <summary>
		///		Tiempo de espera para la ejecución de la sentencia
		/// </summary>
		internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

		/// <summary>
		///		Columnas de entrada
		/// </summary>
		internal System.Collections.Generic.Dictionary<string, string> Mappings { get; } = new System.Collections.Generic.Dictionary<string, string>();
	}
}
